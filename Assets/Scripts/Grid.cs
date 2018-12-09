using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class Grid : Singleton<Grid>
{
    List<GameObject> lines = new List<GameObject>();
    public float lineWidth = 1;

    public uint columns
    {
        get { return (uint) gridRenderer.GridColumns; }
        set { gridRenderer.GridColumns = (int) value; }
    }
    public uint rows
    {
        get { return (uint)gridRenderer.GridRows; }
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
        //TODO: implement L = (msPerScreen/1000) * (BPM/60) * (1/division)

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
