using System;
using System.Collections.Generic;
using NAudio.Wave;

public class SoundPool
{
    private string soundFilePath;

    private List<AudioFileReader> audioFileReaders;
    private List<WaveOutEvent> outputDevices;

    public SoundPool(string soundFilePath, int poolSize = 20)
    {
        this.soundFilePath = soundFilePath;

        audioFileReaders = new List<AudioFileReader>(poolSize);
        outputDevices = new List<WaveOutEvent>(poolSize);

        for (int i = 0; i < poolSize; i++)
        {
            var audioFileReader = new AudioFileReader(soundFilePath);
            audioFileReaders.Add(audioFileReader);
            var outputDevice = new WaveOutEvent();
            outputDevice.Init(audioFileReader);
            outputDevices.Add(outputDevice); 
        }
    }

    public void PlaySound()
    {
        for (int i = 0; i < audioFileReaders.Count; i++)
        {
            AudioFileReader audioFileReader = audioFileReaders[i];
            //Finished playing sound
            if (audioFileReader.Position == audioFileReader.Length || audioFileReader.Position == 0)
            {
                outputDevices[i].Play();
            }
        }
    }
}
