using TMPro;
using UnityEngine;

public class ColumnSlider : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI columnText;
    private UnityEngine.UI.Slider slider;

    private void Start()
    {
        slider = GetComponent<UnityEngine.UI.Slider>();
    }

    void Update()
    {
        Grid.Instance.columns = slider.value + 0.001f;
        columnText.text = slider.value.ToString();
    }
}
