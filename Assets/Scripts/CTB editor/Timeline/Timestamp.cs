public class Timestamp : TextBehaviour
{
    void Update()
    {
        textMesh.text = StringFormatter.GetTimeFormat(TimeLine.CurrentTimeStamp);
    }
}