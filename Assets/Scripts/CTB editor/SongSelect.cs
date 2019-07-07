using System.IO;
using SFB;
using UnityEngine;
using UnityEngine.UI;

public class SongSelect : MonoBehaviour
{
    [SerializeField] private Text selectedSongText;

    private Button button;
    private MusicPlayer player;

    void Start ()
    {
        player = FindObjectOfType<MusicPlayer>();
	    button = GetComponent<Button>();
	    button.onClick.AddListener(SelectSong);
	}

    void SelectSong()
    {
        string[] path = StandaloneFileBrowser.OpenFilePanel("Select song","","mp3",false);
        player.SetSong(path[0]);
        selectedSongText.text = "Selected song:\t" + Path.GetFileName(path[0]);
    }
}
