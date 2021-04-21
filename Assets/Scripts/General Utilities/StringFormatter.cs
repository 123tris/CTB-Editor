using System;

public static class StringFormatter
{
    ///<summary>Expects time in milliseconds. Will return a formatted string that is split up in minutes, seconds, and milliseconds</summary>
    public static string GetTimeFormat(int time)
    {
        TimeSpan timeSpan = TimeSpan.FromMilliseconds(time);

        return $"{timeSpan.Minutes:00}:{timeSpan.Seconds:00}:{timeSpan.Milliseconds:000}";
    }
}
