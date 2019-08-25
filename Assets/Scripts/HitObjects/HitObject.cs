using System;
using UnityEngine;

public abstract class HitObject : MonoBehaviour
{
    /// <summary>
    /// The local position from the grid's perspective.
    /// e.g. If the object's position is the same as the grid's position then position is 0
    /// </summary>
    public Vector2Int position = Vector2Int.down; //It defaults to down to indicate that the hitobject's position has not been set yet
    public HitObjectType type;

    /// <summary>
    /// Whether this fruit can initiate a hyperdash.
    /// </summary>
    public bool hyperDash => hyperDashTarget != null;

    /// <summary>
    /// The target fruit if we are to initiate a hyperdash.
    /// </summary>
    public HitObject hyperDashTarget;

    [NonSerialized] public bool initialized = false;

    protected virtual void Start()
    {
        UnHighlight();
        UpdateCircleSize();
    }

    protected virtual void Update()
    {
        UpdateHyperDashState();
    }

    ///Initialize hitobject, this will add it to the manager and set the position. HitObject.Initialized will turn true
    public void Init(Vector3 pPosition)
    {
        SetPosition(pPosition);
        HitObjectManager.AddHitObject(this);
        initialized = true;
    }

    /// <summary> SetPosition requires a local position from the grid's perspective
    /// <para>It will then reinterpret how the y position translates to the timestamp of the hitobject</para>
    /// It will return when a fruit already occupies the same y position as the one which was passed </summary>
    public void SetPosition(Vector3 newPosition)
    {
        float hitTime = Grid.Instance.GetHitTime(newPosition);
        int timeStamp = (int) Grid.Instance.GetHitTime(newPosition);

        if (HitObjectManager.ContainsFruit(timeStamp)) return;

        if (HitObjectManager.ContainsFruit(position.y)) //If moving an existing fruit
        {
            HitObjectManager.EditHitObjectTimeStamp(this,timeStamp);
        }

        transform.position = newPosition + Grid.Instance.transform.position; //Apply grid's position to set global position
        SetXPosition(newPosition.x);
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

    protected abstract void UpdateHyperDashState();

    public abstract void UpdateCircleSize();

    public abstract void OnHightlight();

    public abstract void UnHighlight();

    private void OnDestroy()
    {
        if (!transform || !transform.parent || !GameManager.garbage) return;
        if (transform.parent != GameManager.garbage.transform && transform.parent.GetComponent<Slider>() == null)
        {
            HitObjectManager.RemoveHitObject(position.y);
            Selection.Remove(this);
        }
    }
}
