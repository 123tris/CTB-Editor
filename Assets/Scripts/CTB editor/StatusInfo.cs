using UnityEngine;

public class StatusInfo : TextBehaviour
{
    private Brush brush;

    void Start()
    {
        brush = FindObjectOfType<Brush>();
    }

    void Update()
    {
        //Display text
        Vector2 brushCoordsVec = new Vector2();
        brushCoordsVec.x = Mathf.RoundToInt(brush.mousePositionOnGrid.x / Grid.GetWidthRatio());
        brushCoordsVec.y = Grid.Instance.GetHitTime((int)brush.mousePositionOnGrid.y);

        string text;

        if (Selection.selectedHitObjects.Count == 0)
        {
            text = $"x: {brushCoordsVec.x}\tTimestamp: ";
            text += $"{StringFormatter.GetTimeFormat(TimeLine.CurrentTimeStamp)}";
        }
        else
        {
            text = $"x: {Selection.last.position.x}\tTimestamp: ";
            text += $"{StringFormatter.GetTimeFormat(Selection.last.position.y)}";

            HitObject previousHitObject = HitObjectManager.GetPreviousFruit(Selection.GetFirstFruit());
            HitObject nexHitObject = HitObjectManager.GetNextFruit(Selection.GetLastFruit());

            string prev = "Prev: ";
            string next = "Next: ";

            if (previousHitObject != null)
                prev += $"{previousHitObject.position.x - Selection.last.position.x}";
            if (nexHitObject != null)
                next += $"{nexHitObject.position.x - Selection.GetLastFruit().position.x}";

            text += "\t" + prev + ", " + next;
        }

        textMesh.text = text;
    }
}
