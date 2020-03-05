using System.IO;
using SFB;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SongSelect : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI selectedSongText;

    private Button button;
    private MusicPlayer player;

    private EditorSettings editorSettings;

    void Start ()
    {
        editorSettings = FindObjectOfType<EditorSettings>();
        player = FindObjectOfType<MusicPlayer>();
	    button = GetComponent<Button>();
	    button.onClick.AddListener(SelectSong);
	}

    void Update()
    {
        if (selectedSongText != null)
            selectedSongText.text = "Selected song:\n" + BeatmapSettings.audioFileName;
    }

    void SelectSong()
    {
        string[] path = StandaloneFileBrowser.OpenFilePanel("Select song","","mp3",false);

        string filepath = path[0];

        if (path.Length == 0) return;
        player.SetSong(filepath);
        BeatmapSettings.audioFileName = Path.GetFileNameWithoutExtension(filepath);
        editorSettings.metaData.audioSourceFilePath = filepath;
    }
}
