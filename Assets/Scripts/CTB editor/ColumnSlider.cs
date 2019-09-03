using TMPro;
using UnityEngine;

public class ColumnSlider : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI columnText;
    private UnityEngine.UI.Slider slider;

    private void Start()
    {
        slider = GetComponent<UnityEngine.UI.Slider>();
        slider.onValueChanged.AddListener(OnValueChanged);
    }

    void OnValueChanged(float value)
    {
        Grid.Instance.columns = value + 0.001f;
        columnText.text = value.ToString();
    }
}
