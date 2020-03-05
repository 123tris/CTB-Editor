using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class DragSelect : MonoBehaviour
{
    private Vector2 startDragPos;

    private Image image;

    void Start()
    {
        image = GetComponent<Image>();
        image.enabled = false;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            image.enabled = true;
            startDragPos = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0)) image.enabled = false;
    }
}
