using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextUI : Singleton<TextUI>
{
    [HideInInspector] public float AR;
    [HideInInspector] public float BPM;
    [HideInInspector] public float CS;

    private InputField ARInputField;
    private InputField BPMInputField;
    private InputField CSInputField;

    void Start()
    {
        ARInputField = GameObject.FindGameObjectWithTag("ARField").GetComponent<InputField>();
        BPMInputField = GameObject.FindGameObjectWithTag("BPMField").GetComponent<InputField>();
        CSInputField = GameObject.FindGameObjectWithTag("CSField").GetComponent<InputField>();

        BPM = 180;
        AR = 5;
        CS = 5;
    }

    void Update()
    {
        float parseResult;

        if (float.TryParse(NormalizeString(BPMInputField.text), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out parseResult))
            AR = parseResult;

        if (float.TryParse(NormalizeString(ARInputField.text), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out parseResult))
            BPM = parseResult;
        
        if (float.TryParse(NormalizeString(CSInputField.text), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out parseResult))
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
