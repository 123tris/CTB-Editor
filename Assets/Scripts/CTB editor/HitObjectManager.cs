using System;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Object = UnityEngine.Object;

public class HitObjectManager
{
    public static HitObjectManager instance;
    
    public const float DEFAULT_OSU_PLAYFIELD_WIDTH = 512f;
    public const float DEFAULT_OSU_PLAYFIELD_HEIGHT = 384f;

    public const int EDITOR_FIELD_WIDTH = 745; // will have to stop being hard-coded 

    /// <summary>
    /// How big the editor is compared to the playfield. 
    /// Should always be > 1.
    /// </summary>
    public const float WidthRatio = EDITOR_FIELD_WIDTH / DEFAULT_OSU_PLAYFIELD_WIDTH;

    public GameObject sliderPrefab;
    public GameObject fruitPrefab;

    public SortedDictionary<int, HitObject> hitObjects = new SortedDictionary<int, HitObject>(); //Key indicates when the hitobject is played in MS

    public HitObjectManager()
    {
        instance = this;
    }

    public Slider CreateSlider(Vector2 position, Transform parent)
    {
        Slider slider = Object.Instantiate(sliderPrefab, parent).GetComponent<Slider>();
        slider.SetPosition(position);
        AddHitObject(slider);
        #if UNITY_EDITOR
        Undo.RegisterCreatedObjectUndo(slider.gameObject,"Create Slider");
        #endif
        return slider;
    }

    /// <summary>
    /// Instantiates a fruit but instead of adding it to the managed hitobjects of the hitobjectmanager
    /// it will let the slider who owns the fruit to manage it instead. And the hitobjectmanager will manage the slider
    /// </summary>
    /// <param name="position">The global position of the fruit</param>
    /// <param name="slider">The slider which owns the fruit</param>
    public Fruit CreateSliderFruit(Vector2 position, Transform slider)
    {
        Fruit fruit = Object.Instantiate(fruitPrefab, slider).GetComponent<Fruit>();
        fruit.SetPosition(position);
        #if UNITY_EDITOR
        Undo.RegisterCreatedObjectUndo(fruit.gameObject,"Create Slider Fruit");
        #endif
        return fruit;
    }

    /// <summary> Instantiates a fruit and adds it to the managed hitobjects of the hitobjectmanager </summary>
    public Fruit CreateFruit(Vector2 position,Transform parent)
    {
        Fruit fruit = Object.Instantiate(fruitPrefab, parent).GetComponent<Fruit>();
        fruit.SetPosition(position);
        AddHitObject(fruit);
        RuntimeUndo.Undo.RegisterCreatedObject(fruit.gameObject);
        return fruit;
    }

    /// <summary>
    /// Adds a created hitobject to the hitobjectmanager's list of hitobjects that it manages over
    /// </summary>
    /// <param name="hitObject"></param>
    public void AddHitObject(HitObject hitObject)
    {
        hitObjects[hitObject.position.y] = hitObject;
    }

    public void RemoveHitObject(int yAxis)
    {
        hitObjects.Remove(yAxis);
    }

    public HitObject GetHitObjectByYAxis(int yAxis)
    {
        return hitObjects[yAxis];
    }

    public bool ContainsFruit(int timeStamp)
    {
        bool output = hitObjects.ContainsKey(timeStamp);
        return output;
    }

    public void UpdateAllCircleSize()
    {
        foreach (HitObject h in hitObjects.Values)
        {
            h.UpdateCircleSize();
        }
    }

    public HitObject GetPreviousHitObject(HitObject hitObject)
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

    public HitObject GetNextHitObject(HitObject hitObject)
    {
        int index = 0;
        foreach (KeyValuePair<int, HitObject> keyValuePair in hitObjects)
        {
            if (keyValuePair.Key == hitObject.position.y)
            {
                if (index == hitObjects.Count - 1) return null;
                return hitObjects.ElementAt(index + 1).Value;
            }
            index++;
        }
        throw new Exception("Couldn't find hitobject inside of hitObjects!");
    }
}
