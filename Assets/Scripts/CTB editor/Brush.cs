using System;
using System.Collections.Generic;
using RuntimeUndo;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public enum BrushState
{
    Select, Fruit, Slider
}

[DisallowMultipleComponent]
public class Brush : MonoBehaviour
{
    public BrushState state = BrushState.Fruit;
    public Vector3 mousePositionOnGrid;

    [SerializeField] private GameObject selectPanel;
    [SerializeField] private GameObject fruitPanel;
    [SerializeField] private GameObject sliderPanel;

    private Grid grid => Grid.Instance;
    private Slider createdSlider;

    private Vector3 distanceFromSliderFruit;
    [SerializeField] private GameObject fruitDisplayPrefab;
    public Fruit fruitDisplay;

    public static Vector2 startSelectPos;

    private SelectionBox selectionBox;
    private bool createSelectionBox;

    void Start()
    {
        if (fruitDisplayPrefab == null)
            Debug.LogError("Fruit display is not set!", this);
        else if (fruitDisplayPrefab.GetComponent<Fruit>() == null)
            Debug.LogError("Fruit display prefab does not contain a fruit script!", fruitDisplayPrefab);

        fruitDisplay = Instantiate(fruitDisplayPrefab, transform).GetComponent<Fruit>();
        SVGImage fruitImage = fruitDisplay.GetComponent<SVGImage>();
        fruitImage.raycastTarget = false;
        fruitImage.color.OverrideAlpha(0.5f);
        fruitDisplay.name = "Fruit preview display";
        selectionBox = FindObjectOfType<SelectionBox>();
        selectionBox.enabled = false;
    }

    public void SetBrushState(int index)
    {
        state = (BrushState)index;
    }

    void UpdatePanels()
    {
        selectPanel.SetActive(false);
        fruitPanel.SetActive(false);
        sliderPanel.SetActive(false);

        if (state == BrushState.Select)
            selectPanel.SetActive(true);
        else if (state == BrushState.Fruit)
            fruitPanel.SetActive(true);
        else
            sliderPanel.SetActive(true);
    }

    void Update()
    {
        transform.localScale = new Vector3(1, 1 / grid.zoom, 1);

        UpdatePanels();

        mousePositionOnGrid = grid.GetMousePositionOnGrid();

        //Display fruit over cursor for accurate placement
        if (state != BrushState.Select && WithinGridRange(Input.mousePosition))
        {
            fruitDisplay.SetPosition(mousePositionOnGrid);
            fruitDisplay.UpdateVisuals();
            fruitDisplay.gameObject.SetActive(true);
        }
        else fruitDisplay.gameObject.SetActive(false);

        //Brush inputs
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
            state = BrushState.Select;
        if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
            state = BrushState.Fruit;
        if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
            state = BrushState.Slider;

        if (Input.GetMouseButtonUp(0))
        {
            createSelectionBox = false;
            selectionBox.enabled = false;
            selectionBox.EndSelection();
        }

        if (!WithinGridRange(Input.mousePosition)) return;

        if (Input.GetMouseButtonDown(1))
        {
            Fruit detectedHitobject = DetectHitObject(); //TODO: Add undo
            if (detectedHitobject != null)
            {
                if (detectedHitobject.isSliderFruit)
                    Undo.DestroyObject(detectedHitobject.slider.gameObject);
                else
                    Undo.DestroyObject(detectedHitobject.gameObject);
            }
        }

        switch (state)
        {
            case BrushState.Select:
                OnSelectState();
                break;
            case BrushState.Fruit:
                OnFruitState();
                break;
            case BrushState.Slider:
                OnSliderState();
                break;
        }
    }

    private void OnSelectState()
    {
        //Select pressed hitobject
        if (Input.GetMouseButtonDown(0))
        {
            //Higlighting of hitobjects
            Fruit hitObject = DetectHitObject();
            if (hitObject != null)
            {
                Slider slider = hitObject.transform.parent.GetComponent<Slider>();
                if (slider && !ClickManager.DoubleClick()) //Select slider
                {
                    if (Input.GetKey(KeyCode.LeftControl))
                    {
                        if (Selection.Contains(slider)) Selection.Remove(slider);
                        else Selection.Add(slider);
                    }
                    else if (!Selection.Contains(slider)) Selection.SetSelected(slider);
                }
                else //Select fruit
                {
                    //Debug.Assert(hitObject.initialized,"Selected uninitialized hitobject",hitObject);
                    if (Input.GetKey(KeyCode.LeftControl))
                    {
                        if (Selection.Contains(hitObject)) Selection.Remove(hitObject);
                        else Selection.Add(hitObject);
                    }
                    else if (!Selection.Contains(hitObject)) Selection.SetSelected(hitObject);
                }
            }
            else
            {
                Selection.Clear();
                startSelectPos = grid.transform.InverseTransformPoint(Input.mousePosition);
                createSelectionBox = true;
                selectionBox.enabled = true;
                selectionBox.BeginSelection();
            }
        }

        //Dragging selection box
        if (createSelectionBox)
        {
            Selection.SetSelected(DetectHitObjects());
        }
        else
        {

            Selection.UpdateDragging();
        }
    }

    private void OnFruitState()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (TimeStampOccupied())
            {
                HitObject hitObject = HitObjectManager.GetHitObjectByTime(Mathf.RoundToInt(grid.GetHitTime(mousePositionOnGrid)));
                if (hitObject is Fruit)
                {
                    Undo.RecordHitObject(hitObject);
                    hitObject.SetXPosition(mousePositionOnGrid.x);
                }
            }
            else
            {
                var fruit = HitObjectManager.CreateFruit(mousePositionOnGrid, transform);
                Undo.RegisterCreatedObject(fruit.gameObject);
            }
        }
    }

    private void OnSliderState()
    {
        if (Input.GetMouseButtonDown(0) && !TimeStampOccupied())
        {
            //If create slider is null then we're creating our first slider
            if (createdSlider == null)
            {
                createdSlider = HitObjectManager.CreateSlider(mousePositionOnGrid, transform);
            }
            else
            {
                createdSlider.AddFruit(mousePositionOnGrid);
                createdSlider = null;
            }
        }

        if (createdSlider != null)
        {
            //Display preview
            createdSlider.DisplayPreview(fruitDisplay.GetComponent<Fruit>());

            if (Input.GetMouseButtonDown(1)) //Right click when currently making a slider
            {
                //Finish slider creation
                createdSlider.AddFruit(mousePositionOnGrid);
                createdSlider = null;
            }
        }
    }

    private Fruit DetectHitObject()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        if (results.Count > 0)
        {
            foreach (RaycastResult raycastResult in results)
            {
                if (raycastResult.gameObject.CompareTag("HitObject"))
                {
                    return raycastResult.gameObject.GetComponent<Fruit>();

                }
            }
        }

        return null;
    }

    private List<Fruit> DetectHitObjects()
    {
        float GetHitTime(float y)
        {
            y -= grid.height / 10; //Apply hit indicator offset
            return y * grid.GetVisibleTimeRange() * grid.zoom / grid.height + TimeLine.CurrentTimeStamp;
        }

        //Find fruits within the y-axis of the selection box
        Vector2 endSelectPos = grid.transform.InverseTransformPoint(Input.mousePosition);

        int minY = (int)Mathf.Min(startSelectPos.y, endSelectPos.y);
        int maxY = (int)Mathf.Max(startSelectPos.y, endSelectPos.y);


        float startTime = GetHitTime(minY);
        float endTime = GetHitTime(maxY);

        var fruitsByRange = HitObjectManager.GetFruitsByRange(startTime, endTime);

        //Find fruits within the x-axis of the selection box
        var startWorld = grid.transform.TransformPoint(startSelectPos);
        var endWorld = grid.transform.TransformPoint(endSelectPos);

        float minX = Mathf.Min(startWorld.x, endWorld.x);
        float maxX = Mathf.Max(startWorld.x, endWorld.x);

        List<Fruit> fruitsWithinSelectionBox = new List<Fruit>();

        foreach (Fruit fruit in fruitsByRange)
        {
            if (fruit.transform.position.x >= minX && fruit.transform.position.x <= maxX)
            {
                fruitsWithinSelectionBox.Add(fruit);
            }
        }

        return fruitsWithinSelectionBox;
    }

    private bool TimeStampOccupied()
    {
        return HitObjectManager.ContainsFruit(Mathf.RoundToInt(grid.GetHitTime(mousePositionOnGrid.y)));
    }

    /// <summary>
    /// Returns whether or not the given position is inside of the grid
    /// </summary>
    private bool WithinGridRange(Vector2 vec)
    {
        Vector3 gridPos = grid.transform.position;
        RectTransform gridRect = grid.GetComponent<RectTransform>();
        return vec.x > gridPos.x && vec.y > gridPos.y && vec.x < gridPos.x + gridRect.sizeDelta.x && vec.y < gridPos.y + gridRect.sizeDelta.y;
    }
}
