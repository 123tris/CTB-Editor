using UnityEngine;
using UnityEngine.UI;

public class ToggleBehaviour : MonoBehaviour
{
    public bool toggled;

    [Header("Toggle settings")]
    [SerializeField] private float disabledAlpha = 0.25f;
    [SerializeField] private float enabledAlpha = 1;
    [SerializeField] private float fadeDuration = 0.1f;

    protected Button button;

    protected virtual void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnToggle);

        ApplyAlpha();
    }

    public virtual void OnToggle()
    {
        toggled = !toggled;
        ApplyAlpha();
    }

    public void ApplyAlpha()
    {
        var alpha = toggled ? enabledAlpha : disabledAlpha;
        button.colors = button.colors.OverrideAlpha(alpha);
    }
}
