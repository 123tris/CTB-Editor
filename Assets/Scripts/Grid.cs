using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class Grid : Singleton<Grid>
{
    public float lineWidth = 1;

    public float columns
    {
        get { return gridRenderer.GridColumns; }
        set { gridRenderer.GridColumns = (int) value; }
    }
    public float rows
    {
        get { return gridRenderer.GridRows; }
        set { gridRenderer.GridRows = (int)value; }
    }

    private UIGridRenderer gridRenderer;
    private RectTransform rectTransform;

    void Start()
    {
        gridRenderer = GetComponent<UIGridRenderer>();
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        CalculateHorizontalLines();
    }

    private void CalculateHorizontalLines()
    {
        float lines = GetVisibleTimeRange() / 1000 * (TextUI.Instance.BPM / 60) * BeatsnapDivisor.Instance.division;
        rows = lines;
    }

    private float GetVisibleTimeRange()
    {
        return DifficultyRange(TextUI.Instance.AR, 1800, 1200, 450);
    }

    private float DifficultyRange(float difficulty, float min, float mid, float max)
    {
        if (difficulty > 5)
            return mid + (max - mid) * (difficulty - 5) / 5;
        if (difficulty < 5)
            return mid - (mid - min) * (5 - difficulty) / 5;
        return mid;
    }

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
