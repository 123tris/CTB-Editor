using System;
using CooldownManagerNamespace;
using UnityEngine;

public class PopupManager : MonoBehaviour
{
    [SerializeField] private GameObject popupPrefab;
    [SerializeField] private GameObject questionPopupPrefab;
    public GameObject shadow;

    public static PopupManager Instance;

    void Awake() => Instance = this;

    /// <summary>Displays a question popup where you can select yes or no</summary>
    public static void ShowYesNo(string text, Action onYesPressed, Action onNoPressed)
    {
        PopupWindow currentPopup = Instantiate(Instance.questionPopupPrefab, Instance.transform).GetComponent<PopupWindow>();

        if (onNoPressed == null)
            onNoPressed = () => { };

        currentPopup.Show(text, onYesPressed, onNoPressed);
        Instance.shadow.SetActive(true);
        currentPopup.onClose += () => Instance.shadow.SetActive(false);
    }

    public static void ShowYesNo(string text, Action onYesPressed) => ShowYesNo(text, onYesPressed, () => { });

    /// <summary>Shows a popup with text and an ok button</summary>
    public static void Show(string text, Action onOkPressed)
    {
        var currentPopup = Instantiate(Instance.popupPrefab,Instance.transform).GetComponent<PopupWindow>();
        currentPopup.Show(text, onOkPressed);

        Instance.shadow.SetActive(true);
        currentPopup.onClose += () => Instance.shadow.SetActive(false);
    }

    /// <summary>Shows a popup with text and an ok button</summary>
    public static void Show(string text) => Show(text, () => { });
}