using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class EditorSettings : MonoBehaviour
{
    public EditorMetaData metaData = new EditorMetaData();
    public EditorMapSettings mapSettings = new EditorMapSettings();

    public string settingsPath => Application.streamingAssetsPath + "/EditorSettings.json";
    public string mapPath => Application.streamingAssetsPath + "/MapSettings.json";

    void Awake()
    {
        if (!File.Exists(settingsPath)) SaveSettings();
        if (!File.Exists(mapPath)) SaveMap();

        LoadSettings();
        LoadMap();
    }

    void OnDestroy()
    {
        SaveSettings();
        SaveMap();
    }

    public void LoadSettings() => Load(out metaData,settingsPath);

    public void SaveSettings() => Save(metaData,settingsPath);

    public void LoadMap() => Load(out mapSettings,mapPath);

    public void SaveMap() => Save(mapSettings,mapPath);

    private void Load<T>(out T obj,string path)
    {
        using (StreamReader file = File.OpenText(path))
        {
            JsonSerializer serializer = new JsonSerializer();
            obj = (T) serializer.Deserialize(file, typeof(T));
        }
    }

    private void Save<T>(T obj, string path)
    {
        using (StreamWriter file = File.CreateText(path))
        {
            JsonSerializer serializer = new JsonSerializer();
            serializer.Serialize(file, obj);
        }
    }
}
