using UnityEngine;
using UnityEngine.UI;

public static class ColorExtensions
{
    public static Color OverrideAlpha (this Color color, float a)
    {
        return new Color(color.r,color.g,color.b,a); //
    }

    //Changes the normal color's alpha of a color block
    public static ColorBlock OverrideAlpha(this ColorBlock colorBlock, float a)
    {
        colorBlock.normalColor = colorBlock.normalColor.OverrideAlpha(a);
        colorBlock.highlightedColor = colorBlock.highlightedColor.OverrideAlpha(a);
        colorBlock.selectedColor = colorBlock.selectedColor.OverrideAlpha(a);
        return colorBlock;
    }
}
