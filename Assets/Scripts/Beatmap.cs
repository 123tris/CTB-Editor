//using System.Collections.Generic;
//using System.Numerics;
//using OsuParsers.Beatmaps.Objects;
//using OsuParsers.Enums;
//using OsuParsers.Enums.Beatmaps;
//using UnityEngine;
//using PHitObject = OsuParsers.Beatmaps.Objects.HitObject;

//public class Beatmap //TODO: should be updated so that the beatmap converter relies purely on the data provided in Beatmap.cs
//{
//    public List<HitObject> hitObjects = new List<HitObject>();
//    public float BPM = 180; //default to 180 bpm to avoid possible bugs with 0 BPM
//    public float AR = 9;
//    public float CS = 4;
//    public string audioFileName;

//    public OsuParsers.Beatmaps.Beatmap GetParseData()
//    {
//        OsuParsers.Beatmaps.Beatmap beatmapData = new OsuParsers.Beatmaps.Beatmap();

//        beatmapData.GeneralSection.AudioFilename = audioFileName;
//        beatmapData.GeneralSection.Mode = Ruleset.Fruits;

//        beatmapData.DifficultySection.ApproachRate = AR;
//        beatmapData.DifficultySection.CircleSize = CS;

//        TimingPoint timingPoint = new TimingPoint();
//        timingPoint.BeatLength = Mathf.RoundToInt(60000f / BPM);
//        beatmapData.TimingPoints.Add(timingPoint);

//        List<PHitObject> pHitObjects = new List<PHitObject>();
//        foreach (HitObject hitObject in hitObjects)
//        {
//            var addHitObject = new PHitObject();
//            addHitObject.Position = hitObject.position.ToNumerical();
//            pHitObjects.Add(addHitObject);
//        }
//        beatmapData.HitObjects = pHitObjects;

//        return beatmapData;
//    }
//}
