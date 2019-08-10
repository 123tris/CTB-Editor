using System.Collections.Generic;
using UnityEngine;

public static class CopyManager
{
    private static List<HitObject> copy = new List<HitObject>();

    private static Transform level => GameManager.Instance.level;

    public static void Copy()
    {
        Copy(Selection.selectedHitObjects);
    }

    public static void Copy(List<HitObject> hitObjects)
    {
        Clear();
        hitObjects.Sort((x,y) => y.position.y.CompareTo(x.position.y)); //sort hitobject by timestamp

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

        Selection.Clear();

        for (int i = 0; i < copy.Count; i++)
        {
            HitObject createdHitObject;
            if (copy[i] is Fruit)
                createdHitObject = HitObjectManager.instance.CreateFruit(Vector2.zero, level);
            else
                createdHitObject = HitObjectManager.instance.CreateSlider(Vector2.zero, level);

            Selection.Add(createdHitObject);

            if (i == 0)
            {
                createdHitObject.SetPosition(referencePoint); //Paste object where the mouse is held on the grid
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
