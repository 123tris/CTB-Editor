using UnityEngine;

public class AudioBuffer
{
    public readonly int channels;
    private readonly float[] samples;
    public readonly int sampleRate;

    public AudioBuffer(int channels, float[] samples, int sampleRate)
    {
        this.channels = channels;
        this.samples = samples;
        this.sampleRate = sampleRate;
        sampleCount = samples.Length / channels;
    }

    public float GetSampleForChannel(int channelIndex, int sampleIndex)
    {
        var index = sampleIndex * channels + channelIndex;
        if (index < samples.Length)
            return samples[index];
        else
            return 0;
    }

    public void SetSampleForChannel(int sampleIndex, int channelIndex, float sample)
    {
        samples[sampleIndex * channels + channelIndex] = sample;
    }

    public float LinearSample(float sampleIndex, int channel)
    {
        float start = GetSampleForChannel(channel, (int)sampleIndex);
        float target = GetSampleForChannel(channel, (int)(sampleIndex + 1));
        float delta = sampleIndex % 1;
        return Mathf.Lerp(start, target, delta);
    }

    public int sampleCount { get; }

    public override string ToString()
    {
        string output = "Samples: {";
        foreach (var sample in samples)
            output += sample + ",\t";
        output += "}";
        return output;
    }
}
