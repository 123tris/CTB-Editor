using TMPro;
using UnityEngine;

public class BeatsnapDivisor : Singleton<BeatsnapDivisor>
{
    [SerializeField] private TextMeshProUGUI displayDivision;

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

    public void SetDivision(int div)
    {
        slider.value = div;
        division = div;
    }
}
