using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

public class Selection
{
    public List<HitObject> selectedHitObjects = new List<HitObject>();

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

    public HitObject GetFirstByTime() => selectedHitObjects.OrderBy(item => item.position.y).First();
    public HitObject GetLastByTime() => selectedHitObjects.OrderBy(item => item.position.y).Last();

    public void UpdateDragging()
    {
        if (!Input.GetMouseButton(0)) return;

        foreach (HitObject selectedHitObject in selectedHitObjects)
        {
            if (selectedHitObject is Fruit)
                DragFruit(selectedHitObject as Fruit);
            else
                DragSlider(selectedHitObject as Slider);
        }
    }

    private void DragFruit(Fruit fruit)
    {
        Vector2 mousePositionOnGrid = Grid.Instance.NearestPointOnGrid(Input.mousePosition);
        //Update position of dragging fruit
        fruit.SetXPosition(mousePositionOnGrid.x);
        fruit.SetPosition(mousePositionOnGrid);
    }

    private void DragSlider(Slider slider)
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
}
