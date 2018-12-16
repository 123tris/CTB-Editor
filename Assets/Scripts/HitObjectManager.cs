using System;
using System.Collections.Generic;
using UnityEngine;

public class HitObjectManager
{
    public static HitObjectManager instance;

    public const float DefaultOsuPlayfieldWidth = 512f;
    public const int EditorFieldWith = 745; // will have to stop being hardcodded 

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

    public Fruit CreateSliderFruit(Vector2 position, Transform parent)
    {
        Fruit fruit = UnityEngine.Object.Instantiate(fruitPrefab, parent).GetComponent<Fruit>();
        fruit.SetPosition(position);
        return fruit;
    }

    public Fruit CreateFruit(Vector2 position,Transform parent)
    {
        Fruit fruit = UnityEngine.Object.Instantiate(fruitPrefab, parent).GetComponent<Fruit>();
        fruit.SetPosition(position);
        AddHitObject(fruit);
        return fruit;
    }

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
