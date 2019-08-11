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

    public static string audioFileName;

    void Start ()
    {
        player = FindObjectOfType<MusicPlayer>();
	    button = GetComponent<Button>();
	    button.onClick.AddListener(SelectSong);
	}

    void SelectSong()
    {
        string[] path = StandaloneFileBrowser.OpenFilePanel("Select song","","mp3",false);
        if (path.Length == 0) return;
        player.SetSong(path[0]);
        audioFileName = Path.GetFileName(path[0]);
        selectedSongText.text = "Selected song:\t" + audioFileName;
    }
}
