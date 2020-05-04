﻿using System;
using System.Collections.Generic;
using System.Linq;
using OsuParsers.Beatmaps.Objects.Catch;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Object = UnityEngine.Object;
using PSlider = OsuParsers.Beatmaps.Objects.Slider;

public static class HitObjectManager
{
    public static GameObject sliderPrefab;
    public static GameObject fruitPrefab;

    private static Grid grid => Grid.Instance;

    private static List<Fruit> fruits = new List<Fruit>();
    private static List<Slider> sliders = new List<Slider>();

    #region DomainReloadingSupport
    [RuntimeInitializeOnLoadMethod]
    static void Init() //Static references need to be cleared if the domain is not reloaded
    {
        fruits.Clear();
        sliders.Clear();
    }
    #endregion

    /// <summary> Instantiates a fruit and adds it to the managed hitobjects of the hitobjectmanager </summary>
    public static Fruit CreateFruit(Vector2 position, Transform parent)
    {
        Fruit fruit = Object.Instantiate(fruitPrefab, parent).GetComponent<Fruit>();
        fruit.SetPosition(position);
        AddFruit(fruit);
        return fruit;
    }

    public static void CreateFruitByParser(CatchFruit hitObject)
    {
        Vector2 position = new Vector2(hitObject.Position.X, hitObject.StartTime);

        Fruit fruit = Object.Instantiate(fruitPrefab, GameManager.Instance.level).GetComponent<Fruit>();
        fruit.position = position.ToVector2Int();

        float xPos = position.x * Grid.GetWidthRatio();
        float yPos = grid.GetYPosition(position.y);
        fruit.transform.position = new Vector3(xPos, yPos) + grid.transform.position;

        AddFruit(fruit);
    }

    public static void CreateSliderByParser(CatchJuiceStream pSlider)
    {
        //TODO: implement repeat sliders & remove hardcode
        if (pSlider.Repeats > 1) return;
        int startTime = pSlider.StartTime;
        int endTime = pSlider.EndTime;

        List<System.Numerics.Vector2> points = pSlider.SliderPoints;

        float xPos = pSlider.Position.X * Grid.GetWidthRatio();
        float yPos = grid.GetYPosition(startTime);

        Vector2 position = new Vector2(xPos, yPos);

        Brush brush = GameManager.Instance.brush;
        Slider slider = CreateSlider(position, brush.transform);

        Vector2 sliderDirection = (points[0] - pSlider.Position).ToUnityVector().normalized;

        xPos += (float)(pSlider.PixelLength * sliderDirection.x * Grid.GetWidthRatio());
        yPos = grid.GetYPosition(endTime);
        position = new Vector2(xPos, yPos);

        slider.AddFruit(position);
    }

    public static Slider CreateSlider(Vector2 position, Transform parent)
    {
        Slider slider = Object.Instantiate(sliderPrefab, parent).GetComponent<Slider>();
        slider.SetPosition(position);
        slider.AddFruit(position);
        AddSlider(slider);

        Debug.Log(slider.fruits.First().transform.localPosition);
        return slider;
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

    public static void EditFruitTimeStamp(Fruit fruit, int timeStamp)
    {
        fruit.position.y = timeStamp;
        fruits.Sort();
    }

    public static HitObject GetHitObjectByTime(int timeStamp)
    {
        List<Fruit> fruitsAtTimeStamp = fruits.Where(fruit => fruit.position.y == timeStamp).ToList();
        if (fruitsAtTimeStamp.Count == 0)
            return null;
        else
            return fruitsAtTimeStamp.First();
    }

    public static bool ContainsFruit(int timeStamp)
    {
        return fruits.Any(fruit => fruit.position.y == timeStamp);
    }

    public static Fruit GetPreviousFruit(Fruit fruit)
    {
        if (fruit == null) return null; //Fix code that references this function that forces this check

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
        if (fruit == null) return null; //Fix code that references this function that forces this check

        if (fruits.IndexOf(fruit) == fruits.Count - 1)
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

    /// <summary>Returns all fruits that could be either a slider fruit or independent fruit. If you want non slider fruits uses GetNonSliderFruits()</summary>
    public static List<Fruit> GetFruits() => fruits;

    public static List<Fruit> GetNonSliderFruits()
    {
        return fruits.Where(i => !i.isSliderFruit).ToList();
    }

    public static List<Slider> GetSliders() => sliders;

    /// <summary>Updates the active states of only fruit that are inside of the graph to save performance</summary>
    public static void UpdateActiveStates()
    {
        throw new NotImplementedException(); //TODO: Finish implementation for performance boost

        float maxY = Grid.Instance.height * Grid.Instance.zoom * 1.5f;
        float minY = 0;

        foreach (Fruit fruit in fruits)
        {
            if (fruit.isSliderFruit == false)
            {
                Vector3 fruitPosition = fruit.transform.position;
                bool active = fruitPosition.y > maxY || fruitPosition.y < minY;
                fruit.gameObject.SetActive(!active);
            }
            else //TODO: Change to foreach loop
            {
                Vector3 fruitPosition1 = fruit.slider.fruits[0].transform.position;
                Vector3 fruitPosition2 = fruit.slider.fruits[1].transform.position;

                bool activeFruit1 = fruitPosition1.y > maxY || fruitPosition1.y < minY;
                bool activeFruit2 = fruitPosition2.y > maxY || fruitPosition2.y < minY;

                fruit.slider.gameObject.SetActive(!activeFruit1 && !activeFruit2);
            }
        }
    }

    public static void Reset()
    {
        sliders.ForEach(slider => Object.Destroy(slider.gameObject));
        fruits.ForEach(fruit => Object.Destroy(fruit.gameObject));
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

    public static void UpdateFruitVisuals()
    {
        foreach (Fruit fruit in fruits)
        {
            if (fruit.gameObject.activeInHierarchy)
                fruit.UpdateVisuals();
        }
    }
}
