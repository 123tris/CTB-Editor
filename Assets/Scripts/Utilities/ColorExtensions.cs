using UnityEngine;

public static class ColorExtensions
{
    public static Color OverrideAlpha (this Color color, float a)
    {
        return new Color(color.r,color.g,color.b,a); //
    }
}
