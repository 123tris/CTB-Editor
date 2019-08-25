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
            Debug.LogError("Not all input fields in TextUI is set",gameObject);

        BeatmapSettings.BPM = 180;
        BeatmapSettings.AR = 5;
        BeatmapSettings.CS = 5;
    }

    void Update()
    {
        float parseResult;

        if (float.TryParse(NormalizeString(ARInputField.text), NumberStyles.Float, CultureInfo.InvariantCulture, out parseResult))
            BeatmapSettings.AR = parseResult;

        if (float.TryParse(NormalizeString(BPMInputField.text), NumberStyles.Float, CultureInfo.InvariantCulture, out parseResult))
            BeatmapSettings.BPM = parseResult;
        
        if (float.TryParse(NormalizeString(CSInputField.text), NumberStyles.Float, CultureInfo.InvariantCulture, out parseResult))
        {
            if (parseResult == BeatmapSettings.CS)
                return;

            BeatmapSettings.CS = parseResult;
            HitObjectManager.UpdateAllCircleSize();
        }
    }

    private static string NormalizeString(string str)
    {
        return str.Replace(",", ".");
    }

}
