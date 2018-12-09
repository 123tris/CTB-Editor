using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BpmUI : Singleton<BpmUI>
{
    [HideInInspector] public float BPM;

    private InputField inputField;

    void Start()
    {
        inputField = GetComponent<InputField>();
    }

    void Update()
    {
        float parseResult;
        if (float.TryParse(inputField.text, out parseResult))
            BPM = parseResult;
    }

}
