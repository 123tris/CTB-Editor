using System;
using System.Collections.Generic;
using NaughtyAttributes;
using OsuParsers.Enums.Beatmaps;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [Header("Audio clips")]
    [SerializeField] private AudioClip normalHitSound;
    [SerializeField] private AudioClip whistleHitSound;
    [SerializeField] private AudioClip finishHitSound;
    [SerializeField] private AudioClip clapHitSound;

    [SerializeField] private AudioClip hitObjectClickSound;

    [Header("Settings")]
    [SerializeField] float audioSourcePoolSize = 5;
    public float volume = 0.1f;

    public int scheduledHitsoundIndex; //index of the scheduled hitsound

    private static SoundManager Instance;

    public List<ScheduledHitsound> scheduledHitsounds = new List<ScheduledHitsound>();

    private List<AudioSource> audioSources = new List<AudioSource>();

    private double currentTime => TimeLine.CurrentTimeStamp;

    void Awake()
    {
        Instance = this;

        for (int i = 0; i < audioSourcePoolSize; i++)
        {
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSources.Add(audioSource);
            audioSource.playOnAwake = false;
            audioSource.Pause();
            audioSource.clip = normalHitSound;
            audioSource.volume = volume;
        }
    }

    void Update()
    {
        if (scheduledHitsoundIndex >= scheduledHitsounds.Count || scheduledHitsounds.Count == 0)
            return;

        if (TimeLine.CurrentTimeStamp > scheduledHitsounds[scheduledHitsoundIndex - 1].hitTime * 1000)
        {
            ScheduleNextHitSound();
        }
    }

    double GetTrackTime(AudioSource audioSource)
    {
        return audioSource.timeSamples / (double)audioSource.clip.frequency;
    }

    void ScheduleNextHitSound()
    {
        if (scheduledHitsoundIndex >= scheduledHitsounds.Count)
            return;

        for (int i = 0; i < audioSources.Count; i++)
        {
            AudioSource audioSource = audioSources[i];
            //print($"Scheduling next hitsound audio source index: {i}, track time: {GetTrackTime(audioSource)}");
            if (audioSource.isPlaying && GetTrackTime(audioSource) < 0.2f)
                continue;

            var scheduledHitsound = scheduledHitsounds[scheduledHitsoundIndex];
            double scheduledTime = (scheduledHitsound.hitTime - currentTime/1000) / MusicPlayer.playbackSpeed;

            //print($"Schedule hitsound for audio source index: {i}, scheduled {scheduledHitsoundIndex} for {scheduledTime}");

            audioSource.clip = GetHitSoundClip(scheduledHitsound.hitSoundType);
            audioSource.PlayScheduled(AudioSettings.dspTime + scheduledTime);
            break;
        }

        scheduledHitsoundIndex++;
    }

    AudioClip GetHitSoundClip(HitSoundType hitSoundType)
    {
        if (hitSoundType == HitSoundType.Normal)
            return normalHitSound;
        if (hitSoundType.HasFlag(HitSoundType.Whistle))
            return whistleHitSound;
        if (hitSoundType.HasFlag(HitSoundType.Finish))
            return finishHitSound;
        if (hitSoundType.HasFlag(HitSoundType.Clap))
            return clapHitSound;

        return normalHitSound;
    }

    public static void ScheduleHitsounds()
    {
        List<Fruit> fruits = HitObjectManager.GetFruits();

        Instance.scheduledHitsounds = new List<ScheduledHitsound>(fruits.Count);
        Instance.scheduledHitsoundIndex = -1;

        double currentTime = TimeLine.CurrentTimeStamp;

        for (var i = 0; i < fruits.Count; i++)
        {
            Fruit fruit = fruits[i];

            Instance.scheduledHitsounds.Add(new ScheduledHitsound(fruit.position.y / 1000d, fruit.hitSound)); //convert to seconds

            if (currentTime < fruit.position.y && Instance.scheduledHitsoundIndex == -1)
            {
                Instance.scheduledHitsoundIndex = i;
                Instance.ScheduleNextHitSound();
            }

            //double scheduleTime = (fruit.position.y - currentTime) / MusicPlayer.playbackSpeed / 1000d;
            //Instance.scheduledHitsounds.Add((int)(scheduleTime * Instance.sampleRate));
        }
    }

    public static void CancelSchedule()
    {
        foreach (AudioSource audioSource in Instance.audioSources)
        {
            audioSource.Stop();
        }
        Instance.scheduledHitsounds.Clear();
    }

    ///<summary>Set time in seconds</summary>
    public static void SetTime(double time)
    {
        ScheduleHitsounds();
    }

    public static void PlaySound(AudioClip clip)
    {
        if (Instance.audioSources.Count == 0)
        {
            Debug.LogError("Attempting to play a sound but is unable to do so due to the sound manager not having loaded the audiosources in yet", Instance);
            return;
        }
        Instance.audioSources[0].PlayOneShot(clip);
    }

    public static void PlayHitObjectClickSound()
    {
        PlaySound(Instance.hitObjectClickSound);
    }
}

public class ScheduledHitsound
{
    ///<summary>hit time of the hitsound in seconds</summary>
    public double hitTime;
    public HitSoundType hitSoundType;

    public ScheduledHitsound(double scheduleTime, HitSoundType fruitHitSound)
    {
        hitTime = scheduleTime;
        hitSoundType = fruitHitSound;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(SoundManager))]
public class SoundManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        SoundManager soundManager = (SoundManager)target;

        soundManager.scheduledHitsoundIndex = EditorGUILayout.IntField("Scheduled hitsound index", soundManager.scheduledHitsoundIndex);

        for (int i = 0; i < soundManager.scheduledHitsounds.Count; i++)
        {
            var scheduledHitsound = soundManager.scheduledHitsounds[i];
            var label = $"Hitobject #{i + 1} Scheduled for:\nHit time: {scheduledHitsound.hitTime}\t Hit type: {scheduledHitsound.hitSoundType.ToString()}";
            EditorGUILayout.TextArea(label);
        }
    }
}
#endif