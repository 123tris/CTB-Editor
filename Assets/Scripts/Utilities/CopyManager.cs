using System.Collections.Generic;
using UnityEngine;

public static class CopyManager
{
    private static List<HitObject> copy = new List<HitObject>();

    public static void Copy(List<HitObject> hitObjects)
    {
        foreach (HitObject hitObject in hitObjects)
        {
            HitObject copiedHitObject = Object.Instantiate(hitObject,GameManager.garbage.transform);
            copiedHitObject.gameObject.SetActive(false);
            copy.Add(copiedHitObject);
        }
    }

    public static void Paste()
    {
        foreach (HitObject hitObject in copy)
        {
            
        }
    }

    public static void Clear()
    {
        foreach (HitObject hitObject in copy)
        {
            Object.Destroy(hitObject);
        }
        copy.Clear();
    }
}
