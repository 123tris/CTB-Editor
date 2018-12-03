using System.Collections;
using System.IO;
using NAudio.Wave;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    private AudioSource audioSource;
    private AudioClip audioClip;
    private Mp3FileReader mp3Reader;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void LateUpdate()
    {
        TimeLine.instance.SetCurrentTimeStamp((int) (audioSource.time * 1000));
    }

    public void SetSong(string filepath)
    {
        //mp3Reader = new Mp3FileReader(filepath);
        StartCoroutine(LoadAudioClip(File.ReadAllBytes(filepath),filepath));
    }

    public void SetPlayback(float playbackMS)
    {
        audioSource.time = playbackMS/1000f;
    }

    private IEnumerator LoadAudioClip(byte[] rawData,string filepath)
    {
        //string tempFile = Application.persistentDataPath + "/bytes.ogg";
        //File.WriteAllBytes(tempFile, rawData);

        WWW loader = new WWW("file://" + filepath);
        yield return loader;
        if (!System.String.IsNullOrEmpty(loader.error))
            Debug.LogError(loader.error);

        audioClip = loader.GetAudioClip();
        audioSource.clip = audioClip;
        TimeLine.instance.SetTimeLineLength((int) (audioClip.length*1000)); //Multiply with a 1000 to convert from seconds to milliseconds
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
        audioSource.Stop();
        SetPlayback(0);
    }

    public void ResumeSong()
    {
        audioSource.UnPause();
    }

}