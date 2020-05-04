using UnityEngine;
using UnityEngine.UI;
// ReSharper disable Unity.PreferAddressByIdToGraphicsParams

public class Grid : MonoBehaviour
{
    public const int DEFAULT_OSU_PLAYFIELD_WIDTH = 512;
    public const int DEFAULT_OSU_PLAYFIELD_HEIGHT = 384;

    /// <summary>
    /// How big the editor is compared to the playfield. 
    /// Should always be > 1.
    /// <para> width / OSU_PLAYFIELD_WIDTH</para>
    /// </summary>
    public static float GetWidthRatio() => Instance.width / DEFAULT_OSU_PLAYFIELD_WIDTH;

    public static float GetHeightRatio() => Instance.height / DEFAULT_OSU_PLAYFIELD_HEIGHT;

    ///<summary>The amount of milliseconds passed in the beatmap relative to a single pixel
    /// <para>This takes zoom into consideration. If you want a clean msPerPixel without the zoom taken into consideration you will have to recalculate it.</para></summary>
    public float msPerPixel => height / zoom / GetVisibleTimeRange(); 

    public float columns
    {
        get => gridMaterial.GetFloat("_Columns");  //TODO: retrieving from material is slow, value could be cached
        set => gridMaterial.SetFloat("_Columns", value);
    }

    public float rows
    {
        get => gridMaterial.GetFloat("_Rows");
        set => gridMaterial.SetFloat("_Rows", value);
    }

    public float rowOffset
    {
        get => gridMaterial.GetFloat("_RowOffset");
        set => gridMaterial.SetFloat("_RowOffset", value);
    }

    public int beatsnapDivisor
    {
        get => gridMaterial.GetInt("_BeatsnapDivision");
        set => gridMaterial.SetInt("_BeatsnapDivision", value);
    }

    private Material gridMaterial;
    private RectTransform rectTransform;

    public float height => rectTransform.sizeDelta.y;
    public float width => rectTransform.sizeDelta.x;

    public Vector2 GetSnappedMousePosition() => NearestPointOnGrid(Input.mousePosition);
    public Vector2 GetMousePositionOnGrid() => GetSnappedMousePosition() - transform.position.ToVector2();

    public static Grid Instance;

    public float zoom = 1;
    public float minZoom = 1;
    public float maxZoom = 4;

    public int test = 80;

    void Awake() => Instance = this;

    void Start()
    {
        gridMaterial = GetComponent<Image>().materialForRendering;
        rectTransform = GetComponent<RectTransform>();
        float pixelHeight = Screen.height * height / 1080;
        float pixelWidth = Screen.width * width / 1920;
        gridMaterial.SetVector("_RectSize", rectTransform.sizeDelta);
    }

    void Update()
    {
        rows = CalculateRows();

        if (Input.GetKey(KeyCode.LeftControl) && Input.mouseScrollDelta.y != 0)
        {
            zoom = Mathf.Clamp(zoom - Input.mouseScrollDelta.y / 5, minZoom, maxZoom);
        }

        rowOffset = TimeLine.CurrentTimeStamp * msPerPixel - height / 10 - BeatmapSettings.BPMOffset * msPerPixel;
    }

    public float GetTimeStampOffset() => GetHitIndicatorOffset() + BeatmapSettings.BPMOffset;

    public float GetHitIndicatorOffset() => GetVisibleTimeRange() / 10;

    ///<summary>Returns how much distance in global space is moved due to the BPM Offset and Hitindicator offset. Takes zoom into consideration</summary>
    public float GetOffset()
    {
        return height / 10 + BeatmapSettings.BPMOffset * msPerPixel;
    }

    private float CalculateRows()
    {
        float visibleTimeRange = zoom * GetVisibleTimeRange();
        beatsnapDivisor = BeatsnapDivisor.Instance.division;
        return visibleTimeRange / 1000 * (BeatmapSettings.BPM / 60) * beatsnapDivisor;
    }

    /// <summary> Amount of milliseconds that it takes for a fruit to move from the top to the bottom of the screen </summary>
    public float GetVisibleTimeRange() => DifficultyCalculator.DifficultyRange(BeatmapSettings.AR, 1800, 1200, 450);

    /// <summary> Returns the global position of the nearest point on the grid </summary>
    /// <param name="point">A global position on the grid</param>
    public Vector2 NearestPointOnGrid(Vector2 point)
    {
        //Apply local grid position
        point -= new Vector2(transform.position.x, transform.position.y);

        //Snap X position
        float columnDistance;
        if (Mathf.RoundToInt(columns) > 0)
            columnDistance = width / columns;
        else
            columnDistance = width / 512; //Osu's playfield width is 512 so any x position of a fruit needs to be a fraction of that

        point.x = Mathf.Round(point.x / columnDistance) * columnDistance;

        //Snap Y position
        float rowDistance = height / rows;
        float nearestRow = Mathf.Round((point.y + rowOffset % rowDistance) / rowDistance);
        point.y = nearestRow * rowDistance - rowOffset % rowDistance;
        point += transform.position.ToVector2();
        return point;
    }

    /// <summary>Make sure y is in Grid space and not global space</summary>
    public float GetHitTime(float y)
    {
        y -= height / 10; //Apply hit indicator offset
        return y * (GetVisibleTimeRange() * zoom) / height + TimeLine.CurrentTimeStamp - BeatmapSettings.BPMOffset;
    }

    public float GetHitTime(Vector2 pos) => GetHitTime(pos.y);

    //Gets the Y position of a specific hit time
    public float GetYPosition(float hitTime)
    {
        return (hitTime + GetHitIndicatorOffset()) * (height / GetVisibleTimeRange() /*ms/s*/);
    }

    public bool WithinGridRange(Vector2 position)
    {
        bool withinXBounds = position.x > transform.position.x && position.x < transform.position.x + width;
        bool withinYBounds = position.y > transform.position.y && position.y < transform.position.y + height;
        bool withinGridRange = withinYBounds && withinXBounds;
        return withinGridRange;
    }
}
