using System;
using System.Collections.Generic;
using UnityEngine;

public class HitObjectManager
{
    public static HitObjectManager instance;
    
    public const float DEFAULT_OSU_PLAYFIELD_WIDTH = 512f;
    public const float DEFAULT_OSU_PLAYFIELD_HEIGHT = 384f;

    public const int EDITOR_FIELD_WIDTH = 745; // will have to stop being hardcodded 

    /// <summary>
    /// How big the editor is compared to the playfield. 
    /// Should always be > 1.
    /// </summary>
    public const float WidthRatio = EDITOR_FIELD_WIDTH / DEFAULT_OSU_PLAYFIELD_WIDTH;

    public GameObject sliderPrefab;
    public GameObject fruitPrefab;

    public HitObjectManager()
    {
        instance = this;
    }

    public Dictionary<int, HitObject> hitObjects = new Dictionary<int, HitObject>(); //Key indicates the y-axis of the hitobject

    public Slider CreateSlider(Vector2 position, Transform parent)
    {
        Slider slider = UnityEngine.Object.Instantiate(sliderPrefab, parent).GetComponent<Slider>();
        slider.SetPosition(position);
        AddHitObject(slider);
        return slider;
    }

    /// <summary>
    /// Instantiates a fruit but instead of adding it to the managed hitobjects of the hitobjectmanager
    /// it will let the slider who owns the fruit to manage it instead. And the hitobjectmanager will manage the slider
    /// </summary>
    /// <param name="position">The global position of the fruit</param>
    /// <param name="slider">The slider which owns the fruit</param>
    /// <returns></returns>
    public Fruit CreateSliderFruit(Vector2 position, Transform slider)
    {
        Fruit fruit = UnityEngine.Object.Instantiate(fruitPrefab, slider).GetComponent<Fruit>();
        fruit.SetPosition(position);
        return fruit;
    }

    /// <summary>
    /// Instantiates a fruit and adds it to the managed hitobjects of the hitobjectmanager
    /// </summary>
    /// <param name="position"></param>
    /// <param name="parent"></param>
    /// <returns></returns>
    public Fruit CreateFruit(Vector2 position,Transform parent)
    {
        Fruit fruit = UnityEngine.Object.Instantiate(fruitPrefab, parent).GetComponent<Fruit>();
        fruit.SetPosition(position);
        AddHitObject(fruit);
        return fruit;
    }

    /// <summary>
    /// Adds a created hitobject to the hitobjectmanager's list of hitobjects that it manages over
    /// </summary>
    /// <param name="hitObject"></param>
    private void AddHitObject(HitObject hitObject)
    {
        hitObjects.Add(hitObject.position.y, hitObject);
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
        return hitObjects.ContainsKey(timeStamp);
    }

    public void UpdateAllCircleSize()
    {
        foreach (HitObject h in hitObjects.Values)
        {
            h.UpdateCircleSize();
        }
    }
}
