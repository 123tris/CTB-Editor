using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class Grid : Singleton<Grid>
{
    public float columns
    {
        get { return gridUI.GetFloat("Columns"); }
        set { gridUI.SetFloat("Columns", value); }
    }
    public float rows
    {
        get { return gridUI.GetFloat("Rows"); }
        set { gridUI.SetFloat("Rows", value); }
    }

    private Material gridUI;
    private RectTransform rectTransform;

    void Start()
    {
        gridUI = GetComponent<Image>().material;
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
