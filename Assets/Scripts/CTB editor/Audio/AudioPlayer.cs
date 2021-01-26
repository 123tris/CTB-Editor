using System;
using CooldownManagerNamespace;
using UnityEngine;

public class AudioPlayer
{
    private AudioSource audioSource;

    private double trackStartTime;
    private double pausedTime;

    private bool playing;

    public const double SCHEDULE_DELAY = 0.2;

    public AudioPlayer(AudioSource audioSource)
    {
        this.audioSource = audioSource;
    }

    public void Play()
    {
        trackStartTime = AudioSettings.dspTime + SCHEDULE_DELAY; //Add 0.2 seconds to ensure accurate time tracking
        audioSource.PlayScheduled(trackStartTime);
        playing = true;
    }

    public void Resume()
    {
        double trackTime = GetTrackTime();
        audioSource.time = (float)trackTime;

        double elapsedTime = pausedTime - trackStartTime;
        trackStartTime = AudioSettings.dspTime - elapsedTime;
        trackStartTime += SCHEDULE_DELAY; //Add 0.2 seconds for the scheduled delay
        audioSource.PlayScheduled(AudioSettings.dspTime + SCHEDULE_DELAY);
        playing = true;
        Debug.Log("Resume with track time: "+GetTrackTime());
    }

    public void Pause()
    {
        pausedTime = AudioSettings.dspTime + SCHEDULE_DELAY;
        audioSource.SetScheduledEndTime(pausedTime);
        playing = false;

        Debug.Log("Paused at: "+pausedTime+ " with track time of " +GetTrackTime());
    }

    public double GetTrackTime()
    {
        if (!playing)
        {
            return pausedTime - trackStartTime;
        }

        return AudioSettings.dspTime - trackStartTime;
    }

    public void SetPlayback(float playbackTime)
    {
        trackStartTime = AudioSettings.dspTime - playbackTime;
        audioSource.time = playbackTime;
    }

    public void Stop()
    {
        audioSource.Stop();
        playing = false;
    }
}
