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

    void Start()
    {
        settings = FindObjectOfType<EditorSettings>();
        button.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        if (BeatmapConverter.importedBeatmap != null)
        {
            BeatmapConverter.WriteOsuFile(BeatmapConverter.importedBeatmapPath);
        }
        else
            BeatmapConverter.WriteOsuFile(settings.mapSettings.beatmapFilepath + "/" + BeatmapSettings.audioFileName + ".osu");
    }
}
