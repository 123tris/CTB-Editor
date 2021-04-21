using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static CooldownManagerNamespace.CooldownManager;

public class PopupWindow : MonoBehaviour
{
    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;
    [SerializeField] private Button okButton;
    [SerializeField] private bool isQuestionPopup;
    [SerializeField] private TextMeshProUGUI textUI;

    public Action onClose = () => { };

    void Start()
    {
        //Clean up popup after button press
        yesButton?.onClick.AddListener(() => Destroy(gameObject));
        noButton?.onClick.AddListener(() => Destroy(gameObject));
        okButton?.onClick.AddListener(() => Destroy(gameObject));
    }

    public void Show(string text, Action onYesPressed, Action onNoPressed)
    {
        textUI.text = text;

        yesButton.onClick.AddListener(onYesPressed.Invoke);
        noButton.onClick.AddListener(onNoPressed.Invoke);
    }

    public void Show(string text, Action onOkPressed)
    {
        textUI.text = text;

        okButton.onClick.AddListener(onOkPressed.Invoke);
    }

    private void OnDestroy()
    {
        onClose.Invoke();
    }
}
