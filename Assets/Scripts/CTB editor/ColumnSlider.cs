using TMPro;
using UnityEngine;

public class ColumnSlider : MonoBehaviour
{
    private int[] columnValues;

    [SerializeField] private TextMeshProUGUI columnText;
    private UnityEngine.UI.Slider slider;

    private void Start()
    {
        columnValues = new[] {0,8,16,32,48,64,128};

        slider = GetComponent<UnityEngine.UI.Slider>();
        slider.wholeNumbers = true;
        slider.maxValue = columnValues.Length - 1;
    }

    void Update()
    {
        Grid.Instance.columns = columnValues[(int) slider.value] + 0.001f;
        columnText.text = columnValues[(int) slider.value].ToString();
    }
}
