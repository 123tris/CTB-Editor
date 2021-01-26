using System;
using OsuParsers.Enums.Beatmaps;
using UnityEngine;

public abstract class HitObject : MonoBehaviour, IComparable
{
    /// <summary>
    /// The local position from the grid's perspective.
    /// e.g. If the object's position is the same as the grid's position then position is 0
    /// </summary>
    public Vector2Int position = Vector2Int.down; //It defaults to down to indicate that the hitobject's position has not been set yet
    
    public HitObjectType type;

    public bool isNewCombo;

    [NonSerialized]
    public HitSoundType hitSound = HitSoundType.Normal;

    /// <summary>
    /// Whether this fruit can initiate a hyperdash.
    /// </summary>
    public bool hyperDash => hyperDashTarget != null;

    /// <summary>
    /// The target fruit if we are to initiate a hyperdash.
    /// </summary>
    public HitObject hyperDashTarget;

    public bool isSliderFruit => this is Fruit && ((Fruit) this).slider != null;

    protected virtual void Start()
    {
        UnHighlight();
    }

    /// <summary> SetPosition requires a local position from the grid's perspective
    /// <para>It will then reinterpret how the y position translates to the timestamp of the hitobject</para>
    /// It will return when a fruit already occupies the same y position as the one which was passed </summary>
    public abstract void SetPosition(Vector3 newPosition);
    //public void SetPosition(Vector3 newPosition)
    //{
    //    SetXPosition(newPosition.x);

    //    float hitTime = Grid.Instance.GetHitTime(newPosition);
    //    int timeStamp = (int) Grid.Instance.GetHitTime(newPosition);

    //    if (HitObjectManager.ContainsFruit(timeStamp)) return;

    //    if (HitObjectManager.ContainsFruit(position.y)) //If moving an existing fruit
    //    {
    //        HitObjectManager.EditFruitTimeStamp(this,timeStamp);
    //    }

    //    transform.position = newPosition + Grid.Instance.transform.position; //Apply grid's position to set global position
    //    position.y = timeStamp;

    //    //If this is a slider fruit, update the line connections
    //    if (transform.parent.GetComponent<Slider>())
    //        transform.parent.GetComponent<Slider>().UpdateLines();

    //}

    public void SetXPosition(float x)
    {
        position.x = Mathf.RoundToInt(x / Grid.GetWidthRatio());
        transform.position = new Vector2(x + Grid.Instance.transform.position.x, transform.position.y);

        if (this is Fruit && ((Fruit) this).isSliderFruit)
        {
            Fruit fruit = (Fruit) this;
            fruit.slider.UpdateLines();
        }
    }

    public abstract void OnHightlight();

    public abstract void UnHighlight();

    private void OnDestroy()
    {
        if (!transform || !transform.parent || !GameManager.garbage) return;
        if (transform.parent != GameManager.garbage.transform)
        {
            if (this is Fruit)
                HitObjectManager.RemoveFruit((Fruit)this);
            else
                HitObjectManager.RemoveSlider((Slider)this);
            Selection.Remove(this);
        }
    }

    public int CompareTo(object obj)
    {
        if (obj == null) return 1;

        HitObject otherHitObject = obj as HitObject;
        if (otherHitObject == null)
            throw new ArgumentException("Object is not a hitobject");

        return position.y.CompareTo(otherHitObject.position.y);
    }
}
