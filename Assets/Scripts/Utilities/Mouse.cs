using UnityEngine;

public class Mouse
{
    public static Vector2 mouseDelta => new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
}
