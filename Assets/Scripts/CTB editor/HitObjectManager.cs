using System;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Object = UnityEngine.Object;

public static class HitObjectManager
{
    public static GameObject sliderPrefab;
    public static GameObject fruitPrefab;

    private static Grid grid => Grid.Instance;

    private static List<Fruit> fruits = new List<Fruit>();
    private static List<Slider> sliders = new List<Slider>();

    /// <summary> Instantiates a fruit and adds it to the managed hitobjects of the hitobjectmanager </summary>
    public static Fruit CreateFruit(Vector2 position,Transform parent)
    {
        Fruit fruit = Object.Instantiate(fruitPrefab, parent).GetComponent<Fruit>();
        fruit.SetPosition(position);
        AddFruit(fruit);
        return fruit;
    }

    public static void CreateFruitByData(Vector2 position)
    {
        Fruit fruit = Object.Instantiate(fruitPrefab, GameManager.Instance.level).GetComponent<Fruit>();
        fruit.position = position.ToVector2Int();

        float xPos = position.x * Grid.WidthRatio + grid.transform.position.x;
        float yPos = grid.GetYPosition(position.y);
        fruit.transform.position = new Vector3(xPos,yPos);

        AddFruit(fruit);
        RuntimeUndo.Undo.RegisterCreatedObject(fruit.gameObject);
    }

    public static Slider CreateSlider(Vector2 position, Transform parent)
    {
        Slider slider = Object.Instantiate(sliderPrefab, parent).GetComponent<Slider>();
        slider.SetPosition(position);
        slider.AddFruit(position);
        AddSlider(slider);

        Debug.Log(slider.fruits.First().transform.localPosition);
        RuntimeUndo.Undo.RegisterCreatedObject(slider.gameObject);
        return slider;
    }

    public static void CreateSliderByData(float positionX, int startTime, List<System.Numerics.Vector2> sliderPoints, int repeats)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Adds a created hitobject to the hitobjectmanager's list of hitobjects that it manages over
    /// Use HitObjectManager.CreateFruit whenever possible
    /// </summary>
    /// <param name="fruit"></param>
    public static void AddFruit(Fruit fruit)
    {
        fruits.Add(fruit);
        fruits.Sort();
    }

    public static void AddSlider(Slider slider)
    {
        sliders.Add(slider);
        sliders.Sort();
    }

    public static void EditFruitTimeStamp(Fruit fruit,int timeStamp)
    {
        fruit.position.y = timeStamp;
        fruits.Sort();
    }

    public static HitObject GetHitObjectByTime(int timeStamp)
    {
        var fruitsAtTimeStamp = fruits.Where(fruit => fruit.position.y == timeStamp).ToList();
        if (fruitsAtTimeStamp.Count == 0)
            return null;
        else
            return fruitsAtTimeStamp.First();
    }

    public static bool ContainsFruit(int timeStamp)
    {
        return fruits.Any(fruit => fruit.position.y == timeStamp);
    }

    public static void UpdateAllCircleSize()
    {
        foreach (Fruit h in fruits)
        {
            h.UpdateCircleSize();
        }
    }

    public static Fruit GetPreviousFruit(Fruit fruit)
    {
        if (fruits.IndexOf(fruit) == 0)
            return null;

        return fruits[fruits.IndexOf(fruit) - 1];
    }

    public static HitObject GetPreviousFruitByTimeStamp(int timestamp)
    {
        foreach (Fruit fruit in fruits)
        {
            if (timestamp > fruit.position.y)
                return fruit;
        }
        return null;
    }

    public static HitObject GetNextFruit(Fruit fruit)
    {
        if (fruits.IndexOf(fruit) == fruits.Count-1)
            return null;

        return fruits[fruits.IndexOf(fruit) + 1];
    }

    public static HitObject GetNextFruitByTimeStamp(int timestamp)
    {
        foreach (Fruit fruit in fruits)
        {
            if (timestamp < fruit.position.y)
                return fruit;
        }
        return null;
    }

    /// <summary>Returns fruits that are both authored by a slider or independent fruits. If you want non slider fruits uses GetNonSliderFruits()</summary>
    public static List<Fruit> GetFruits() => fruits;

    public static List<Fruit> GetNonSliderFruits()
    {
        return fruits.Where(i => !i.isSliderFruit).ToList();
    }

    public static List<Slider> GetSliders() => sliders;

    public static void Reset()
    {
        sliders.ForEach(Object.Destroy);
        fruits.ForEach(Object.Destroy);
    }

    public static void RemoveFruit(Fruit hitObject) => fruits.Remove(hitObject);

    public static void RemoveSlider(Slider slider) => sliders.Remove(slider);

    public static List<Fruit> GetFruitsByRange(float startTime, float endTime)
    {
        List<Fruit> output = new List<Fruit>();
        foreach (Fruit fruit in fruits)
        {
            if (endTime < fruit.position.y) break;

            if (startTime <= fruit.position.y)
            {
                output.Add(fruit);
            }
        }
        return output;
    }
}
