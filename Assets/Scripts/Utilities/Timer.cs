using System;
using UnityEngine;

public class Timer
{
    ///<summary>The amount of time elapsed in seconds</summary>
    public float elapsedTime => GetElapsedTime();

    ///<summary>At what time in the application the timer started in seconds. (Set by Time.time)</summary>
    public float startTime = -1;

    private float elapsedTimeOnPause = 0;

    private bool paused = true;

    private float GetElapsedTime()
    {
        if (startTime == -1) return 0;

        if (paused) return elapsedTimeOnPause;

        return Time.time - startTime;
    }

    public void Start()
    {
        startTime = Time.time;
        if (paused)
            startTime -= elapsedTimeOnPause;
        paused = false;
    }

    public void Pause()
    {
        elapsedTimeOnPause = elapsedTime;
        paused = true;
    }

    public void Stop()
    {
        paused = true;
        elapsedTimeOnPause = 0;
    }

    ///<summary>Set time in seconds</summary>
    public void SetTime(float time)
    {
        startTime = elapsedTimeOnPause = Time.time - time;
    }
}
