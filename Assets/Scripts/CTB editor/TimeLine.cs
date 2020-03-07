using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI.Extensions;
using SliderUI = UnityEngine.UI.Slider;

public class TimeLine : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private static SliderUI slider;

    public static int hitIndicatorOffset
    {
        get
        {
            Grid grid = Grid.Instance;
            return (int)(grid.height / 10 * grid.GetVisibleTimeRange() / grid.height);
        }
    }

    /// <summary>
    /// This is the time in milliseconds which the timeline is currently set at
    /// </summary>
    public static int currentTimeStamp => (int)slider.value - hitIndicatorOffset;

    public int scrollSpeed = 1;
    [SerializeField] private int scrollMultiplier = 5;

    [SerializeField] private Transform level;

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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
            scrollSpeed *= scrollMultiplier;

        if (Input.GetKeyUp(KeyCode.LeftControl))
            scrollSpeed /= scrollMultiplier;
    }

    private void OnSliderChange(float value)
    {
        float oldYPos = level.position.y;
        level.position = new Vector2(level.position.x, -value * (Grid.Instance.height / Grid.Instance.GetVisibleTimeRange()));

        Vector2 delta = new Vector2(0, level.position.y - oldYPos);
        SelectionBox.origin += delta;
        Brush.startSelectPos += delta;

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
