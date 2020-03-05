using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using SliderUI = UnityEngine.UI.Slider;

public class TimeLine : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private static SliderUI slider;

    private static int hitIndicatorOffset
    {
        get
        {
            Grid grid = Grid.Instance;
            return (int) (grid.height / 10 * grid.GetVisibleTimeRange() / grid.height);
        }
    }

    /// <summary>
    /// This is the time in milliseconds which the timeline is currently set at
    /// </summary>
    public static int currentTimeStamp => (int) slider.value - hitIndicatorOffset;

    [SerializeField] private Transform level;
    [SerializeField] public int scrollSpeed = 1;

    public static TimeLine instance;
    private static bool hovering;
    private MusicPlayer player;

    void Awake()
    {
        instance = this;
        player = FindObjectOfType<MusicPlayer>();
        slider = GetComponent<SliderUI>();
        slider.onValueChanged.AddListener(OnSliderChange);
        level.position = new Vector2(level.position.x, 0);
    }

    private void OnSliderChange(float value)
    {
        level.position = new Vector2(level.position.x, -value * (Grid.Instance.height/Grid.Instance.GetVisibleTimeRange()));
        if (Input.GetMouseButton(0))
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
