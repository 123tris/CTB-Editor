using System;
using RuntimeUndo;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public GameObject fruitPrefab;
    public GameObject sliderPrefab;
    public GameObject handlePrefab;

    public RectTransform level;
    public Brush brush;
    public static GameObject garbage;
    public EditorSettings settings;

    public static GameManager Instance;

    void Awake()
    {
        Instance = this;
        if (brush == null) brush = FindObjectOfType<Brush>();
        if (level == null) Debug.LogError("Reference to the level instance is not set in the game manager", this);
        garbage = new GameObject("Garbage Collection");
    }

    void Start()
    {
        HitObjectManager.fruitPrefab = fruitPrefab;
        HitObjectManager.sliderPrefab = sliderPrefab;

        Application.targetFrameRate = 120;
    }

    public float mouseClickTime;
    public float time;

    void Update()
    {
        mouseClickTime = ClickManager.mouseClickTime;
        time = Time.unscaledTime;

        HitObjectManager.UpdateFruits();

        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (Input.GetKeyDown(KeyCode.Z))
                Undo.PerformUndo();
            else if (Input.GetKeyDown(KeyCode.Y))
                Undo.PerformRedo();
            else if (Input.GetKeyDown(KeyCode.C))
                CopyManager.Copy();
            else if (Input.GetKeyDown(KeyCode.V))
                CopyManager.Paste();
            else if (Input.GetKeyDown(KeyCode.A))
                Selection.SelectAll();
            else if (Input.GetKeyDown(KeyCode.E)) //Test function
                BeatmapConverter.WriteOsuFile(Application.streamingAssetsPath + "/Exported CTB map.osu");
            else if (Input.GetKeyDown(KeyCode.S))
            {
                if (BeatmapConverter.importedBeatmap != null)
                    BeatmapConverter.WriteOsuFile(BeatmapConverter.importedBeatmapPath);
            }
        }

        if (Input.GetKeyDown(KeyCode.Delete))
            Selection.DestroySelected();


        //print($"Window screen: {Screen.width} x {Screen.height}\nScreen resolution: {Screen.currentResolution}");
    }

    public void LoadScene(string sceneName)
    {
        HitObjectManager.Reset();
        SceneManager.LoadScene(sceneName,LoadSceneMode.Single);
    }

    void LateUpdate()
    {
        ClickManager.Update();
    }
}
