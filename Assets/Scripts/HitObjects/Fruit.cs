using System;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using static CooldownManagerNamespace.CooldownManager;

public class Fruit : HitObject
{
    public const float OBJECT_RADIUS = 44;

    public float Scale => (1.0f - 0.7f * (BeatmapSettings.CS - 5) / 5f) * Grid.GetHeightRatio();

    public Slider slider => transform.parent.GetComponent<Slider>();

    private NicerOutline outline;

    private SVGImage image;

    private Brush brush;

    private Fruit fruitPreview => brush.fruitDisplay;

    private RectTransform rect;

    private double oldTimeStamp;

    public Fruit()
    {
        type = HitObjectType.Fruit;
    }

    private void Awake()
    {
        outline = GetComponent<NicerOutline>();
        image = GetComponent<SVGImage>();
        brush = FindObjectOfType<Brush>();
        rect = GetComponent<RectTransform>();
    }

    public void OnUpdate()
    {
        double currenTime = MusicPlayer.instance.currentTime * 1000;

        if (MusicPlayer.instance.isPlaying && oldTimeStamp <= position.y &&
            currenTime > position.y)
        {
            //image.color = Color.red; //TODO: Remove debug code
            //Cooldown(0.001f, () => image.color = Color.white);
        }

        oldTimeStamp = currenTime; //add schedule delay to trigger sound effect ahead of time

        //Is the fruit in the current view of the user?
        float posY = transform.position.y;
        if (posY > Grid.Instance.height * Grid.Instance.zoom * 1.5f || posY < 0)
            return;

        UpdateHyperDashState();
        UpdateCircleSize();
        image.color = hyperDash ? Color.red : Color.white;
    }

    /// <summary> SetPosition requires a local position from the grid's perspective
    /// <para> If the position is already occupied it will throw an error</para></summary>
    public override void SetPosition(Vector3 newPosition)
    {
        //TODO: For now placing a fruit on the same beatmap is allowed for simplicity sake. This can possibly be improved on in the future
        float timeStamp = Grid.Instance.GetHitTime(newPosition);

        HitObjectManager.EditFruitTimeStamp(this, Mathf.RoundToInt(timeStamp)); //Essentially setting the y position
        SetXPosition(newPosition.x);

        transform.position = newPosition + Grid.Instance.transform.position; //Apply grid's position to set global position
    }

    private void UpdateCircleSize()
    {
        float circleSizePX = OBJECT_RADIUS * Scale;
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, circleSizePX);
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, circleSizePX * Grid.Instance.zoom);
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
        HitObject nextHitObject = HitObjectManager.GetNextFruitByTimeStamp(position.y);

        bool showPreview = fruitPreview.isActiveAndEnabled && fruitPreview.position.y > position.y;

        if (nextHitObject == null)
        {
            if (showPreview)
                nextHitObject = fruitPreview;
            else
            {
                hyperDashTarget = null;
                return;
            }
        }
        else
        {
            //prioritize preview over next object in the map
            if (fruitPreview.position.y < nextHitObject.position.y && showPreview)
                nextHitObject = fruitPreview;
        }

        //Check if next fruit is catchable
        double halfCatcherWidth = Catcher.GetCatcherSize() / 2;
        double timeToNext = nextHitObject.position.y - position.y - 1000f / 60f / 4; // 1/4th of a frame of grace time, taken from osu
        double distanceToNext = Math.Abs(nextHitObject.position.x - position.x) - halfCatcherWidth * Grid.DEFAULT_OSU_PLAYFIELD_WIDTH;
        float distanceToHyper = (float)(timeToNext * Catcher.BASE_SPEED - distanceToNext);

        if (distanceToHyper < 0)
        {
            hyperDashTarget = nextHitObject;
            return;
        }

        hyperDashTarget = null;
    }
}
