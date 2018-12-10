using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridUI : Graphic
{
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        
        AddQuad(vh,rectTransform.rect.min,rectTransform.rect.max,Color.black,Vector2.zero,Vector2.one);
    }

    void AddQuad(VertexHelper vh, Vector2 lowerleft, Vector2 upperright, Color color, Vector2 lowerleftUV, Vector2 upperrightUV)
    {
        int index = vh.currentIndexCount; //Store starting index count
        //Add vertices
        vh.AddVert(lowerleft, color, lowerleftUV);
        vh.AddVert(new Vector2(lowerleft.x, upperright.y), color, new Vector2(lowerleftUV.x, upperrightUV.y));
        vh.AddVert(upperright, color, upperrightUV);
        vh.AddVert(new Vector2(upperright.x, lowerleft.y), color, new Vector2(upperrightUV.x, lowerleftUV.y));
        //Add triangles
        vh.AddTriangle(index, index + 1, index + 2);
        vh.AddTriangle(index, index + 2, index + 3);
    }
}
