using System;
using CooldownManagerNamespace;
using UnityEngine;

public class PopupManager : MonoBehaviour
{
    [SerializeField] private GameObject popupPrefab;
    [SerializeField] private GameObject questionPopupPrefab;

    /// <summary>Displays a question popup where you can select yes or no</summary>
    public void ShowYesNo(string text, Action onYesPressed, Action onNoPressed)
    {
        PopupWindow currentPopup = Instantiate(questionPopupPrefab, transform).GetComponent<PopupWindow>();

        if (onNoPressed == null)
            onNoPressed = () => { };

        currentPopup.Show(text, onYesPressed, onNoPressed);
    }

    public void ShowYesNo(string text, Action onYesPressed) => ShowYesNo(text, onYesPressed, () => { });

    /// <summary>Shows a popup with text and an ok button</summary>
    public void Show(string text, Action onOkPressed)
    {
        var currentPopup = Instantiate(popupPrefab,transform).GetComponent<PopupWindow>();
        currentPopup.Show(text, onOkPressed);
    }

    /// <summary>Shows a popup with text and an ok button</summary>
    public void Show(string text) => Show(text, () => { });
}