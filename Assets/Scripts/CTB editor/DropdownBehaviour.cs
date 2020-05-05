using UnityEngine;

public class DropdownBehaviour : ButtonBehaviour
{
    [SerializeField] private GameObject shadow;
    [SerializeField] private GameObject panel;

    void Update()
    {
        if (shadow.activeSelf && Input.GetKeyDown(KeyCode.Escape))
            ToggleDropdown();
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
