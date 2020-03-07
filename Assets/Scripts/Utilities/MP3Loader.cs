using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NAudio.Wave;
using NLayer;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

public static class MP3Loader
{
    public static AudioClip LoadMP3(string filePath)
    {
        string filename = Path.GetFileNameWithoutExtension(filePath);

        Mp3FileReader reader = new Mp3FileReader(filePath);
        float[] audioData = new float[reader.Length];
        ISampleProvider sampleProvider = reader.ToSampleProvider();
        sampleProvider.Read(audioData, 0, audioData.Length);

        AudioClip ac = AudioClip.Create(filename,
            (int)reader.Length,
            reader.WaveFormat.Channels,
            reader.WaveFormat.SampleRate,
            false);

        ac.SetData(audioData, 0);
        return ac;
    }
}