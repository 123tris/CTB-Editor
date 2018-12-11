using System;
using System.Collections.Generic;
using UnityEngine;

public class HitObjectManager
{
    public static HitObjectManager instance;

    private const float DefaultOsuPlayfieldWidth = 512f;
    private const int EditorFieldWith = 745; // will have to stop being hardcodded 

    public const float OBJECT_RADIUS = 44;

    public float Scale
    {
        get {  return (1.0f - 0.7f * (TextUI.Instance.CS - 5) / 5f) * EditorFieldWith / DefaultOsuPlayfieldWidth; }
        private set { }
    }

    public HitObjectManager()
    {
        instance = this;
    }

    public Dictionary<int, HitObject> hitObjects = new Dictionary<int, HitObject>(); //Key indicates the y-axis of the hitobject

    public void AddHitObject(HitObject hitObject)
    {
        hitObjects.Add(hitObject.position.y, hitObject);
    }

    public void RemoveHitObject(int yAxis)
    {
        hitObjects.Remove(yAxis);
    }

    public HitObject GetHitObjectByYAxis(int yAxis)
    {
        return hitObjects[yAxis];
    }

    public bool ContainsFruit(int timeStamp)
    {
        return hitObjects.ContainsKey(timeStamp);
    }

    public void UpdateAllCircleSize()
    {
        foreach (HitObject h in hitObjects.Values)
        {
            UpdateFruitCircleSize(h);
        }

    }

    public void UpdateFruitCircleSize(HitObject h)
    {
        float circleSizePX = OBJECT_RADIUS * Scale;
        RectTransform r = h.GetComponent<RectTransform>();
        r.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, circleSizePX);
        r.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, circleSizePX);
    }
}
