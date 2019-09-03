using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
    private Grid grid => Grid.Instance;
    private Slider createdSlider;

    private Vector3 distanceFromSliderFruit;
    [SerializeField] private GameObject fruitDisplayPrefab;
    private Fruit fruitDisplay;

    void Start()
    {
        if (fruitDisplayPrefab == null)
            Debug.LogError("Fruit display is not set!", this);
        else if (fruitDisplayPrefab.GetComponent<Fruit>() == null)
            Debug.LogError("Fruit display prefab does not contain a fruit script!", fruitDisplayPrefab);

        fruitDisplay = Instantiate(fruitDisplayPrefab, transform).GetComponent<Fruit>();
        fruitDisplay.name = "Fruit preview display";
    }

    public void SetBrushState(int index)
    {
        state = (BrushState)index;
    }

    void Update()
    {
        mousePositionOnGrid = grid.GetMousePositionOnGrid();
        brushCoords.text = (Input.mousePosition - grid.transform.position).ToString("F2");

        //Display fruit over cursor for accurate placement
        if (state != BrushState.Select && WithinGridRange(Input.mousePosition))
        {
            fruitDisplay.transform.position = mousePositionOnGrid + grid.transform.position;
            fruitDisplay.gameObject.SetActive(true);
            fruitDisplay.UpdateCircleSize();
            fruitDisplay.position.x = (int) mousePositionOnGrid.x;
            fruitDisplay.position.y = (int) grid.GetHitTime(mousePositionOnGrid);
            if (state == BrushState.Slider && createdSlider != null && createdSlider.fruitCount >= 2)
            {
                fruitDisplay.transform.position = createdSlider.GetProjectedPosition(mousePositionOnGrid);
            }
        }
        else fruitDisplay.gameObject.SetActive(false);

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
                Debug.Assert(hitObject.initialized,"Selected uninitialized hitobject",hitObject);
                Slider slider = hitObject.transform.parent.GetComponent<Slider>();
                if (slider) //Select slider
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
                    if (Input.GetKey(KeyCode.LeftControl))
                    {
                        if (Selection.Contains(hitObject)) Selection.Remove(hitObject);
                        else Selection.Add(hitObject);
                    }
                    else if (!Selection.Contains(hitObject)) Selection.SetSelected(hitObject);
                }
            }
            else Selection.Clear();
        }

        if (Input.GetKeyDown(KeyCode.Delete))
            Selection.DestroySelected();

        if (Selection.selectedHitObjects.Count == 0) return;

        SelectionBehaviour();

        Selection.UpdateDragging();
    }

    private void OnFruitState()
    {
        if (Input.GetMouseButton(0))
        {
            if (TimeStampOccupied())
            {
                HitObject hitObject = HitObjectManager.GetHitObjectByTime((int)grid.GetHitTime(mousePositionOnGrid));
                hitObject.SetXPosition(mousePositionOnGrid.x);
            }
            else
                HitObjectManager.CreateFruit(mousePositionOnGrid, transform);
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
        HitObject previousHitObject = HitObjectManager.GetPreviousHitObject(Selection.last);
        HitObject nexHitObject = HitObjectManager.GetNextHitObject(Selection.last);

        string prev = "Prev: ";
        string next = "Next: ";

        if (previousHitObject != null)
            prev += $"{previousHitObject.position.x - Selection.last.position.x}";
        if (nexHitObject != null)
            next += $"{nexHitObject.position.x - Selection.last.position.x}";

        nextHitobjectTime.text = prev + ", " + next + $"\nPosition x: {Selection.last.position.x}\nTime: {Selection.last.position.y}ms";
    }

    private Slider CreateSlider()
    {
        Slider slider = HitObjectManager.CreateSlider(Input.mousePosition, transform);
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
        return HitObjectManager.ContainsFruit((int)grid.GetHitTime(mousePositionOnGrid));
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
