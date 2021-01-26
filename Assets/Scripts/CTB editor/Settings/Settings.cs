using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//TODO: Remove this class from the earth and replace it with something that doesn't encourage spaghetti
public class Settings : MonoBehaviour
{
    [SerializeField] private TMP_InputField ARInputField;
    [SerializeField] private TMP_InputField BPMInputField;
    [SerializeField] private TMP_InputField CSInputField;
    [SerializeField] private TMP_InputField HPInputField;

    Settings()
    {
        BeatmapSettings.BPM = 180;
        BeatmapSettings.AR = 9;
        BeatmapSettings.CS = 4;
        BeatmapSettings.HP = 4;
    }

    void Start()
    {
        if (!ARInputField || !BPMInputField || !CSInputField) //if one of them is null
            Debug.LogError("Not all input field references in Settings are set", gameObject);

        ARInputField.onEndEdit.AddListener(input => UpdateValue(ref BeatmapSettings.AR, input));
        BPMInputField.onEndEdit.AddListener(input => UpdateValue(ref BeatmapSettings.BPM, input));
        CSInputField.onEndEdit.AddListener(input =>
        {
            UpdateValue(ref BeatmapSettings.CS, input);
        });
        HPInputField.onEndEdit.AddListener(input => UpdateValue(ref BeatmapSettings.HP, input));
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            gameObject.SetActive(false);
            PopupManager.Instance.shadow.SetActive(false);
        }
    }

    private void OnEnable()
    {
        //Load settings
        ARInputField.text = BeatmapSettings.AR.ToString();
        CSInputField.text = BeatmapSettings.CS.ToString();
        BPMInputField.text = BeatmapSettings.BPM.ToString();
        HPInputField.text = BeatmapSettings.HP.ToString();
    }

    void UpdateValue(ref float value, string text)
    {
        float parseResult;
        if (float.TryParse(NormalizeString(text), NumberStyles.Float, CultureInfo.InvariantCulture, out parseResult))
        {
            value = parseResult;
        }
    }

    private static string NormalizeString(string str)
    {
        return str.Replace(",", ".");
    }

}
