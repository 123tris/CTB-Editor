using System.Collections.Generic;

public class Beatmap //TODO: should be updated so that the beatmap converter relies purely on the data provided in Beatmap.cs
{
    public List<HitObject> hitObjects = new List<HitObject>();
    public float BPM = 180; //default to 180 bpm to avoid possible bugs with 0 BPM
    public float AR;
    public float CS;
    public string audioFileName;
}
