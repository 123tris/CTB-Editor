using TMPro;
using UnityEngine;

public class BeatsnapDivisor : MonoBehaviour
{
    public static BeatsnapDivisor Instance;

    [SerializeField] private TextMeshProUGUI displayDivision;

    ///<summary>Beatsnap division (contains either 1, 2, 4, 8, or 16)</summary>
    [HideInInspector] public int division;
    private UnityEngine.UI.Slider slider;

    private int[] divisionSettings = { 1, 2, 3, 4, 6, 8, 12, 16};

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        slider = GetComponent<UnityEngine.UI.Slider>();
    }

    void Update()
    {
        division = (int)Mathf.Pow(2, slider.value);
        displayDivision.text = "1/" + division;
    }

    public void SetDivision(int div)
    {
        slider.value = div;
        division = div;
    }
}
