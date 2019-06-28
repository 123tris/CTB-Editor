using System.Collections.Generic;

public class Beatmap
{
    public List<HitObject> hitObjects = new List<HitObject>();
    public float BPM = 180; //default to 180 bpm to avoid possible bugs with 0 BPM
}
