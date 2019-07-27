using System.Collections.Generic;
using UnityEngine;

public static class CopyManager
{
    private static List<HitObject> copy = new List<HitObject>();

    private static Transform level => GameManager.Instance.level;

    public static void Copy(List<HitObject> hitObjects)
    {
        Clear();
        hitObjects.Sort((x,y) => x.position.y.CompareTo(y.position.y)); //sort hitobject by timestamp

        foreach (HitObject hitObject in hitObjects)
        {
            HitObject copiedHitObject = Object.Instantiate(hitObject,GameManager.garbage.transform);
            copiedHitObject.gameObject.SetActive(false);
            copy.Add(copiedHitObject);
        }
    }

    public static void Paste()
    {
        Vector3 referencePoint = Grid.Instance.GetMousePositionOnGrid();
        HitObject firstObject = copy[0];
        for (int i = 0; i < copy.Count; i++)
        {
            HitObject createdHitObject = Object.Instantiate(copy[i], level);
            if (i == 0)
            {
                createdHitObject.SetPosition(referencePoint);
            }
            else
            {
                Vector3 targetPos = referencePoint + (firstObject.transform.position - copy[i].transform.position);
                createdHitObject.SetPosition(targetPos);
            }
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
