using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class MusicPlayer : MonoBehaviour
{
    private AudioSource audioSource;

    public float audioSourceTime;

    public float mouseScrollDelta;

    public static MusicPlayer instance;

    void Awake() => instance = this;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        TimeLine.instance.SetTimeLineLength((int)(audioSource.clip.length * 1000));
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
            audioSource.Pause();
        }

        BeatmapSettings.audioFileName = audioSource.clip.name;
    }

    void LateUpdate()
    {
        mouseScrollDelta = Input.mouseScrollDelta.y;
        if (TimeLine.instance == null) return;

        if (Math.Abs(Input.mouseScrollDelta.y) > 0.1f)
        {
            float beatsPerMS = (BeatmapSettings.BPM / 60 /*Beats per second*/) * 1000;
            float scrollDistance = Input.mouseScrollDelta.y * TimeLine.instance.scrollSpeed / (beatsPerMS * BeatsnapDivisor.Instance.division);
            //print($"Scroll {scrollDistance}ms\nTarget time: {audioSource.time * 1000+scrollDistance}ms\nAudio clip length: {audioSource.clip.length*1000}");
            audioSource.time = Mathf.Max(audioSource.time + scrollDistance, 0);
        }

        TimeLine.instance.SetCurrentTimeStamp(Mathf.RoundToInt(audioSource.time * 1000));

        audioSourceTime = audioSource.time;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (audioSource.isPlaying) PauseSong();
            else ResumeSong();
        }
    }

    public void SetSong(string filepath)
    {
        var audioClip = MP3Loader.LoadMP3(filepath);
        audioSource.clip = audioClip;
        audioSource.Play();
        audioSource.Pause();
        TimeLine.instance.SetTimeLineLength((int)(audioClip.length * 1000)); //Multiply with a 1000 to convert from seconds to milliseconds
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
    }
}