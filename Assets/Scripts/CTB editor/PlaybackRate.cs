using TMPro;
using UnityEngine;
using USlider = UnityEngine.UI.Slider;

[RequireComponent(typeof(USlider))]
public class PlaybackRate : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    private USlider slider;

    private void Start()
    {
        slider = GetComponent<USlider>();
        slider.onValueChanged.AddListener(OnSliderChange);
    }

    private void OnSliderChange(float sliderValue)
    {
        MusicPlayer.instance.SetPlaybackSpeed(sliderValue/100);
        text.text = sliderValue + "%";
    }
}
