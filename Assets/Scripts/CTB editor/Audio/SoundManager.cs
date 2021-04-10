using System;
using System.Collections.Generic;
using NAudio.Wave;
using OsuParsers.Enums.Beatmaps;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioClip normalHitSound;
    [SerializeField] private AudioClip whistleHitSound;
    [SerializeField] private AudioClip finishHitSound;
    [SerializeField] private AudioClip clapHitSound;

    [SerializeField] private AudioClip hitObjectClickSound;

    private AudioSource audioSource;
    private static SoundManager Instance;

    private List<int> scheduledHitsounds = new List<int>();

    //Hitsound settings
    private float[] hitsoundSamples;
    private int sampleRate;
    //When play is pressed
    private float startTime;
    private AudioBuffer hitsoundBuffer;

    //Keep track of playback time, like some sort of music
    private int currentTime;

    void Awake()
    {
        Instance = this;

        //string hitObjectClickPath = Application.streamingAssetsPath + "/HitobjectClick.wav";
        //soundPool = new SoundPool(hitObjectClickPath);
        audioSource = GetComponent<AudioSource>();

        hitsoundSamples = new float[normalHitSound.samples * normalHitSound.channels];
        normalHitSound.GetData(hitsoundSamples, 0);
        hitsoundBuffer = new AudioBuffer(normalHitSound.channels, hitsoundSamples, normalHitSound.frequency);
        sampleRate = AudioSettings.outputSampleRate;
        currentTime = 0;
    }

    void Update()
    {
        //string time = StringFormatter.GetTimeFormat((int) (currentTime * 1000 / (float) sampleRate));
        //print($"current time: {time}");
    }

    public static void PlayHitSound(HitSoundType hitSound)
    {
        //if (hitSound != HitSoundType.None)
        Instance.audioSource.PlayOneShot(Instance.normalHitSound);
        //Instance.soundPool.PlaySound();
        //switch (hitSound)
        //{
        //    case HitSoundType.Normal:
        //        Instance.audioSource.PlayOneShot(Instance.normalHitSound);
        //        break;
        //    case HitSoundType.Whistle:
        //        Instance.audioSource.PlayOneShot(Instance.whistleHitSound);
        //        break;
        //    case HitSoundType.Finish:
        //        Instance.audioSource.PlayOneShot(Instance.finishHitSound);
        //        break;
        //    case HitSoundType.Clap:
        //        Instance.audioSource.PlayOneShot(Instance.clapHitSound);
        //        break;
        //}
    }

    public static void ScheduleHitsounds()
    {
        List<Fruit> fruits = HitObjectManager.GetFruits();

        Instance.scheduledHitsounds = new List<int>(fruits.Count);

        for (var i = 0; i < fruits.Count; i++)
        {
            Fruit fruit = fruits[i];

            float currentTime = MusicPlayer.instance.currentTime * 1000;
            if (fruit.position.y <= currentTime) continue;

            double scheduleTime = (fruit.position.y - currentTime) / MusicPlayer.playbackSpeed / 1000d;
            Instance.scheduledHitsounds.Add((int)(scheduleTime * Instance.sampleRate));
        }
    }

    public static void CancelSchedule() => Instance.scheduledHitsounds.Clear();

    private int scheduledHitsound; //index of the scheduled hitsound

    private void OnAudioFilterRead(float[] data, int channels)
    {
        AudioBuffer outputBuffer = new AudioBuffer(channels, data, sampleRate);

        //The time it takes to play a hit sound
        int soundToPlayLength = hitsoundSamples.Length;
        var i = scheduledHitsound;
        currentTime += outputBuffer.sampleCount;

        if (scheduledHitsounds.Count == 0) return;
        if (currentTime >= scheduledHitsounds[i]) //Play scheduled hit sound
        {
            //Reschedule
            bool finishedPlayingSound = currentTime - soundToPlayLength >= scheduledHitsounds[i];
            bool nextIndexOutOfBounds = i + 1 >= scheduledHitsounds.Count;
            bool scheduleNextSound = nextIndexOutOfBounds || currentTime >= scheduledHitsounds[i + 1];

            if (finishedPlayingSound || scheduleNextSound)
            {
                if (scheduledHitsound < scheduledHitsounds.Count - 1)
                {
                    scheduledHitsound++;
                }
            }

            if (currentTime < scheduledHitsounds[scheduledHitsound]) return;


            //Play scheduled sound
            double sampleIndex = currentTime - scheduledHitsounds[scheduledHitsound];
            double hitsoundSR = hitsoundBuffer.sampleRate / (double)outputBuffer.sampleRate;
            for (int j = 0; j < outputBuffer.sampleCount; j++)
            {
                for (int k = 0; k < hitsoundBuffer.channels || k < outputBuffer.channels; k++)
                {
                    float finalSample = hitsoundBuffer.LinearSample((float)sampleIndex, k);
                    outputBuffer.SetSampleForChannel(j, k, finalSample);
                }
                sampleIndex += hitsoundSR;
            }
        }

    }

    public static void PlaySound(AudioClip clip) => Instance.audioSource.PlayOneShot(clip);

    ///<summary>Set time in seconds</summary>
    public static void SetTime(double time)
    {
        Instance.currentTime = (int)(time * Instance.sampleRate);

        var scheduledHitsounds = Instance.scheduledHitsounds;
        for (var i = 0; i < scheduledHitsounds.Count; i++)
        {
            if (scheduledHitsounds[i] >= Instance.currentTime)
            {
                Instance.scheduledHitsound = i;
                break;
            }
        }
    }

    public static void PlayHitObjectClickSound()
    {
        Instance.audioSource.PlayOneShot(Instance.hitObjectClickSound);
    }
}
