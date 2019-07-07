using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextUI : Singleton<TextUI>
{
    [HideInInspector] public float AR;
    [HideInInspector] public float BPM;
    [HideInInspector] public float CS;

    [SerializeField] private InputField ARInputField;
    [SerializeField] private InputField BPMInputField;
    [SerializeField] private InputField CSInputField;

    void Start()
    {
        if (!ARInputField || !BPMInputField || !CSInputField) //if one of them is null
            Debug.LogError("Not all input fields in TextUI is set",gameObject);

        BPM = 180;
        AR = 5;
        CS = 5;
    }

    void Update()
    {
        float parseResult;

        if (float.TryParse(NormalizeString(ARInputField.text), NumberStyles.Float, CultureInfo.InvariantCulture, out parseResult))
            AR = parseResult;

        if (float.TryParse(NormalizeString(BPMInputField.text), NumberStyles.Float, CultureInfo.InvariantCulture, out parseResult))
            BPM = parseResult;
        
        if (float.TryParse(NormalizeString(CSInputField.text), NumberStyles.Float, CultureInfo.InvariantCulture, out parseResult))
        {
            if (parseResult == CS)
                return;

            CS = parseResult;
            HitObjectManager.instance.UpdateAllCircleSize();
        }
    }

    private static string NormalizeString(string str)
    {
        return str.Replace(",", ".");
    }

}
