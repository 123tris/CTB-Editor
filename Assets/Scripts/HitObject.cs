using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class HitObject : MonoBehaviour
{
    /// <summary>
    /// The local position from the grid's perspective.
    /// e.g. If the object's position is the same as the grid's position then position is 0
    /// </summary>
    public Vector2Int position;

    public void SetPosition(Vector3 newPosition)
    {
        transform.position = newPosition.ToInt();
        position = (newPosition - Grid.Instance.transform.position).ToVector2Int();
        position.y += TimeLine.currentTimeStamp;
    }

    public abstract void OnHightlight();

    public abstract void UnHighlight();
}
