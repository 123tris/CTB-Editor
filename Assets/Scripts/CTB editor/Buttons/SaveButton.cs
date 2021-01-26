using System.Collections;
using System.Collections.Generic;
using OsuParsers.Beatmaps.Sections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SaveButton : MonoBehaviour
{
    [SerializeField] private GameObject exportWindow;
    private EditorSettings settings;

    private Button button => GetComponent<Button>();

    private DropdownBehaviour fileDropdown;

    void Start()
    {
        fileDropdown = GetComponentInParent<DropdownBehaviour>();
        if (fileDropdown == null) 
            Debug.LogError("Missing DropdownBehaviour script inside of the parent of the import button",this);

        settings = FindObjectOfType<EditorSettings>();
        button.onClick.AddListener(OnClick);
        button.onClick.AddListener(fileDropdown.ToggleDropdown);
    }

    void OnClick()
    {
        if (BeatmapConverter.importedBeatmap != null)
        {
            BeatmapConverter.WriteOsuFile(BeatmapConverter.importedBeatmapPath);
        }
        else
        {
            PopupManager.Show("You have to first import a beatmap before you can save. B-b-b-baka!!!");
            //TODO: edit when export/import task is being worked on
            //string path = settings.mapSettings.beatmapFilepath + "/" + BeatmapSettings.audioFileName + ".osu";
            //BeatmapConverter.WriteOsuFile(path);
        }
    }
}
