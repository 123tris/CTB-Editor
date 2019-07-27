using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

public class Selection
{
    public List<HitObject> selectedHitObjects = new List<HitObject>();

    private Vector3 dragDelta;
    private Vector2 startDragPos;

    public void Add(HitObject hitObject)
    {
        selectedHitObjects.Add(hitObject);
        hitObject.OnHightlight();
    }

    public void Remove(HitObject hitObject)
    {
        selectedHitObjects.Remove(hitObject);
        hitObject.UnHighlight();
    }

    public void Clear()
    {
        foreach (HitObject selectedHitObject in selectedHitObjects)
            selectedHitObject.UnHighlight();

        selectedHitObjects.Clear();
    }

    public HitObject first => selectedHitObjects.First();
    public HitObject last => selectedHitObjects.Last();

    public bool Contains(HitObject hitObject) => selectedHitObjects.Contains(hitObject);

    public HitObject GetFirstByTime() => selectedHitObjects.OrderBy(item => item.position.y).First();
    public HitObject GetLastByTime() => selectedHitObjects.OrderBy(item => item.position.y).Last();

    List<Vector3> startPositions = new List<Vector3>();

    public void UpdateDragging()
    {
        if (!Input.GetMouseButton(0)) return;

        if (Input.GetMouseButtonDown(0)) //On Start Dragging
        {
            startDragPos = Grid.Instance.GetSnappedMousePosition();
            startPositions = selectedHitObjects.Select(item => item.transform.position).ToList();
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

    private void DragFruit(Fruit fruit, int index)
    {
        //Update position of dragging fruit
        var targetPos = startPositions[index] - Grid.Instance.transform.position + dragDelta;
        fruit.SetXPosition(targetPos.x);
        fruit.SetPosition(targetPos);
    }

    private void DragSlider(Slider slider, int index)
    {
        //TODO: slider behaviour needs to be properly designed
        //slider.MoveSlider(Input.mousePosition + distanceFromSliderFruit);
    }

    public void SetSelected(HitObject hitObject)
    {
        Clear();
        selectedHitObjects.Add(hitObject);
        hitObject.OnHightlight();
    }

    public void DestroySelected()
    {
        foreach (HitObject selectedHitObject in selectedHitObjects)
        {
            Object.Destroy(selectedHitObject);
        }
        selectedHitObjects.Clear();
    }

    public void UpdateObjects()
    {
        for (int i = selectedHitObjects.Count - 1; i >= 0; i--)
        {
            if (selectedHitObjects[i] == null)
                selectedHitObjects.RemoveAt(i);
        }
    }
}
