using System;
using UnityEngine;
using UnityEngine.UI;

public class Grid : Singleton<Grid>
{
    public float columns
    {
        get { return gridMaterial.GetFloat("_Columns"); } //TODO: retrieving from material is slow, value could be cached
        set { gridMaterial.SetFloat("_Columns", value); }
    }

    public float rows
    {
        get { return gridMaterial.GetFloat("_Rows"); }
        set { gridMaterial.SetFloat("_Rows", value); }
    }

    public float rowOffset
    {
        get { return gridMaterial.GetFloat("_RowOffset"); }
        set { gridMaterial.SetFloat("_RowOffset", value); }
    }

    public int beatsnapDivisor
    {
        get { return gridMaterial.GetInt("_BeatsnapDivision"); }
        set { gridMaterial.SetInt("_BeatsnapDivision", value); }
    }

    private Material gridMaterial;
    private RectTransform rectTransform;

    public float height => rectTransform.sizeDelta.y;
    public float width => rectTransform.sizeDelta.x;

    public Vector2 GetSnappedMousePosition() => NearestPointOnGrid(Input.mousePosition);
    public Vector2 GetMousePositionOnGrid() => GetSnappedMousePosition() - transform.position.ToVector2();

    void Start()
    {
        gridMaterial = GetComponent<Image>().materialForRendering;
        rectTransform = GetComponent<RectTransform>();
        gridMaterial.SetVector("_RectSize", rectTransform.sizeDelta);
    }

    void Update()
    {
        rows = CalculateRows();

        rowOffset = TimeLine.currentTimeStamp * (height / GetVisibleTimeRange());
    }

    private float CalculateRows()
    {
        float visibleTimeRange = GetVisibleTimeRange();
        beatsnapDivisor = BeatsnapDivisor.Instance.division;
        return visibleTimeRange / 1000 * (BeatmapSettings.BPM / 60) * beatsnapDivisor;
    }

    public float GetVisibleTimeRange() => DifficultyCalculator.DifficultyRange(BeatmapSettings.AR, 1800, 1200, 450);

    /// <summary>
    /// Returns the global position of the nearest point on the grid
    /// </summary>
    /// <param name="point">A position on the grid</param>
    /// <returns></returns>
    public Vector2 NearestPointOnGrid(Vector2 point)
    {
        //Apply local grid position
        point -= new Vector2(transform.position.x, transform.position.y);

        //Snap X position
        if (columns > 0)
        {
            float columnDistance = width / columns;
            point.x = Mathf.Round(point.x / columnDistance) * columnDistance;
        }

        //Snap Y position
        float rowDistance = height / rows;
        float nearestRow = Mathf.Round((point.y + rowOffset % rowDistance) / rowDistance);
        point.y = nearestRow * rowDistance - rowOffset % rowDistance;
        point += new Vector2(transform.position.x, transform.position.y);
        return point;
    }

    /// <summary>Make sure pos is in Grid space and not global space</summary>
    private float GetHitTime(int y) => y * (GetVisibleTimeRange() / height) + TimeLine.currentTimeStamp;
    public float GetHitTime(Vector2 pos) => GetHitTime((int)pos.y);

    public bool WithinGridRange(Vector2 position)
    {
        bool withinXBounds = position.x > transform.position.x && position.x < transform.position.x + width;
        bool withinYBounds = position.y > transform.position.y && position.y < transform.position.y + height;
        bool withinGridRange = withinYBounds && withinXBounds;
        return withinGridRange;
    }

    public Vector2 GetGridSize()
    {
        return rectTransform.sizeDelta;
    }
}
