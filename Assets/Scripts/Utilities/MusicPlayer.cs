using System;
using System.Collections;
using System.IO;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        TimeLine.instance.SetTimeLineLength((int) (audioSource.clip.length*1000));
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
            audioSource.Pause();
        }

        BeatmapSettings.audioFileName = audioSource.clip.name + ".mp3";
    }

    void LateUpdate()
    {
        if (TimeLine.instance == null) return;

        if (Math.Abs(Input.mouseScrollDelta.y) > 0.1f)
            audioSource.time = Mathf.Clamp(audioSource.time + Input.mouseScrollDelta.y / 100 * TimeLine.instance.scrollSpeed,0,float.MaxValue);
        TimeLine.instance.SetCurrentTimeStamp((int) (audioSource.time * 1000));
    }

    public void SetSong(string filepath)
    {
        AudioClip audioClip = MP3Loader.LoadMP3(filepath);
        audioSource.clip = audioClip;
        TimeLine.instance.SetTimeLineLength((int) (audioClip.length*1000)); //Multiply with a 1000 to convert from seconds to milliseconds
    }

    public void SetPlayback(float playbackMS)
    {
        audioSource.time = playbackMS/1000f;
    }

    public void PlaySong()
    {
        print("Pressed play, AudioSource time: "+audioSource.time);
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

}