using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Grid : Singleton<Grid>
{
    List<GameObject> lines = new List<GameObject>();
    public float lineWidth = 1;
    public uint columns = 128;
    public uint rows;
    private float verticalLineSpacing;
    private float horizontalLineSpacing;
    private RectTransform _rectTransform;
    private RectTransform rectTransform //So when editor code accesses recttranform it will be able to retrieve the component
    {
        get
        {
            if (_rectTransform == null)
                _rectTransform = GetComponent<RectTransform>();
            return _rectTransform;
        }
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

    public void SetupLines()
    {
        //Cleanup lines
        for (int i = 0; i < lines.Count; i++)
        {
            DestroyImmediate(lines[i]);
        }
        lines.Clear();

        //Remove children
        for (int i = 0; i < transform.childCount; i++)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }

        //Calculate line spacing
        verticalLineSpacing = rectTransform.sizeDelta.y / rows;
        horizontalLineSpacing = rectTransform.sizeDelta.x / columns;

        //Create graph lines
        //CreateLines(rows, columns);
    }

    private void CreateLines(uint pRows, uint pColumns)
    {
        for (uint i = 0; i < pColumns + 1; i++)
        {
            //Create vertical line
            Transform obj = CreateLine("Vertical line " + i, false).transform;
            RectTransform childRectTrans = obj.GetComponent<RectTransform>();

            //Set position
            obj.position = new Vector3(i * horizontalLineSpacing, 0) + transform.position;
            obj.SetParent(transform);

            //Set size
            childRectTrans.sizeDelta = new Vector2(lineWidth, rectTransform.sizeDelta.y);
        }

        for (uint i = 0; i < pRows + 1; i++)
        {
            //Create horizontal line
            Transform obj = CreateLine("Horizontal line " + i, true).transform;
            RectTransform childRectTrans = obj.GetComponent<RectTransform>();

            //Set position
            obj.position = new Vector3(0, i * verticalLineSpacing) + transform.position;
            obj.SetParent(transform);

            //Set size
            childRectTrans.sizeDelta = new Vector2(rectTransform.sizeDelta.x, lineWidth);
        }
    }

    private GameObject CreateLine(string objectName, bool horizontal)
    {
        //Create and set name of line
        GameObject obj = new GameObject(objectName);
        lines.Add(obj);

        //Set color of line
        var image = obj.AddComponent<Image>();
        image.color = Color.black;

        //Set pivot of line
        var childRectTrans = obj.GetComponent<RectTransform>();
        if (horizontal)
            childRectTrans.pivot = new Vector2(0, 0.5f);
        else
            childRectTrans.pivot = new Vector2(0.5f, 0);
        return obj;
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


#if UNITY_EDITOR
[CustomEditor(typeof(Grid))]
public class CustomGridEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var grid = (Grid)target;

        if (GUILayout.Button("Update"))
        {
            grid.SetupLines();
        }
    }
}
#endif
