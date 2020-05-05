using UnityEngine;

public class SettingsDropdown : ButtonBehaviour
{
    [SerializeField] private GameObject settingsWindow;

    protected override void OnClick()
    {
        settingsWindow.SetActive(true);
        PopupManager.Instance.shadow.SetActive(true);
    }
}