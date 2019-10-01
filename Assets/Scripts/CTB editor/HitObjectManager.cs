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

    private static SortedDictionary<int, HitObject> hitObjects = new SortedDictionary<int, HitObject>(); //Key indicates when the hitobject is played in MS

    public static Slider CreateSlider(Vector2 position, Transform parent)
    {
        Slider slider = Object.Instantiate(sliderPrefab, parent).GetComponent<Slider>();
        slider.Init(position);
        slider.AddFruit(position);
        RuntimeUndo.Undo.RegisterCreatedObject(slider.gameObject);
        return slider;
    }

    /// <summary> Instantiates a fruit and adds it to the managed hitobjects of the hitobjectmanager </summary>
    public static Fruit CreateFruit(Vector2 position,Transform parent)
    {
        Fruit fruit = Object.Instantiate(fruitPrefab, parent).GetComponent<Fruit>();
        fruit.Init(position);
        RuntimeUndo.Undo.RegisterCreatedObject(fruit.gameObject);
        return fruit;
    }

    public static void CreateFruitByData(Vector2 position)
    {
        Fruit fruit = Object.Instantiate(fruitPrefab, GameManager.Instance.level).GetComponent<Fruit>();
        fruit.position = position.ToVector2Int();

        float xPos = position.x * Grid.WidthRatio + grid.transform.position.x;
        float yPos = grid.GetYPosition(position.y);
        fruit.transform.position = new Vector3(xPos,yPos);

        AddHitObject(fruit);
        fruit.initialized = true;

        RuntimeUndo.Undo.RegisterCreatedObject(fruit.gameObject);
    }

    /// <summary>
    /// Adds a created hitobject to the hitobjectmanager's list of hitobjects that it manages over
    /// </summary>
    /// <param name="hitObject"></param>
    public static void AddHitObject(HitObject hitObject)
    {
        hitObjects[hitObject.position.y] = hitObject;
    }

    public static void EditHitObjectTimeStamp(HitObject hitObject,int timeStamp)
    {
        hitObjects.Remove(hitObject.position.y);
        hitObjects[timeStamp] = hitObject;
    }

    public static void RemoveHitObject(int yAxis)
    {
        if (hitObjects.ContainsKey(yAxis))
            hitObjects.Remove(yAxis);
    }

    public static HitObject GetHitObjectByTime(int timeStamp)
    {
        return hitObjects[timeStamp];
    }

    public static bool ContainsFruit(int timeStamp)
    {
        bool output = hitObjects.ContainsKey(timeStamp);
        return output;
    }

    public static void UpdateAllCircleSize()
    {
        foreach (HitObject h in hitObjects.Values)
        {
            h.UpdateCircleSize();
        }
    }

    public static HitObject GetPreviousHitObject(HitObject hitObject)
    {
        int index = 0;
        foreach (KeyValuePair<int, HitObject> keyValuePair in hitObjects)
        {
            if (keyValuePair.Key == hitObject.position.y)
            {
                if (index == 0) return null;
                return hitObjects.ElementAt(index - 1).Value;
            }
            index++;
        }
        throw new Exception("Couldn't find hitobject inside of hitObjects!");
    }

    public static HitObject GetNextHitObject(HitObject hitObject)
    {
        return GetNextHitObject(hitObject.position.y);
    }

    private static HitObject GetNextHitObject(int timestamp)
    {
        foreach (KeyValuePair<int, HitObject> keyValuePair in hitObjects)
        {
            if (keyValuePair.Key > timestamp)
                return keyValuePair.Value;
        }
        return null;
    }

    public static List<HitObject> GetHitObjects()
    {
        return hitObjects.Values.ToList();
    }

    public static void Reset()
    {
        foreach (KeyValuePair<int, HitObject> hitObject in hitObjects)
        {
            Object.Destroy(hitObject.Value.gameObject);
        }
    }
}
