using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI.Extensions;
using SliderUI = UnityEngine.UI.Slider;

public class TimeLine : MonoBehaviour
{
    /// <summary>
    /// This is the time in milliseconds which the timeline is currently set at
    /// </summary>
    public static int CurrentTimeStamp => Instance.currentTimeStamp;

    private int currentTimeStamp => (int)slider.value;

    public int scrollSpeed = 1;
    [SerializeField] private int scrollMultiplier = 5;

    private RectTransform level => GameManager.Instance.level;
    [SerializeField] private TextMeshProUGUI displayProgressPercentage;

    public static TimeLine Instance;

    private MusicPlayer player;
    private SliderUI slider;
    private Grid grid => Grid.Instance;

    private int startScrollSpeed;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        startScrollSpeed = scrollSpeed;
        player = FindObjectOfType<MusicPlayer>();
        slider = GetComponent<SliderUI>();
        slider.onValueChanged.AddListener(OnSliderChange);

        UpdateLevelPosition();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftAlt))
            scrollSpeed = startScrollSpeed * scrollMultiplier;
        else
            scrollSpeed = startScrollSpeed;

        float oldYPos = level.position.y;
        UpdateLevelPosition(currentTimeStamp);

        Vector2 delta = new Vector2(0, level.position.y - oldYPos);
        SelectionBox.origin += delta;
        Brush.startSelectPos += delta;

        displayProgressPercentage.text = (currentTimeStamp / (slider.maxValue / 100f)).ToString("F2") + "%";
    }

    public void UpdateLevelPosition(float timeStamp = 0)
    {
        //Update level position
        float targetY = -timeStamp * grid.msPerPixel + grid.GetOffset();
        level.anchoredPosition = new Vector2(level.anchoredPosition.x, targetY);
    }

    private void OnSliderChange(float value)
    {
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
}
