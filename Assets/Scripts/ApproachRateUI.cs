using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ApproachRateUI : Singleton<ApproachRateUI>
{
    [HideInInspector] public float approachRate;

    private InputField inputField;

    void Start()
    {
        inputField = GetComponent<InputField>();

    }

    void Update()
    {
        float parseResult;
        if (float.TryParse(inputField.text, out parseResult))
            approachRate = parseResult;
    }
}
