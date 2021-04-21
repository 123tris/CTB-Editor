using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(NAudioImporter))]
public class MusicPlayer : MonoBehaviour
{
    public static MusicPlayer instance;

    public bool isPlaying => audioSource.isPlaying || nAudioPlayer.isPlaying;

    public bool useNAudio;

    //temporary
    public static float playbackSpeed = 1;

    private AudioSource audioSource;
    private AudioMixer audioMixer;
    private AudioImporter audioImporter;

    ///<summary>Current time in seconds</summary>
    public float currentTime => useNAudio ? nAudioPlayer.currentTime : audioSource.timeSamples / (float)audioSource.clip.frequency;

    private float clipLength => useNAudio ? nAudioPlayer.clipLength : audioSource.clip.length;

    private NAudioPlayer nAudioPlayer;
    //When play is executed
    [ShowNonSerializedField]
    private double startTime;
    [ShowNonSerializedField]
    private double timePassedAudioThread;

    void Awake()
    {
        instance = this;
        nAudioPlayer = FindObjectOfType<NAudioPlayer>();
        audioSource = GetComponent<AudioSource>();
        AudioMixerGroup mixerGroup = Resources.Load<AudioMixerGroup>("AudioMixer");
        audioMixer = mixerGroup.audioMixer;

        audioImporter = GetComponent<AudioImporter>();
        audioImporter.Loaded += SongLoaded;
    }

    private void Start()
    {
        TimeLine.Instance.SetTimeLineLength((int)(audioSource.clip.length * 1000));
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
            audioSource.Pause();
        }
        SetPlayback(0);
        startTime = AudioSettings.dspTime;

        BeatmapSettings.audioFileName = audioSource.clip.name;
    }

    void Update()
    {
        timePassedAudioThread = AudioSettings.dspTime - startTime;
    }

    void LateUpdate()
    {
        if (TimeLine.Instance == null) return;


        float beatLength = 60 / BeatmapSettings.BPM / BeatsnapDivisor.Instance.division; //Length of a beat in seconds

        if (Math.Abs(Input.mouseScrollDelta.y) > 0.1f && !Input.GetKey(KeyCode.LeftControl))
        {
            float bpmOffset = BeatmapSettings.BPMOffset / 1000f;

            //TODO: FIX When implementing multiple timing points

            float audioTime = audioSource.timeSamples / (float)audioSource.clip.frequency;

            //Scroll the length of a divided beat (dictated by the beatsnap)
            float scrollDistance = Input.mouseScrollDelta.y * TimeLine.Instance.scrollSpeed * beatLength;
            audioTime = Mathf.Clamp(audioTime + scrollDistance, bpmOffset, audioSource.clip.length);

            //Snap time to nearest beat
            audioTime -= bpmOffset;
            audioTime = Mathf.Round(audioTime / beatLength) * beatLength; //Round to nearest beat
            audioTime += bpmOffset;
            SetPlayback(audioTime);
        }

        TimeLine.Instance.SetCurrentTimeStamp(Mathf.RoundToInt(currentTime * 1000));

        if (Input.GetKeyDown(KeyCode.Space) && !PopupManager.Instance.shadow.activeInHierarchy)
        {
            if (nAudioPlayer.isPlaying || audioSource.isPlaying) PauseSong();
            else PlaySong();
        }
    }

    public void SetSong(string filepath)
    {
        if (useNAudio) nAudioPlayer.SetSong(filepath);
        else audioImporter.Import(filepath);
    }

    private void SongLoaded(AudioClip audioClip)
    {
        audioSource.clip = audioClip;
        audioSource.Play();
        audioSource.Pause();
        TimeLine.Instance.SetTimeLineLength((int)(audioClip.length * 1000)); //Multiply with a 1000 to convert from seconds to milliseconds
    }

    ///<summary>Set playback time. Expects time in seconds</summary>
    public void SetPlayback(float playbackTime)
    {
        if (useNAudio) nAudioPlayer.SetPlayback(playbackTime);
        else
        {
            audioSource.time = playbackTime;
            if (audioSource.isPlaying)
                SoundManager.ScheduleHitsounds();
        }
    }

    public void PlaySong()
    {
        if (useNAudio) nAudioPlayer.Play();
        else
        {
            //startTime = AudioSettings.dspTime + .05;
            //audioSource.PlayScheduled(startTime);
            //startTime -= currentTime;
            audioSource.Play();
            SoundManager.ScheduleHitsounds();
        }
    }

    public void PauseSong()
    {
        if (useNAudio) nAudioPlayer.Pause();
        else audioSource.Pause();
        SoundManager.CancelSchedule();
    }

    public void StopSong()
    {
        if (useNAudio)
        {
            nAudioPlayer.SetPlayback(0);
            nAudioPlayer.Stop();
        }
        else
        {
            SetPlayback(0);
            audioSource.Pause();
        }
        SoundManager.CancelSchedule();
    }

    ///<summary>The volume of the audio source (0.0 to 1.0) </summary>
    public void SetVolume(float volume)
    {
        if (useNAudio)
        {
            nAudioPlayer.SetVolume(volume);
        }
        else
        {
            audioSource.volume = volume;
        }
    }

    public void SetPlaybackSpeed(float sliderValue)
    {
        //audioSource.pitch = sliderValue;
        audioSource.pitch = sliderValue;
        float pitchShift = 1f / sliderValue;
        audioMixer.SetFloat("pitch1", pitchShift);
        playbackSpeed = sliderValue;
        SoundManager.CancelSchedule();
        SoundManager.ScheduleHitsounds();
        //audioMixer.SetFloat("pitch2", Mathf.Clamp(pitchShift - 1, 1, 2));
    }
}