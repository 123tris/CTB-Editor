using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public enum BrushState
{
    Select, Fruit, Slider
}

[DisallowMultipleComponent]
public class Brush : MonoBehaviour
{
    public BrushState state = BrushState.Fruit;
    public Vector3 mousePositionOnGrid;

    [SerializeField] private Text brushCoords;
    [SerializeField] private Text nextHitobjectTime;
    private Grid grid;
    private HitObjectManager hitObjectManager = new HitObjectManager();
    private HitObject selectedHitObject;
    private HitObject draggingHitObject;
    private Slider createdSlider;

    private Vector3 distanceFromSliderFruit;
    [SerializeField] private GameObject fruitDisplayPrefab;
    private GameObject fruitDisplay;

    void Start()
    {
        grid = Grid.Instance;
        if (fruitDisplayPrefab == null)
            Debug.LogError("Fruit display is not set!", this);
        else if (fruitDisplayPrefab.GetComponent<Fruit>() == null)
            Debug.LogError("Fruit display prefab does not contain a fruit script!", fruitDisplayPrefab);

        fruitDisplay = Instantiate(fruitDisplayPrefab, transform);
    }

    public void SetBrushState(int index)
    {
        state = (BrushState)index;
    }

    void Update()
    {
        mousePositionOnGrid = grid.NearestPointOnGrid(Input.mousePosition);
        brushCoords.text = (Input.mousePosition - grid.transform.position).ToString("F2");

        //Display fruit over cursor for accurate placement
        if (state != BrushState.Select && WithinGridRange(Input.mousePosition))
        {
            fruitDisplay.transform.position = mousePositionOnGrid;
            fruitDisplay.SetActive(true);
            fruitDisplay.GetComponent<Fruit>().UpdateCircleSize();
            if (state == BrushState.Slider && createdSlider != null && createdSlider.fruitCount >= 2)
            {
                fruitDisplay.transform.position = createdSlider.GetProjectedPosition(mousePositionOnGrid);
            }
        }
        else fruitDisplay.SetActive(false);

        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
            state = BrushState.Select;
        if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
            state = BrushState.Fruit;
        if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
            state = BrushState.Slider;

        if (!WithinGridRange(Input.mousePosition)) return;

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
        //On left click
        if (Input.GetMouseButtonDown(0))
        {
            //Higlighting of hitobjects
            Fruit hitObject = DetectHitObject();
            if (hitObject != null)
            {
                //Select slider
                if (!ClickManager.DoubleClick() && hitObject.transform.parent.GetComponent<Slider>())
                {
                    Slider slider = hitObject.transform.parent.GetComponent<Slider>();
                    selectedHitObject = slider;

                    //When the dragging starts
                    if (draggingHitObject == null)
                    {
                        distanceFromSliderFruit = slider.transform.position - Input.mousePosition;
                    }

                    draggingHitObject = slider;
                    slider.OnHightlight();
                }
                else //Select fruit
                {
                    //Unhighlight previously selected 
                    if (selectedHitObject != null) selectedHitObject.UnHighlight();

                    selectedHitObject = hitObject;
                    draggingHitObject = hitObject;
                    hitObject.OnHightlight();
                }
            }
            else
            {
                if (selectedHitObject != null) selectedHitObject.UnHighlight();
                selectedHitObject = null;
            }
        }

        if (Input.GetKeyDown(KeyCode.Delete))
        {
            Destroy(selectedHitObject.gameObject);
            selectedHitObject = null;
            draggingHitObject = null;
        }

        //Reset dragging if mouse button is released
        if (Input.GetMouseButtonUp(0))
            draggingHitObject = null;

        if (selectedHitObject == null) return;
        SelectionBehaviour();

        if (draggingHitObject == null) return;
        DraggingBehaviour();
    }

    private void OnFruitState()
    {
        if (Input.GetMouseButton(0))
        {
            if (TimeStampOccupied())
            {
                HitObject hitObject = hitObjectManager.hitObjects[(int)grid.GetHitTime(mousePositionOnGrid)];
                hitObject.SetXPosition(mousePositionOnGrid.x);
            }
            else
                hitObjectManager.CreateFruit(mousePositionOnGrid, transform);
        }
    }

    private void OnSliderState()
    {
        if (Input.GetMouseButtonDown(0) && !TimeStampOccupied())
        {
            //If create slider is null then we're creating our first slider
            if (createdSlider == null)
                createdSlider = CreateSlider();
            else
            {
                createdSlider.AddFruit(fruitDisplay.transform.position);
            }
        }

        if (createdSlider != null)
        {
            //Display preview
            createdSlider.DisplayPreview(fruitDisplay.GetComponent<Fruit>());

            if (Input.GetMouseButtonDown(1)) //Right click when currently making a slider
            {
                //Finish slider creation
                createdSlider.AddFruit(fruitDisplay.transform.position);
                createdSlider = null;
            }
        }
    }

    private void SelectionBehaviour()
    {
        HitObject previousHitObject = hitObjectManager.GetPreviousHitObject(selectedHitObject);
        HitObject nexHitObject = hitObjectManager.GetNextHitObject(selectedHitObject);

        string prev = "Prev: ";
        string next = "Next: ";

        if (previousHitObject != null)
            prev += $"{previousHitObject.position.x - selectedHitObject.position.x}";
        if (nexHitObject != null)
            next += $"{nexHitObject.position.x - selectedHitObject.position.x}";

        nextHitobjectTime.text = prev + ", " + next +$"\nPosition x: {selectedHitObject.position.x}\nTime: {selectedHitObject.position.y}ms";
    }

    private void DraggingBehaviour()
    {
        if (draggingHitObject is Slider)
            DragSlider();
        else
            DragFruit();
    }

    private void DragFruit()
    {
        //Update position of dragging fruit
        draggingHitObject.SetXPosition(mousePositionOnGrid.x);
        draggingHitObject.SetPosition(mousePositionOnGrid);
    }

    private void DragSlider()
    {
        Slider draggingSlider = (Slider)draggingHitObject;
        draggingSlider.MoveSlider(Input.mousePosition + distanceFromSliderFruit);
    }

    private Slider CreateSlider()
    {
        Slider slider = hitObjectManager.CreateSlider(Input.mousePosition, transform);
        slider.AddFruit(Input.mousePosition);
        return slider;
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

    private bool TimeStampOccupied()
    {
        return hitObjectManager.ContainsFruit((int)grid.GetHitTime(mousePositionOnGrid));
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
