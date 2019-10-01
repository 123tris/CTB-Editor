using NLayer;
using UnityEngine;

public static class MP3Loader
{
    public static AudioClip LoadMP3(string filePath)
    {
        string filename = System.IO.Path.GetFileNameWithoutExtension(filePath);

        MpegFile mpegFile = new MpegFile(filePath);

        // assign samples into AudioClip
        AudioClip ac = AudioClip.Create(filename,
            (int)(mpegFile.Length / sizeof(float) / mpegFile.Channels),
            mpegFile.Channels,
            mpegFile.SampleRate,
            false,
            data => { int actualReadCount = mpegFile.ReadSamples(data, 0, data.Length); },
            position => { mpegFile = new MpegFile(filePath); });

        return ac;
    }
}