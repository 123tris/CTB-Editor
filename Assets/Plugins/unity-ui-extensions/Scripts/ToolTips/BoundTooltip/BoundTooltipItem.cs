///Credit Martin Nerurkar // www.martin.nerurkar.de // www.sharkbombs.com
///Sourced from - http://www.sharkbombs.com/2015/02/10/tooltips-with-the-new-unity-ui-ugui/

using TMPro;

namespace UnityEngine.UI.Extensions
{
    [AddComponentMenu("UI/Extensions/Bound Tooltip/Tooltip Item")]
    public class BoundTooltipItem : MonoBehaviour
    {
        public bool IsActive => gameObject.activeSelf;

        public TextMeshProUGUI TooltipText;
        public Vector3 ToolTipOffset;

        public Vector2 backgroundMargin;
        private RectTransform rectTransform;

        void Awake()
        {
            instance = this;
            rectTransform = GetComponent<RectTransform>();
            if(!TooltipText) TooltipText = GetComponentInChildren<TextMeshProUGUI>();
            HideTooltip();
        }

        void Update()
        {
            rectTransform.sizeDelta = TooltipText.GetComponent<RectTransform>().sizeDelta + backgroundMargin;
        }

        public void ShowTooltip(string text, Vector3 pos)
        {
            if (TooltipText.text != text)
                TooltipText.text = text;

            transform.position = pos + ToolTipOffset;

            gameObject.SetActive(true);
        }

        public void HideTooltip()
        {
            gameObject.SetActive(false);
        }

        // Standard Singleton Access
        private static BoundTooltipItem instance;
        public static BoundTooltipItem Instance
        {
            get
            {
                if (instance == null)
                    instance = GameObject.FindObjectOfType<BoundTooltipItem>();
                return instance;
            }
        }
    }
}

 
