using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Selection
{
    public static List<HitObject> selectedHitObjects = new List<HitObject>();

    private static Vector3 dragDelta;
    private static Vector2 startDragPos;

    private static List<Vector2> startPositions = new List<Vector2>();

    public static HitObject first => selectedHitObjects.First();
    public static HitObject last => selectedHitObjects.Last();

    public static void Add(HitObject hitObject)
    {
        selectedHitObjects.Add(hitObject);
        hitObject.OnHightlight();
    }

    public static void Remove(HitObject hitObject)
    {
        selectedHitObjects.Remove(hitObject);
        hitObject.UnHighlight();
    }

    public static void Clear()
    {
        foreach (HitObject selectedHitObject in selectedHitObjects)
            selectedHitObject.UnHighlight();

        selectedHitObjects.Clear();
    }

    public static bool Contains(HitObject hitObject)
    {
        return selectedHitObjects.Contains(hitObject);
    }

    public static HitObject GetFirstByTime()
    {
        return selectedHitObjects.OrderBy(item => item.position.y).First();
    }

    public static HitObject GetLastByTime()
    {
        return selectedHitObjects.OrderBy(item => item.position.y).Last();
    }

    public static void UpdateDragging()
    {
        if (!Input.GetMouseButton(0)) return;

        if (Input.GetMouseButtonDown(0)) //On Start Dragging
        {
            startDragPos = Grid.Instance.GetSnappedMousePosition();
            startPositions = selectedHitObjects.Select(item => Grid.Instance.NearestPointOnGrid(item.transform.position)).ToList(); //use grid to resnap when dragging
        }

        dragDelta = Grid.Instance.GetSnappedMousePosition() - startDragPos;

        for (int i = 0; i < selectedHitObjects.Count; i++)
        {
            HitObject selectedHitObject = selectedHitObjects[i];
            if (selectedHitObject is Fruit)
                DragFruit(selectedHitObject as Fruit, i);
            else
                DragSlider(selectedHitObject as Slider, i);
        }
    }

    private static void DragFruit(Fruit fruit, int index)
    {
        //Update position of dragging fruit
        Vector3 targetPos = startPositions[index].ToVector3() - Grid.Instance.transform.position + dragDelta;
        fruit.SetXPosition(targetPos.x);
        fruit.SetPosition(targetPos);
    }

    private static void DragSlider(Slider slider, int index)
    {
        //TODO: slider behaviour needs to be properly designed
        //slider.MoveSlider(Input.mousePosition + distanceFromSliderFruit);
    }

    public static void SetSelected(HitObject hitObject)
    {
        Clear();
        selectedHitObjects.Add(hitObject);
        hitObject.OnHightlight();
    }

    public static void SetSelected(List<HitObject> hitObjects)
    {
        Clear();
        foreach (HitObject hitObject in hitObjects)
        {
            selectedHitObjects.Add(hitObject);
            hitObject.OnHightlight();
        }
    }

    public static void DestroySelected()
    {
        foreach (HitObject selectedHitObject in selectedHitObjects)
        {
            RuntimeUndo.Undo.DestroyObject(selectedHitObject.gameObject);
            Object.Destroy(selectedHitObject.gameObject);
        }
    }
}