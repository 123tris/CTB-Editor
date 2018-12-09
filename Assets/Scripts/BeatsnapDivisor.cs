using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BeatsnapDivisor : Singleton<BeatsnapDivisor>
{
    [SerializeField] private Text displayDivision;

    [HideInInspector] public int division;
    private UnityEngine.UI.Slider slider;

	void Start ()
	{
	    slider = GetComponent<UnityEngine.UI.Slider>();
	}
	
	void Update ()
	{
	    division = (int) Mathf.Pow(2,slider.value);
	    displayDivision.text = "1/" + division;
	}
}
