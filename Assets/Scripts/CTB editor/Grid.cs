using System;
using UnityEngine;
using UnityEngine.UI;

public class Grid : Singleton<Grid>
{
    public float columns
    {
        get { return gridMaterial.GetFloat ("_Columns"); } //TODO: retrieving from material is slow, value could be cached
        set { gridMaterial.SetFloat ("_Columns", value); }
    }

    public float rows
    {
        get { return gridMaterial.GetFloat ("_Rows"); }
        set { gridMaterial.SetFloat ("_Rows", value); }
    }

    private Material gridMaterial;
    private RectTransform rectTransform;

    void Start()
    {
        gridMaterial = GetComponent<Image> ().material;
        rectTransform = GetComponent<RectTransform>();
        gridMaterial.SetVector("_RectSize",rectTransform.sizeDelta);
    }

    void Update()
    {
        rows = CalculateRows();

        gridMaterial.SetFloat("_RowOffset",TimeLine.currentTimeStamp % (GetVisibleTimeRange()/rows));
    }

    private float CalculateRows()
    {
        float visibleTimeRange = GetVisibleTimeRange();
        return visibleTimeRange / 1000 * (TextUI.Instance.BPM / 60) * BeatsnapDivisor.Instance.division;
    }

    private float GetVisibleTimeRange() => Math.Abs(DifficultyCalculator.DifficultyRange(TextUI.Instance.AR, 1800, 1200, 450));

    /// <summary>
    /// Returns the global position of the nearest point on the grid
    /// </summary>
    /// <param name="point">A position on the grid</param>
    /// <returns></returns>
    public Vector2 NearestPointOnGrid(Vector2 point)
    {
        point += new Vector2(transform.position.x, transform.position.y);
        return point;
    }

    public bool WithinGridRange(Vector2 position)
    {
        bool withinXBounds = position.x > transform.position.x && position.x < transform.position.x + rectTransform.sizeDelta.x;
        bool withinYBounds = position.y > transform.position.y && position.y < transform.position.y + rectTransform.sizeDelta.y;
        bool withinGridRange = withinYBounds && withinXBounds;
        return withinGridRange;
    }

    public Vector2 GetGridSize()
    {
        return rectTransform.sizeDelta;
    }
}
