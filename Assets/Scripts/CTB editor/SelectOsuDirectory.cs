using System.IO;
using SFB;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SelectOsuDirectory : MonoBehaviour
{
    [SerializeField] private GameObject startUpWindow;
    [SerializeField] private TextMeshProUGUI welcomeText;
    private Button button;

    private EditorSettings _settings;
    private EditorMetaData MetaData => _settings.metaData;

	void Start ()
    {
        _settings = FindObjectOfType<EditorSettings>();
        if (File.Exists(MetaData.osuDirectory + "\\osu!.exe"))
        {
            startUpWindow.SetActive(false);
        }
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        string[] path = StandaloneFileBrowser.OpenFolderPanel("Select Osu Directory","",false);
        if (path.Length == 0) return;

        //Check if valid folder
        if (File.Exists(path[0] + "\\osu!.exe"))
        {
            MetaData.osuDirectory = path[0];
            _settings.SaveSettings();
            startUpWindow.SetActive(false);
        }
        else
        {
            welcomeText.text = "The folder you selected isn't a valid osu directory. You have to select the folder that contains \"osu!.exe\"";
        }
    }
}
