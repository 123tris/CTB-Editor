using UnityEngine;

public static class ClickManager
{
    //property variables
    private const float maxTimeToClick = 0.60f;
    private const float minTimeToClick = 0.05f;

    //private variables to keep track
    private static float _minCurrentTime;
    private static float _maxCurrentTime;

    public static bool DoubleClick()
    {
        if (Time.time >= _minCurrentTime && Time.time <= _maxCurrentTime)
        {
            _minCurrentTime = 0;
            _maxCurrentTime = 0;
            return true;
        }
        _minCurrentTime = Time.time + minTimeToClick;
        _maxCurrentTime = Time.time + maxTimeToClick;
        return false;
    }
}