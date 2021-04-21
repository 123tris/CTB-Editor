using UnityEngine;
using UnityEngine.EventSystems;

public static class ClickManager
{
    //property variables
    const float maximumDelay = 0.2f;

    //The time the mouse was clicked
    public static float mouseClickTime;

    public static bool DoubleClick()
    {
        return mouseClickTime + maximumDelay > Time.unscaledTime;
    }

    //Requires some kind of monobehaviour to call this every update (this is bad practice but because of lack of framework and to avoid spending too much time on a framework, a compromise has been made)
    public static void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mouseClickTime = Time.unscaledTime;
        }
    }
}