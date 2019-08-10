﻿using System.Collections;
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
    public HitObjectType type;

    /// <summary> SetPosition requires a local position from the grid's perspective
    /// <para>It will then reinterpret how the y position translates to the timestamp of the hitobject</para>
    /// It will return when a fruit already occupies the same y position as the one which was passed </summary>
    public void SetPosition(Vector3 newPosition)
    {
        float hitTime = Grid.Instance.GetHitTime(newPosition);
        int timeStamp = (int) Grid.Instance.GetHitTime(newPosition);

        if (HitObjectManager.instance.ContainsFruit(timeStamp)) return;

        if (HitObjectManager.instance.ContainsFruit(position.y)) //If moving an existing fruit
        {
            HitObjectManager.instance.hitObjects.Remove(position.y); //Remove from previous position
            HitObjectManager.instance.hitObjects[timeStamp] = this;
        }

        transform.position = newPosition + Grid.Instance.transform.position; //Apply grid's position to set global position
        position = newPosition.ToVector2Int();
        position.y = timeStamp;

        //If this is a slider fruit, update the line connections
        if (transform.parent.GetComponent<Slider>())
            transform.parent.GetComponent<Slider>().UpdateLines();

    }

    public void SetXPosition(float x)
    {
        position.x = (int) x;
        transform.position = new Vector2(x + Grid.Instance.transform.position.x, transform.position.y);
    }

    public abstract void UpdateCircleSize();

    public abstract void OnHightlight();

    public abstract void UnHighlight();

    private void OnDestroy()
    {
        HitObjectManager.instance.hitObjects.Remove(position.y);
    }
}
