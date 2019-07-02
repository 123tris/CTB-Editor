using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using SliderUI = UnityEngine.UI.Slider;
public class TimeLine : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private static SliderUI slider;
    /// <summary>
    /// This is the time in milliseconds which the timeline is currently set at
    /// </summary>
    public static int currentTimeStamp => (int) slider.value;

    [SerializeField] private Transform level;
    public static TimeLine instance;
    private static bool hovering;
    private MusicPlayer player;

    void Awake()
    {
        instance = this;
    }

    void Start ()
    {
        player = FindObjectOfType<MusicPlayer>();
        slider = GetComponent<SliderUI>();
        slider.onValueChanged.AddListener(OnSliderChange);
    }

    private void OnSliderChange(float value)
    {
        level.position = new Vector2(level.position.x, -value);
        if (hovering && Input.GetMouseButton(0))
            player.SetPlayback(value);
    }

    public void SetCurrentTimeStamp(int value)
    {
        slider.value = value;
    }

    public void SetTimeLineLength(int lengthInMS)
    {
        slider.maxValue = lengthInMS;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hovering = false;
    }
}
