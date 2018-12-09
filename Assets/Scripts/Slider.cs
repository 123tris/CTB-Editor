using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;

public class Slider : HitObject
{
    [HideInInspector] public GameObject fruitPrefab; //When brush creates a slider it is required to set the fruitprefab

    //Previous fruit should have a lower Y and next fruit should have a higher Y
    private List<Fruit> fruits = new List<Fruit>();
    private UILineRenderer lineRenderer;

    void Awake()
    {
        lineRenderer = GetComponent<UILineRenderer>();
    }

    public void AddFruit(Vector2 spawnPosition)
    {
        //TODO: Set position of slider when the first fruit is placed

        //Spawn fruit
        Fruit fruit = Instantiate(fruitPrefab,transform).GetComponent<Fruit>();
        fruit.SetPosition(spawnPosition);

        //Update slider's fruits
        fruits.Add(fruit);
        fruits.Sort((fruit1,fruit2) => fruit1.position.y.CompareTo(fruit2.position.y));

        UpdateLines();
    }

    public void UpdateLines()
    {
        lineRenderer.Points = new Vector2[fruits.Count];
        for (int i = 0; i < fruits.Count; i++)
        {
            lineRenderer.Points[i] = transform.InverseTransformPoint(fruits[i].transform.position);
        }

        lineRenderer.SetAllDirty();
    }

    public override void OnHightlight()
    {
        fruits.ForEach(f => f.OnHightlight());
    }

    public override void UnHighlight()
    {
        fruits.ForEach(f => f.UnHighlight());
    }

    public void MoveSlider(Vector2 pPosition)
    {
        SetPosition(pPosition);
        foreach (Fruit fruit in fruits)
            fruit.SetPosition(fruit.transform.position);
    }
}