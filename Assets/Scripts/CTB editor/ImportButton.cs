using System.Collections.Generic;
using System.IO;
using System.Linq;
using OsuParsers.Beatmaps;
using SFB;
using UnityEngine;
using UnityEngine.UI;

public class ImportButton : MonoBehaviour
{
    private MusicPlayer player;
    private Button button;
    private EditorMapSettings mapSettings;
    private TextUI textUI;

    private void Start()
    {
        textUI = FindObjectOfType<TextUI>();
        mapSettings = FindObjectOfType<EditorSettings>().mapSettings;
        button = GetComponent<Button>();
        player = FindObjectOfType<MusicPlayer>();
        button.onClick.AddListener(Import);
    }

    private void Import()
    {
        string[] path = StandaloneFileBrowser.OpenFilePanel("Select beatmap","","osu",false);
        
        //Did user select a file?
        if (path.Length == 0) return;

        //Load the selected file
        Beatmap beatmap = BeatmapConverter.ImportBeatmap(path.First());
        textUI.LoadSettings();

        //Update editor settings
        string directory = Path.GetDirectoryName(path.First());
        mapSettings.beatmapFilepath = directory;

        player.SetSong(directory+"\\"+BeatmapSettings.audioFileName);
    }
}
