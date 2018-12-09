using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Beatmap
{
    public List<HitObject> hitObjects = new List<HitObject>();
    public float BPM = 180; //default to 180 bpm to avoid possible bugs with 0 BPM
}
