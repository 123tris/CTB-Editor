using System.Collections.Generic;
using System.IO;
using System.Linq;
using CooldownManagerNamespace;
using OsuParsers.Beatmaps;
using SFB;
using UnityEngine;
using UnityEngine.UI;

public class ImportButton : MonoBehaviour
{
    private MusicPlayer player;
    private Button button;
    private EditorMapSettings mapSettings;

    private DropdownBehaviour fileDropdown;

    private void Start()
    {
        mapSettings = FindObjectOfType<EditorSettings>().mapSettings;
        button = GetComponent<Button>();
        player = FindObjectOfType<MusicPlayer>();

        fileDropdown = GetComponentInParent<DropdownBehaviour>();
        if (fileDropdown == null) 
            Debug.LogError("Missing filedropdown script inside of the parent of the import button",this);

        button.onClick.AddListener(Import);
        button.onClick.AddListener(fileDropdown.ToggleDropdown);
    }

    private void Import()
    {
        string[] path = StandaloneFileBrowser.OpenFilePanel("Select beatmap","","osu",false);
        
        //Did user select a file?
        if (path.Length == 0) return;

        //Load the selected file
        Grid.Instance.zoom = 1; //Reset the zoom to avoid possible import changes (likely design flaw)
        CooldownManager.OnNextFrame(() => //Execute on next frame so that the grid and other components can apply the zoom changes
        {
            BeatmapConverter.ImportBeatmap(path.First());

            //Update editor settings
            string directory = Path.GetDirectoryName(path.First());
            mapSettings.beatmapFilepath = directory;

            player.SetSong(directory+"\\"+BeatmapSettings.audioFileName+".mp3");
        });
    }
}
