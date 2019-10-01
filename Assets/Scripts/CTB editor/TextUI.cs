using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextUI : MonoBehaviour
{
    [SerializeField] private InputField ARInputField;
    [SerializeField] private InputField BPMInputField;
    [SerializeField] private InputField CSInputField;

    void Start()
    {
        if (!ARInputField || !BPMInputField || !CSInputField) //if one of them is null
            Debug.LogError("Not all input fields in TextUI is set", gameObject);

        BeatmapSettings.BPM = 180;
        BeatmapSettings.AR = 5;
        BeatmapSettings.CS = 5;

        ARInputField.onEndEdit.AddListener(input => UpdateValue(ref BeatmapSettings.AR, input));
        BPMInputField.onEndEdit.AddListener(input => UpdateValue(ref BeatmapSettings.AR, input));
        CSInputField.onEndEdit.AddListener(input =>
        {
            UpdateValue(ref BeatmapSettings.AR, input);
            HitObjectManager.UpdateAllCircleSize();
        });
    }

    private void LoadSettings()
    {
        ARInputField.text = BeatmapSettings.AR.ToString();
        CSInputField.text = BeatmapSettings.CS.ToString();
        BPMInputField.text = BeatmapSettings.BPM.ToString();
    }

    void UpdateValue(ref float value, string text)
    {
        float parseResult;
        if (float.TryParse(NormalizeString(text), NumberStyles.Float, CultureInfo.InvariantCulture, out parseResult))
        {
            value = parseResult;
        }
    }

    void Update()
    {
        LoadSettings();
    }

    private static string NormalizeString(string str)
    {
        return str.Replace(",", ".");
    }

}
