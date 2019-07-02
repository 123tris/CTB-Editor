using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI.Extensions;

public class Slider : HitObject
{
    [HideInInspector] public GameObject fruitPrefab; //When brush creates a slider it is required to set the fruitprefab

    //Previous fruit should have a lower Y and next fruit should have a higher Y
    private List<Fruit> fruits = new List<Fruit>();
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
        //TODO: Set position of slider when the first fruit is placed

        //Spawn fruit
        Fruit fruit = HitObjectManager.instance.CreateSliderFruit(spawnPosition,transform);
        #if UNITY_EDITOR
        Undo.RecordObject(lineRenderer,"Create Slider Fruit");
        #endif

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

    public void MoveSlider(Vector2 pPosition)
    {
        SetPosition(pPosition);
        foreach (Fruit fruit in fruits)
            fruit.SetPosition(fruit.transform.position);
    }

    /// <summary> Displays a preview of a slider if `previewFruit` was added </summary>
    public void DisplayPreview(Fruit previewFruit)
    {
        fruits.Add(previewFruit);
        fruits.Sort((fruit1,fruit2) => fruit1.position.y.CompareTo(fruit2.position.y));
        UpdateLines();
        fruits.Remove(previewFruit);
    }

    public Fruit GetLastFruit()
    {
        return fruits[fruits.Count - 1];
    }

    public Vector2 GetProjectedPosition(Vector2 mousePosition)
    {
        Vector2 direction = fruits[0].position - fruits[1].position;
        direction.x *= -1; //invert x-axis
        direction.Normalize();
        Vector2 mouseDirectionToLastFruit = Input.mousePosition - fruits[0].transform.position;
        Vector3 projectedLine = Vector2.Dot(mouseDirectionToLastFruit, direction) * direction;
        return fruits[0].transform.position + projectedLine;
    }
}