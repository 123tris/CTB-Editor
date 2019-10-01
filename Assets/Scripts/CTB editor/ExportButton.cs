using System.Linq;
using SFB;
using UnityEngine;
using UnityEngine.UI;

public class ExportButton : MonoBehaviour
{
    private Button button;

    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(Export);
    }

    private string GetPath()
    {
        if (BeatmapConverter.importedBeatmap != null)
            return BeatmapConverter.importedBeatmap.GeneralSection.AudioFilename;
        else
            return "";
    }

    private void Export()
    {
        string[] path = StandaloneFileBrowser.OpenFolderPanel("Select beatmap",GetPath(),false);

        if (path.Length == 0) return;

        BeatmapConverter.WriteOsuFile(path.First());
    }
}
