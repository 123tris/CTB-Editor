using System.Collections.Generic;
using System.IO;
using System.Linq;
using OsuParsers.Beatmaps;
using OsuParsers.Beatmaps.Objects;
using OsuParsers.Beatmaps.Objects.Catch;
using OsuParsers.Decoders;
using OsuParsers.Enums;
using OsuParsers.Enums.Beatmaps;
using UnityEngine;
using PHitObject = OsuParsers.Beatmaps.Objects.HitObject;

public static class BeatmapConverter
{
    private const int version = 12;

    private static List<string> lines = new List<string>();

    public static Beatmap importedBeatmap = null;

    public static Beatmap CreateBeatmapData()
    {
        //if (importedBeatmap != null)
        //    return importedBeatmap;

        Beatmap beatmap = BeatmapDecoder.Decode(Application.streamingAssetsPath+"/template.osu");

        beatmap.GeneralSection.AudioFilename = BeatmapSettings.audioFileName;
        beatmap.GeneralSection.Mode = Ruleset.Fruits;

        beatmap.DifficultySection.ApproachRate = BeatmapSettings.AR;
        beatmap.DifficultySection.CircleSize = BeatmapSettings.CS;

        TimingPoint timingPoint = new TimingPoint();
        timingPoint.BeatLength = 60000f / BeatmapSettings.BPM;
        beatmap.TimingPoints.Add(timingPoint);

        List<PHitObject> pHitObjects = new List<PHitObject>();
        foreach (HitObject hitObject in HitObjectManager.GetHitObjects())
        {
            PHitObject addHitObject;
            var position = (Vector2Int.right * hitObject.position).ToNumerical();
            var hitTime = hitObject.position.y;

            if (hitObject is Fruit)
            {
                //Add fruit
                addHitObject = new CatchFruit(position, hitTime, hitTime, HitSoundType.None, null, false, 0);
            }
            else
            {
                //Add Slider TODO: Slider logic
                addHitObject = null;
            }

            pHitObjects.Add(addHitObject);
        }
        beatmap.HitObjects = pHitObjects;

        return beatmap;
    }

    public static void ImportBeatmap(string path)
    {
        importedBeatmap = BeatmapDecoder.Decode(path);
        LoadImportedBeatmap();
    }

    private static void LoadImportedBeatmap()
    {
        HitObjectManager.Reset();
        BeatmapSettings.audioFileName = importedBeatmap.GeneralSection.AudioFilename;

        //BeatsnapDivisor.Instance.SetDivision(importedBeatmap.EditorSection.BeatDivisor);

        BeatmapSettings.AR = importedBeatmap.DifficultySection.ApproachRate;
        BeatmapSettings.CS = importedBeatmap.DifficultySection.CircleSize;
        BeatmapSettings.BPM = (float)(60000f / importedBeatmap.TimingPoints.First().BeatLength);

        foreach (PHitObject hitobject in importedBeatmap.HitObjects)
        {
            if (hitobject is Circle)
                HitObjectManager.CreateFruitByData(new Vector2(hitobject.Position.X,hitobject.StartTime));
        }
    }

    public static void WriteOsuFile(string path)
    {
        Beatmap beatmap = CreateBeatmapData();
        beatmap.EditorSection.BeatDivisor = BeatsnapDivisor.Instance.division;
        beatmap.Write(Application.streamingAssetsPath + "/Exported CTB map.osu");
    }
}
