using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI.Extensions;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;
using NVector2 = System.Numerics.Vector2;

public class Slider : HitObject
{
    private GameObject fruitPrefab => GameManager.Instance.fruitPrefab;
    private GameObject handlePrefab => GameManager.Instance.handlePrefab;

    public int startTime => fruits[0].position.y;
    public int endTime => fruits[fruits.Count-1].position.y;

    //Previous fruit should have a lower Y and next fruit should have a higher Y
    public List<Fruit> fruits = new List<Fruit>();
    private List<Transform> handles = new List<Transform>();
    private UILineRenderer lineRenderer;
    public int fruitCount => fruits.Count;

    public Slider()
    {
        type = HitObjectType.Slider;
    }

    void Awake()
    {
        lineRenderer = GetComponent<UILineRenderer>();
    }

    public void AddFruit(Vector2 spawnPosition)
    {
        //Spawn fruit
        Fruit fruit = HitObjectManager.CreateFruit(spawnPosition, transform);

        //Update slider's fruits
        fruits.Add(fruit);
        fruits.Sort((fruit1,fruit2) => fruit1.position.y.CompareTo(fruit2.position.y));

        UpdateLines();
    }

    public void AddHandle(Vector3 mousePositionOnGrid)
    {
        Transform handle = Instantiate(handlePrefab,transform).transform;
        handles.Add(handle);
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

    /// <summary> SetPosition requires a local position from the grid's perspective
    /// <para> If the position is already occupied it will throw an error</para></summary>
    public override void SetPosition(Vector3 newPosition)
    {
        int timeStamp = (int)Grid.Instance.GetHitTime(newPosition);
        position.y = timeStamp;
        SetXPosition(newPosition.x);

       transform.position = newPosition + Grid.Instance.transform.position; //Apply grid's position to set global position

       foreach (Fruit fruit in fruits)
       {
           fruit.SetPosition(fruit.transform.localPosition + newPosition);
       }
    }

    public override void UpdateCircleSize()
    {
        foreach (Fruit fruit in fruits)
        {
            fruit.UpdateCircleSize();
        }
    }

    public override void OnHightlight()
    {
        fruits.ForEach(f => f.OnHightlight());
    }

    public override void UnHighlight()
    {
        fruits.ForEach(f => f.UnHighlight());
    }

    /// <summary> Displays a preview of a slider if `previewFruit` was added </summary>
    public void DisplayPreview(Fruit previewFruit)
    {
        fruits.Add(previewFruit);
        fruits.Sort((fruit1,fruit2) => fruit1.position.y.CompareTo(fruit2.position.y));
        UpdateLines();
        fruits.Remove(previewFruit);
    }

    public List<Fruit> GetFruits() => fruits;

    public Fruit GetLastFruit() => fruits[fruits.Count - 1];

    //TODO: needs rework (position x is scaled by grid width ratio)
    public Vector2 GetProjectedPosition(Vector2 mousePosition)
    {
        Vector2 direction = fruits[0].position - fruits[1].position;
        direction.x *= -1; //invert x-axis
        direction.Normalize();
        Vector2 mouseDirectionToLastFruit = Input.mousePosition - fruits[0].transform.position;
        Vector3 projectedLine = Vector2.Dot(mouseDirectionToLastFruit, direction) * direction;
        return fruits[0].transform.position + projectedLine;
    }

    public int GetFruitIndex(Fruit fruit) => fruits.IndexOf(fruit);

    public HitObject GetFruitByIndex(int fruitIndex) => fruits[fruitIndex];

    public List<NVector2> GetSliderPoints()
    {
        List<NVector2> points = new List<NVector2>();
        foreach (Fruit fruit in fruits)
        {
            points.Add(new NVector2(fruit.position.x,0));
        }
        return points;
    }

}