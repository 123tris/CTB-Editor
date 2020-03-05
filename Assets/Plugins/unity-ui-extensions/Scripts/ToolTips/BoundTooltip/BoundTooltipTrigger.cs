using UnityEngine.EventSystems;
using static CooldownManagerNamespace.CooldownManager;


namespace UnityEngine.UI.Extensions
{
    [AddComponentMenu("UI/Extensions/Bound Tooltip/Tooltip Trigger")]
	public class BoundTooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
	{
		[TextAreaAttribute]
		public string text;

		public bool useMousePosition = false;

		public Vector3 offset;

        [SerializeField] private float popupDelay = 1;

        private Coroutine coroutine;

		public void OnPointerEnter(PointerEventData eventData)
		{
			if (useMousePosition)
			{
				StartHover(new Vector3(eventData.position.x, eventData.position.y, 0f));
			}
			else
			{
				StartHover(transform.position + offset);
			}
		}

		public void OnSelect(BaseEventData eventData)
		{
			StartHover(transform.position);
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			StopHover();
		}

		public void OnDeselect(BaseEventData eventData)
		{
			StopHover();
		}

		void StartHover(Vector3 position)
		{
            coroutine = Cooldown(popupDelay,() => BoundTooltipItem.Instance.ShowTooltip(text, position),"Tooltip Hover");
		}

		void StopHover()
		{
            if (coroutine != null) StopCoroutine(coroutine);
            BoundTooltipItem.Instance.HideTooltip();
		}
	}
}
