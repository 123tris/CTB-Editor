using UnityEngine;
using UnityEngine.UI;

public class Grid : Singleton<Grid>
{
    public float columns;
    public float rows;

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

    private float GetVisibleTimeRange() => DifficultyCalculator.DifficultyRange(TextUI.Instance.AR, 1800, 1200, 450);

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
