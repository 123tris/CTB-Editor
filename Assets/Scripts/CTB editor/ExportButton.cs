using System.IO;
using System.Linq;
using SFB;
using UnityEngine;
using UnityEngine.UI;

public class ExportButton : MonoBehaviour
{
    [SerializeField] private GameObject exportWindow;

    private Button button;
    private EditorSettings settings;
    private EditorMapSettings mapSettings => settings.mapSettings;

    private void Start()
    {
        exportWindow.SetActive(false);
        settings = FindObjectOfType<EditorSettings>();
        if (settings == null) Debug.LogError("Whoops",this);
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        void OnYes()
        {
            CreateMapInOsuDirectory();
            PopupManager.Show("New beatmap created! Saving wil now update the beatmap you're working on!");
        }

        PopupManager.ShowYesNo("Do you want to create a new beatmap?", OnYes);
    }

    public void CreateMapInOsuDirectory()
    {
        string folderPath = settings.metaData.songsDirectory + "/" + BeatmapSettings.audioFileName;
        if (Directory.Exists(folderPath))
        {
            folderPath += $" {Random.Range(0, 99999)}";
            Directory.CreateDirectory(folderPath); //TODO: temporary way of dealing with duplicates.
        }
        else Directory.CreateDirectory(folderPath);

        mapSettings.beatmapFilepath = folderPath;
        
        File.Copy(settings.metaData.audioSourceFilePath,folderPath+$"/{BeatmapSettings.audioFileName}.mp3");

        SaveMapInOsuDirectory();
    }

    private void SaveMapInOsuDirectory()
    {
        BeatmapConverter.WriteOsuFile(mapSettings.beatmapFilepath + "/" + BeatmapSettings.audioFileName + ".osu");
    }
}
