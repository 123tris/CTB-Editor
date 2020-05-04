﻿using System;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(NAudioImporter))]
public class MusicPlayer : MonoBehaviour
{
    private AudioSource audioSource;

    public static MusicPlayer instance;
    private AudioMixer audioMixer;
    private AudioImporter audioImporter;

    void Awake()
    {
        instance = this;
        audioSource = GetComponent<AudioSource>();
        AudioMixerGroup mixerGroup = Resources.Load<AudioMixerGroup>("AudioMixer");
        audioMixer = mixerGroup.audioMixer;

        audioImporter = GetComponent<AudioImporter>();
        audioImporter.Loaded += SongLoaded;
    }

    private void Start()
    {
        TimeLine.Instance.SetTimeLineLength((int)(audioSource.clip.length * 1000));
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
            audioSource.Pause();
        }

        BeatmapSettings.audioFileName = audioSource.clip.name;
    }

    void LateUpdate()
    {
        if (TimeLine.Instance == null) return;

        if (Math.Abs(Input.mouseScrollDelta.y) > 0.1f && !Input.GetKey(KeyCode.LeftControl))
        {
            float beatLength = 60 / BeatmapSettings.BPM; //Length of a beat in seconds

            //Scroll the length of a divided beat (dictated by the beatsnap)
            float scrollDistance = Input.mouseScrollDelta.y * TimeLine.Instance.scrollSpeed * beatLength / BeatsnapDivisor.Instance.division;
            
            audioSource.time = Mathf.Clamp(audioSource.time + scrollDistance, BeatmapSettings.BPMOffset / 1000f, audioSource.clip.length);
        }

        TimeLine.Instance.SetCurrentTimeStamp(Mathf.RoundToInt(audioSource.time * 1000));

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (audioSource.isPlaying) PauseSong();
            else ResumeSong();
        }
    }

    public void SetSong(string filepath)
    {
        audioImporter.Import(filepath);
    }

    private void SongLoaded(AudioClip audioClip)
    {
        audioSource.clip = audioClip;
        audioSource.Play();
        audioSource.Pause();
        TimeLine.Instance.SetTimeLineLength((int)(audioClip.length * 1000)); //Multiply with a 1000 to convert from seconds to milliseconds
    }

    public void SetPlayback(float playbackMS)
    {
        audioSource.time = playbackMS / 1000f;
    }

    public void PlaySong()
    {
        print("Pressed play, AudioSource time: " + audioSource.time);
        audioSource.Play();
    }

    public void PauseSong()
    {
        audioSource.Pause();
    }

    public void StopSong()
    {
        SetPlayback(0);
        audioSource.Pause();
    }

    public void ResumeSong()
    {
        audioSource.UnPause();
    }

    //<summary>The volume of the audio source (0.0 to 1.0) </summary>
    public void SetVolume(float volume)
    {
        audioSource.volume = volume;
    }

    public void SetPlaybackSpeed(float sliderValue)
    {
        audioSource.pitch = sliderValue;
        audioMixer.SetFloat("pitch", 1f / sliderValue);
    }
}