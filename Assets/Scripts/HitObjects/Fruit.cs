﻿using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class Fruit : HitObject {
    public const float OBJECT_RADIUS = 44;

    public float Scale => (1.0f - 0.7f * (BeatmapSettings.CS - 5) / 5f) * Grid.WidthRatio;

    public bool isSliderFruit => slider;
    public Slider slider => transform.parent.GetComponent<Slider>();

    private NicerOutline outline;

    private Image image;

    public Fruit()
    {
        type = HitObjectType.Fruit;
    }

    private void Awake()
    {
        outline = GetComponent<NicerOutline>();
        image = GetComponent<Image>();
    }

    void Update()
    {
        UpdateHyperDashState();
        image.color = hyperDash ? Color.red.OverrideAlpha(0.5f) : Color.white;
    }

    public override void UpdateCircleSize()
    {
        float circleSizePX = OBJECT_RADIUS * Scale;
        RectTransform r = GetComponent<RectTransform>();
        r.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, circleSizePX);
        r.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, circleSizePX);
    }

    public override void OnHightlight()
    {
        GetComponent<NicerOutline>().enabled = true;
    }

    public override void UnHighlight()
    {
        GetComponent<NicerOutline>().enabled = false;
    }

    void UpdateHyperDashState()
    {
        HitObject nextHitObject = HitObjectManager.GetNextHitObject(this);
        if (isSliderFruit)
        {
            int fruitIndex = slider.GetFruitIndex(this);
            if (fruitIndex < slider.fruitCount - 1)
                nextHitObject = slider.GetFruitByIndex(++fruitIndex);
        }

        if (nextHitObject != null)
        {
            //Check if next fruit is catchable
            double halfCatcherWidth = Catcher.GetCatcherSize() / 2;
            double timeToNext = nextHitObject.position.y - position.y - 1000f / 60f / 4; // 1/4th of a frame of grace time, taken from osu
            double distanceToNext = Math.Abs(nextHitObject.position.x - position.x) - halfCatcherWidth * 512;
            float distanceToHyper = (float)(timeToNext * Catcher.BASE_SPEED - distanceToNext);

            if (distanceToHyper < 0)
            {
                hyperDashTarget = nextHitObject;
                return;
            }
        }

        hyperDashTarget = null;
    }
}
