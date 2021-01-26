using System;
using NAudio.Wave;
using SFB;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class NAudioPlayer : MonoBehaviour
{
    public float currentTime => timer.elapsedTime;
    public float clipLength => (float)audioFile.TotalTime.TotalSeconds;

    public bool isPlaying => outputDevice.PlaybackState == PlaybackState.Playing;

    AudioFileReader audioFile;
    WaveOutEvent outputDevice = new WaveOutEvent();

    public readonly Timer timer = new Timer();

    void Start()
    {
        string filepath = "G:\\Repos\\CTB-Editor\\Assets\\Audio\\96Neko_-_Tapioca_Sennou_Song_-memoReal_ver.mp3";
        SetSong(filepath);
    }

    public void SetSong(string filepath)
    {
        audioFile = new AudioFileReader(filepath) { Volume = .05f };

        outputDevice.Stop();
        outputDevice.Init(audioFile);
    }

    private void OnDestroy()
    {
        outputDevice.Stop();
    }

    public void Play()
    {
        outputDevice.Play();
        timer.Start();
    }

    public void Pause()
    {
        outputDevice.Pause();
        timer.Pause();
    }

    public void Stop()
    {
        outputDevice.Stop();
        timer.Stop();
    }

    public void SetPlayback(float playbackTime)
    {
        audioFile.CurrentTime = TimeSpan.FromSeconds(playbackTime);
        timer.SetTime(playbackTime);
    }

    public void SetVolume(float volume)
    {
        audioFile.Volume = volume;
    }
}

//Exists for pure testing purposes
#if UNITY_EDITOR
[CustomEditor(typeof(NAudioPlayer))]
[CanEditMultipleObjects]
public class NAudioPlayerEditor : Editor
{
    private float time;

    public override void OnInspectorGUI()
    {
        NAudioPlayer player = (NAudioPlayer)target;

        if (GUILayout.Button("Play"))
            player.Play();
        if (GUILayout.Button("Pause"))
            player.Pause();
        if (GUILayout.Button("Stop"))
            player.Stop();

        if (GUILayout.Button("Load song"))
        {
            string[] path = StandaloneFileBrowser.OpenFilePanel("Select song", "", "mp3", false);
            if (path.Length != 0) player.SetSong(path[0]);
        }

        time = EditorGUILayout.FloatField("Time",time);
        if (GUILayout.Button("Set time"))
        {
            player.SetPlayback(time);
        }

        GUILayout.Label($"Timer elapsed time: {player.timer.elapsedTime}");
        GUILayout.Label($"Timer start time: {player.timer.startTime}");
    }
}
#endif