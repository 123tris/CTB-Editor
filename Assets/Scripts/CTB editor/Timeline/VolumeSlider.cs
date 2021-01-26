using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using USlider = UnityEngine.UI.Slider;

[RequireComponent(typeof(USlider))]
public class VolumeSlider : MonoBehaviour
{
    private USlider slider;
    [SerializeField] private TextMeshProUGUI text;

    private MusicPlayer musicPlayer;

    void Start()
    {
        musicPlayer = FindObjectOfType<MusicPlayer>();
        slider = GetComponent<USlider>();
    }

    void Update()
    {
        musicPlayer.SetVolume(slider.value/300);
        text.text = slider.value + "%";
    }
}
