using UnityEngine;

public class DropdownBehaviour : ButtonBehaviour
{
    [SerializeField] private GameObject shadow;
    [SerializeField] private GameObject panel;

    bool buttonHovered;

    RectTransform buttonTransform;
    RectTransform panelRect;
    void Start()
    {
        buttonTransform = GetComponent<RectTransform>();
        panelRect = panel.GetComponent<RectTransform>();
    }

    void Update()
    {
        if (shadow.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                ToggleDropdown();
            else if (Input.GetMouseButtonDown(0))
            {
                buttonHovered = RectTransformUtility.RectangleContainsScreenPoint(buttonTransform, Input.mousePosition);
                bool hovering = RectTransformUtility.RectangleContainsScreenPoint(panelRect, Input.mousePosition) || buttonHovered;

                if (!hovering)
                    ToggleDropdown();
            }
        }
    }

    public void ToggleDropdown()
    {
        shadow.SetActive(!shadow.activeSelf);
        panel.SetActive(!panel.activeSelf);
    }

    protected override void OnClick()
    {
        ToggleDropdown();
    }
}
