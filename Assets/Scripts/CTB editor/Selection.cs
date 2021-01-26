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

    public static bool hasSelection => selectedHitObjects.Count > 0;

    [RuntimeInitializeOnLoadMethod]
    static void Init() => selectedHitObjects.Clear();

    public static void Add(HitObject hitObject)
    {
        SoundManager.PlayHitObjectClickSound();
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
            Undo.RecordHitObjects(selectedHitObjects); //Record undo snapshot before dragging

            startDragPos = Grid.Instance.GetSnappedMousePosition();
            startPositions = selectedHitObjects.Select(item => Grid.Instance.NearestPointOnGrid(item.transform.position)).ToList(); //use grid to resnap when dragging
        }

        dragDelta = Grid.Instance.GetSnappedMousePosition() - startDragPos;

        for (int i = 0; i < selectedHitObjects.Count; i++)
        {
            HitObject selectedHitObject = selectedHitObjects[i];

            if (dragDelta.x == 0 && dragDelta.y == 0) return;

            Vector3 targetPos = startPositions[i].ToVector3() - Grid.Instance.transform.position + dragDelta;

            selectedHitObject.SetPosition(targetPos);
        }
    }

    public static void SetSelected(HitObject hitObject)
    {
        Clear();
        SoundManager.PlayHitObjectClickSound();
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
        List<GameObject> sliders = new List<GameObject>();
        List<GameObject> fruits = new List<GameObject>();

        foreach (HitObject selectedHitObject in selectedHitObjects)
        {
            var fruit = selectedHitObject as Fruit;
            if (fruit != null)
            {
                if (fruit.isSliderFruit)
                {
                    if (!sliders.Contains(fruit.slider.gameObject))
                        sliders.Add(fruit.slider.gameObject);
                }
                else fruits.Add(fruit.gameObject);
            }
            else if (!sliders.Contains(selectedHitObject.gameObject))
            {
                sliders.Add(selectedHitObject.gameObject);
            }
        }

        List<GameObject> destroyObjects = fruits.Concat(sliders).ToList();
        Undo.DestroyObjects(destroyObjects);
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