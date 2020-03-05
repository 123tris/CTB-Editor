using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using RuntimeUndo;
using UnityEngine;
// ReSharper disable PossibleNullReferenceException

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

    public static Fruit GetFirstFruit()
    {
        if (first is Fruit) return (Fruit)first;
        return ((Slider)first).fruits.First();
    }

    public static Fruit GetLastFruit()
    {
        if (last is Fruit) return (Fruit)last;
        Slider slider = last as Slider;
        return slider.fruits.Last();
    }

    public static void UpdateDragging()
    {
        if (!Input.GetMouseButton(0)) return;

        if (Input.GetMouseButtonDown(0)) //On Start Dragging
        {
            foreach (HitObject selectedHitObject in selectedHitObjects)
            {
                if (selectedHitObject is Fruit)
                    Undo.RecordFruit(selectedHitObject as Fruit);
            }
            startDragPos = Grid.Instance.GetSnappedMousePosition();
            startPositions = selectedHitObjects.Select(item => Grid.Instance.NearestPointOnGrid(item.transform.position)).ToList(); //use grid to resnap when dragging
        }

        dragDelta = Grid.Instance.GetSnappedMousePosition() - startDragPos;

        for (int i = 0; i < selectedHitObjects.Count; i++)
        {
            HitObject selectedHitObject = selectedHitObjects[i];
            Vector3 targetPos = startPositions[i].ToVector3() - Grid.Instance.transform.position + dragDelta;

            if (targetPos.ToVector2() == startPositions[i]) return;

            selectedHitObject.SetPosition(targetPos);
        }
    }

    private static void DragFruit(Fruit fruit, int index)
    {
        //Update position of dragging fruit
        Vector3 targetPos = startPositions[index].ToVector3() - Grid.Instance.transform.position + dragDelta;

        if (targetPos.ToVector2() == startPositions[index]) return;

        fruit.SetXPosition(targetPos.x);
        fruit.SetPosition(targetPos);
    }

    private static void DragSlider(Slider slider, int index)
    {
        //TODO: slider behaviour needs to be properly designed

        //Update position of dragging fruit
        Vector3 targetPos = startPositions[index].ToVector3() - Grid.Instance.transform.position + dragDelta;

        if (targetPos.ToVector2() == startPositions[index]) return;

        slider.SetXPosition(targetPos.x);
        slider.SetPosition(targetPos);
    }

    public static void SetSelected(HitObject hitObject)
    {
        Clear();
        selectedHitObjects.Add(hitObject);
        hitObject.OnHightlight();
    }

    public static void SetSelected(List<Fruit> fruits)
    {
        Clear();
        foreach (Fruit fruit in fruits)
        {
            if (fruit.isSliderFruit)
            {
                if (!selectedHitObjects.Contains(fruit.slider))
                {
                    selectedHitObjects.Add(fruit.slider);
                    fruit.slider.OnHightlight();
                }
            }
            else
            {
                selectedHitObjects.Add(fruit);
                fruit.OnHightlight();
            }
        }
    }

    public static void DestroySelected()
    {
        //TODO: instead of this convoluted code the list of selected hitobjects should be split into slider and fruit in the first place
        List<Slider> sliders = new List<Slider>();
        List<Fruit> fruits = new List<Fruit>();
        foreach (HitObject selectedHitObject in selectedHitObjects)
        {
            var fruit = selectedHitObject as Fruit;
            if (fruit != null)
            {
                if (fruit.isSliderFruit)
                {
                    if (!sliders.Contains(fruit.slider))
                        sliders.Add(fruit.slider);
                }
                else
                    fruits.Add(fruit);
            }
            else if (!sliders.Contains(selectedHitObject))
            {
                sliders.Add((Slider)selectedHitObject);
            }
        }

        foreach (Fruit fruit in fruits)
        {
            fruit.UnHighlight();
            Undo.DestroyObject(fruit.gameObject);
        }

        foreach (Slider slider in sliders)
        {
            slider.UnHighlight();
            Undo.DestroyObject(slider.gameObject);
        }

        selectedHitObjects.Clear();
    }

    public static void SelectAll()
    {
        Clear();

        var fruits = HitObjectManager.GetNonSliderFruits();
        var sliders = HitObjectManager.GetSliders();

        fruits.ForEach(Add);
        sliders.ForEach(Add);
    }
}