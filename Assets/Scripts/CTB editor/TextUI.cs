using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//TODO: Remove this class from the earth and replace it with something that doesn't encourage spaghetti
public class TextUI : MonoBehaviour
{
    [SerializeField] private InputField ARInputField;
    [SerializeField] private InputField BPMInputField;
    [SerializeField] private InputField CSInputField;
    [SerializeField] private InputField HPInputField;

    void Start()
    {
        if (!ARInputField || !BPMInputField || !CSInputField) //if one of them is null
            Debug.LogError("Not all input fields in TextUI is set", gameObject);

        BeatmapSettings.BPM = 180;
        BeatmapSettings.AR = 9;
        BeatmapSettings.CS = 4;
        BeatmapSettings.HP = 4;

        LoadSettings();

        ARInputField.onEndEdit.AddListener(input => UpdateValue(ref BeatmapSettings.AR, input));
        BPMInputField.onEndEdit.AddListener(input => UpdateValue(ref BeatmapSettings.BPM, input));
        CSInputField.onEndEdit.AddListener(input =>
        {
            UpdateValue(ref BeatmapSettings.CS, input);
            HitObjectManager.UpdateAllCircleSize();
        });
        HPInputField.onEndEdit.AddListener(input => UpdateValue(ref BeatmapSettings.HP, input));
    }

    public void LoadSettings()
    {
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
