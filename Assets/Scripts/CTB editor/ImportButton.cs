using System.Collections.Generic;
using System.IO;
using System.Linq;
using SFB;
using UnityEngine;
using UnityEngine.UI;

public class ImportButton : MonoBehaviour
{
    private MusicPlayer player;
    private Button button;

    private void Start()
    {
        button = GetComponent<Button>();
        player = FindObjectOfType<MusicPlayer>();
        button.onClick.AddListener(Import);
    }

    private void Import()
    {
        string[] path = StandaloneFileBrowser.OpenFilePanel("Select beatmap","","osu",false);
        if (path.Length == 0) return;

        BeatmapConverter.ImportBeatmap(path.First());

        List<string> splitString = path.First().Split('\\').ToList();
        splitString.RemoveAt(splitString.Count-1);
        string directory = string.Join("\\",splitString);
        player.SetSong(directory+"\\"+BeatmapSettings.audioFileName);
    }
}
