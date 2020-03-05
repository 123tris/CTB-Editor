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
using NVector2 = System.Numerics.Vector2;

public static class BeatmapConverter
{
    private const int version = 12;

    private static List<string> lines = new List<string>();

    public static Beatmap importedBeatmap = null;

    public static Beatmap CreateBeatmapData()
    {
        Beatmap beatmap = BeatmapDecoder.Decode(Application.streamingAssetsPath + "/template.osu");

        //General
        beatmap.GeneralSection.AudioFilename = BeatmapSettings.audioFileName + ".mp3";
        beatmap.GeneralSection.Mode = Ruleset.Fruits;

        //Meta data
        beatmap.MetadataSection.Artist = "Rick Astley";
        beatmap.MetadataSection.Title = BeatmapSettings.audioFileName;


        //Difficulty
        beatmap.DifficultySection.ApproachRate = BeatmapSettings.AR;
        beatmap.DifficultySection.CircleSize = BeatmapSettings.CS;
        beatmap.DifficultySection.HPDrainRate = BeatmapSettings.HP;

        TimingPoint timingPoint = new TimingPoint();
        timingPoint.BeatLength = 60000f / BeatmapSettings.BPM;
        beatmap.TimingPoints.Add(timingPoint);

        List<PHitObject> pHitObjects = new List<PHitObject>();

        List<Fruit> fruits = HitObjectManager.GetNonSliderFruits();

        //Load fruit data

        foreach (Fruit fruit in fruits)
        {
            var position = (Vector2Int.right * fruit.position).ToNumerical();
            int hitTime = fruit.position.y;

            HitSoundType hitSoundType = HitSoundType.None;
            Extras extras = new Extras();

            CatchFruit addFruit = new CatchFruit(position, hitTime, hitTime, hitSoundType, extras, true, 0);
            pHitObjects.Add(addFruit);
        }

        //Load slider data

        foreach (Slider slider in HitObjectManager.GetSliders())
        {
            int startTime = slider.startTime;
            int endTime = slider.endTime;

            float xDelta = Mathf.Abs(slider.fruits[1].position.x - slider.fruits[0].position.x);

            float sliderMultiplier = (float) beatmap.DifficultySection.SliderMultiplier;
            float xTime = xDelta * sliderMultiplier / 100 * BeatmapSettings.BPS;
            float timeDifference = slider.fruits[1].position.y - slider.fruits[0].position.y;

            //Change sv
            TimingPoint sliderTimingPoint = new TimingPoint();
            sliderTimingPoint.Offset = startTime;
            sliderTimingPoint.Inherited = true;
            sliderTimingPoint.BeatLength = timeDifference/xTime;
            beatmap.TimingPoints.Add(sliderTimingPoint);

            var hitsoundType = new List<HitSoundType>();
            var edgeAdditions = new List<Tuple<SampleSet, SampleSet>>();
            Extras extras = new Extras();

            NVector2 position = new NVector2(startTime,0);
            var sliderPoints = slider.GetSliderPoints();
            sliderPoints.RemoveAt(0);

            PSlider pSlider = new PSlider(position, startTime, endTime, HitSoundType.None, CurveType.Linear,
                sliderPoints, 0, xDelta, hitsoundType, edgeAdditions, extras, true, 0);

            pHitObjects.Add(pSlider);
        }


        beatmap.HitObjects = pHitObjects;

        return beatmap;
    }

    public static Beatmap ImportBeatmap(string path)
    {
        importedBeatmap = BeatmapDecoder.Decode(path);
        LoadImportedBeatmap();
        return importedBeatmap;
    }

    private static void LoadImportedBeatmap()
    {
        HitObjectManager.Reset();
        BeatmapSettings.audioFileName = importedBeatmap.GeneralSection.AudioFilename;

        //TODO: import beat divisor
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
                //HitObjectManager.CreateSliderByData(slider.Position.X, slider.StartTime, slider.SliderPoints, slider.Repeats);
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
