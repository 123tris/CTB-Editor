using UnityEngine;

public static class BeatmapSettings
{
    public static float AR;
    public static float CS;
    public static float HP;
    public static float BPM;

    ///<summary>BPM start offset in milliseconds</summary>
    public static int BPMOffset;

    public static string audioFileName;

    ///<summary>Beats per second</summary>
    public static float BPS => BPM / 60;


    [RuntimeInitializeOnLoadMethod]
    static void Reset()
    {
        AR = 9;
        CS = 4;
        HP = 6;
        BPM = 180;
        BPMOffset = 0;
    }
}
