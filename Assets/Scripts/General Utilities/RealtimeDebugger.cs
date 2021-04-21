#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class RealtimeDebugger : EditorWindow
{
    //private static RealtimeDebugger instance;

    static Dictionary<string, object> debugProperties = new Dictionary<string, object>();

    [MenuItem("Toolkit/RealtimeDebugger")]
    static void Init()
    {
        GetWindow<RealtimeDebugger>();
    }

    void OnGUI()
    {
        foreach (KeyValuePair<string, object> debugProperty in debugProperties)
        {
            DisplayObject(debugProperty.Key, debugProperty.Value);
        }
    }

    void DisplayObject(string label, object value)
    {
        if (value.GetType().IsPrimitive)
        {
            switch (value)
            {
                case int integer:
                    EditorGUILayout.IntField(label, integer);
                    break;
                case float floatingPoint:
                    EditorGUILayout.FloatField(label, floatingPoint);
                    break;
                case bool boolean:
                    EditorGUILayout.Toggle(label, boolean);
                    break;
                case string text:
                    EditorGUILayout.LabelField(label,text);
                    break;
                case double doubleFloatingPoint:
                    EditorGUILayout.DoubleField(label, doubleFloatingPoint);
                    break;
                default:
                    throw new Exception("Unexpected primitive type");
            }
        }
        else if (value is Object o)
        {
            //SerializedObject serializedObject = new SerializedObject(o);
            EditorGUILayout.ObjectField(label, o, o.GetType(), false);
        }
        else
        {
            switch (value)
            {
                case Vector2 vec2:
                    EditorGUILayout.Vector2Field(label, vec2);
                    break;
                case Vector3 vec3:
                    EditorGUILayout.Vector3Field(label, vec3);
                    break;
                default:
                    Debug.Log($"Ur screwed mate {value.GetType().Name}");
                    break;
            }

        }
        Repaint();
    }

    public static void AddDebugProperty(string propertyName, object property)
    {
        debugProperties[propertyName] = property;
    }
}
#else
public class RealtimeDebugger
{
    public static void AddDebugProperty(string propertyName, object property)
    {
    }
}
#endif