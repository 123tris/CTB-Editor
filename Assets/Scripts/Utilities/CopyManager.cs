using System.Collections.Generic;
using System.Linq;
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
        hitObjects = hitObjects.OrderBy(hitObject => hitObject.position.y).ToList(); //sort hitobject by timestamp

        foreach (HitObject hitObject in hitObjects)
        {
            HitObject copiedHitObject = Object.Instantiate(hitObject, GameManager.garbage.transform);
            copiedHitObject.gameObject.SetActive(false);
            copy.Add(copiedHitObject);
        }
    }

    public static void Paste()
    {
        Vector3 referencePoint = Grid.Instance.GetMousePositionOnGrid();
        Selection.Clear();

        for (int i = 0; i < copy.Count; i++)
        {
            HitObject createdHitObject = HitObjectManager.CreateFruit(Vector2.zero,level);
            Selection.Add(createdHitObject);

            if (i == 0)
            {
                createdHitObject.SetPosition(referencePoint); //Paste object where the mouse is held on the grid
            }
            else
            {
                Vector3 targetPos = referencePoint + (copy[i].transform.position - copy[0].transform.position);
                createdHitObject.SetPosition(targetPos);
            }

            createdHitObject.gameObject.SetActive(true);
        }

        List<GameObject> selectedGameObject = Selection.selectedHitObjects.Select(i => i.gameObject).ToList();
        RuntimeUndo.Undo.RegisterCreatedObjects(selectedGameObject);
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
