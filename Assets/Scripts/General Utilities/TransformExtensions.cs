using UnityEngine;

public static class TransformExtensions
{
    static Vector2 referenceResolution = new Vector2(1920, 1080);

    public static void SetGlobalPivot(this Transform transform, Vector2 position)
    {
        transform.position = position * (new Vector2(Screen.width, Screen.height) / referenceResolution);
    }

    public static Vector2 GetGlobalPivot(this Transform transform)
    {
        return transform.position * (referenceResolution / new Vector2(Screen.width, Screen.height));
    }
}
