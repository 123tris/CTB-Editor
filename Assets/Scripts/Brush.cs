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
    private Grid grid;
    private HitObjectManager hitObjectManager = new HitObjectManager();
    private HitObject selectedHitObject;
    private HitObject draggingHitObject = null;
    private Slider createdSlider = null;

    private Vector3 distanceFromSliderFruit;

    void Start()
    {
        grid = Grid.Instance;
    }

    public void SetBrushState(int index)
    {
        state = (BrushState)index;
    }

    public void UpdateLevel()
    {
        transform.localPosition = new Vector2(0, -TimeLine.currentTimeStamp);
    }

    void Update()
    {
        mousePositionOnGrid = Input.mousePosition - grid.transform.position;
        brushCoords.text = mousePositionOnGrid.ToString("F2");
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


        //Display fruit over cursor for accurate placement
        //if (state != BrushState.Select && WithinGridRange(Input.mousePosition))
        //{
        //    fruitDisplay.transform.position = grid.NearestPointOnGrid(mousePositionOnGrid);
        //    fruitDisplay.SetActive(true);
        //}
        //else fruitDisplay.SetActive(false);
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
                    selectedHitObject?.UnHighlight();

                    selectedHitObject = hitObject;
                    draggingHitObject = hitObject;
                    hitObject.OnHightlight();
                }
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
        {
            selectedHitObject?.UnHighlight();
            draggingHitObject = null;
            selectedHitObject = null;
        }

        

        if (draggingHitObject == null) return;
        DraggingBehaviour();
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
        hitObjectManager.hitObjects[draggingHitObject.position.y] = null;

        //Update position of dragging fruit
        draggingHitObject.SetPosition(Input.mousePosition);

        hitObjectManager.hitObjects[draggingHitObject.position.y] = draggingHitObject;
    }

    private void DragSlider()
    {
        Slider draggingSlider = (Slider) draggingHitObject;
        draggingSlider.MoveSlider(Input.mousePosition + distanceFromSliderFruit);
    }

    private void OnFruitState()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!TimeStampOccupied())
            {
                hitObjectManager.CreateFruit(Input.mousePosition,transform);
            }
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
                Undo.RecordObject(createdSlider, "Create Slider Fruit");
                createdSlider.AddFruit(Input.mousePosition);
            }
        }

        if (createdSlider != null && Input.GetMouseButtonDown(1)) //Right click when currently making a slider
        {
            //Finish slider creation
            createdSlider.AddFruit(Input.mousePosition);
            createdSlider = null;
        }
    }

    private Slider CreateSlider()
    {
        Slider slider = hitObjectManager.CreateSlider(Input.mousePosition,transform);
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
        return hitObjectManager.ContainsFruit((int)mousePositionOnGrid.y);
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
