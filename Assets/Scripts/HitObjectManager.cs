using System;
using System.Collections.Generic;

public class HitObjectManager
{
    public static HitObjectManager instance;

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
}
