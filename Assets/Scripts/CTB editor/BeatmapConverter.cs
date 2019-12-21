using System;
using System.Collections.Generic;
using System.Linq;
using OsuParsers.Beatmaps;
using OsuParsers.Beatmaps.Objects;
using OsuParsers.Beatmaps.Objects.Catch;
using OsuParsers.Decoders;
using OsuParsers.Enums;
using OsuParsers.Enums.Beatmaps;
using UnityEngine;
using PHitObject = OsuParsers.Beatmaps.Objects.HitObject;
using PSlider = OsuParsers.Beatmaps.Objects.Slider;

public static class BeatmapConverter
{
    private const int version = 12;

    private static List<string> lines = new List<string>();

    public static Beatmap importedBeatmap = null;

    public static Beatmap CreateBeatmapData()
    {
        //if (importedBeatmap != null)
        //    return importedBeatmap;

        Beatmap beatmap = BeatmapDecoder.Decode(Application.streamingAssetsPath + "/template.osu");

        beatmap.GeneralSection.AudioFilename = BeatmapSettings.audioFileName;
        beatmap.GeneralSection.Mode = Ruleset.Fruits;

        beatmap.DifficultySection.ApproachRate = BeatmapSettings.AR;
        beatmap.DifficultySection.CircleSize = BeatmapSettings.CS;

        TimingPoint timingPoint = new TimingPoint();
        timingPoint.BeatLength = 60000f / BeatmapSettings.BPM;
        beatmap.TimingPoints.Add(timingPoint);

        List<PHitObject> pHitObjects = new List<PHitObject>();

        List<Fruit> fruitsAndSliderFruits = HitObjectManager.GetFruits();
        List<Fruit> fruits = fruitsAndSliderFruits.Where(i => !i.isSliderFruit).ToList();

        foreach (Fruit fruit in fruits)
        {
            var position = (Vector2Int.right * fruit.position).ToNumerical();
            int hitTime = fruit.position.y;

            HitSoundType hitSoundType = HitSoundType.None;
            Extras extras = new Extras();

            CatchFruit addFruit = new CatchFruit(position, hitTime, hitTime, hitSoundType, extras, false, 0);
            pHitObjects.Add(addFruit);
        }

        foreach (Slider slider in HitObjectManager.GetSliders())
        {
            var position = (Vector2Int.right * slider.GetFruitByIndex(0).position).ToNumerical();
            int startTime = slider.startTime;
            int endTime = slider.endTime;
            var sliderPoints = slider.GetSliderPoints();
            //TEST
            sliderPoints[1] = new System.Numerics.Vector2(sliderPoints[1].X,0);

            var pixelLength = Math.Abs(sliderPoints[1].X - sliderPoints[0].X);
            var hitsoundType = new List<HitSoundType>();
            var edgeAdditions = new List<Tuple<SampleSet, SampleSet>>();
            Extras extras = new Extras();

            sliderPoints.Remove(sliderPoints[0]);
            PSlider pSlider = new PSlider(position, startTime, endTime, HitSoundType.None, CurveType.Linear,
                sliderPoints, 0, pixelLength, hitsoundType, edgeAdditions, extras, true, 0);

            pHitObjects.Add(pSlider);
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
                HitObjectManager.CreateFruitByData(new Vector2(hitobject.Position.X, hitobject.StartTime));
            else if (hitobject is PSlider)
            {
                PSlider slider = (PSlider)hitobject;
                if (slider.CurveType != CurveType.Linear) return;
                HitObjectManager.CreateSliderByData(slider.Position.X, slider.StartTime, slider.SliderPoints, slider.Repeats);
            }
        }
    }

    public static void WriteOsuFile(string path)
    {
        Beatmap beatmap = CreateBeatmapData();
        beatmap.EditorSection.BeatDivisor = BeatsnapDivisor.Instance.division;
        beatmap.Write(Application.streamingAssetsPath + "/Exported CTB map.osu");
        beatmap.Write(path);
    }
}
