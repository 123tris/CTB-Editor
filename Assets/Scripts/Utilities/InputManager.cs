using System;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public enum InputAction
{
    SelectBrush,
    FruitBrush,
    SliderBrush,
    ToggleNewCombo,
    ToggleWhistle,
    ToggleFinish,
    ToggleClap,
    ToggleGridSnap,
    TempToggleGridSnap
}

public class InputManager : MonoBehaviour
{
    public Dictionary<InputAction, List<KeyCode>> inputActions = new Dictionary<InputAction, List<KeyCode>>();

    private static InputManager instance; //

    public static Vector2 mousePosition => Input.mousePosition.Multiply(new Vector3(Screen.width/1920f,Screen.height/1080f));

    void Awake()
    {
        instance = this;

        inputActions[InputAction.SelectBrush] = new List<KeyCode> {KeyCode.Alpha1, KeyCode.Keypad1};
        inputActions[InputAction.FruitBrush] = new List<KeyCode> { KeyCode.Alpha2, KeyCode.Keypad2 };
        inputActions[InputAction.SliderBrush] = new List<KeyCode> { KeyCode.Alpha3, KeyCode.Keypad3 };
        inputActions[InputAction.ToggleNewCombo] = new List<KeyCode> { KeyCode.Q };
        inputActions[InputAction.ToggleWhistle] = new List<KeyCode> { KeyCode.W };
        inputActions[InputAction.ToggleFinish] = new List<KeyCode> { KeyCode.E };
        inputActions[InputAction.ToggleClap] = new List<KeyCode> { KeyCode.R };
        inputActions[InputAction.ToggleGridSnap] = new List<KeyCode> { KeyCode.T };
        inputActions[InputAction.TempToggleGridSnap] = new List<KeyCode> { KeyCode.LeftShift };
    }

    public static bool GetButtonDown(InputAction action)
    {
        return instance.inputActions[action].Any(Input.GetKeyDown);
    }

    public static bool GetButtonUp(InputAction action)
    {
        return instance.inputActions[action].Any(Input.GetKeyUp);
    }

    public static bool GetButtonPressed(InputAction action)
    {
        return instance.inputActions[action].Any(Input.GetKey);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(InputManager))]
public class InputManagerEditor : Editor
{
    private string addedAction;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        InputManager inputManager = (InputManager)target;
        var inputActions = inputManager.inputActions;

        //GUILayout.BeginHorizontal();
        //if (GUILayout.Button("Add action"))
        //{
        //    inputActions[addedAction] = new List<KeyCode>{KeyCode.None};
        //}
        //addedAction = EditorGUILayout.TextField(addedAction);
        //GUILayout.EndHorizontal();

        GUILayout.BeginVertical("Box");
        foreach (KeyValuePair<InputAction, List<KeyCode>> actionPair in inputActions)
        {
            GUILayout.BeginHorizontal("Box");
            GUILayout.Label(actionPair.Key.ToString(),GUILayout.MinWidth(120));
            for (int i = 0; i < actionPair.Value.Count; i++)
            {
                KeyCode keyCode = actionPair.Value[i];
                GUILayout.BeginVertical();
                inputActions[actionPair.Key][i] = (KeyCode)EditorGUILayout.EnumPopup(keyCode);
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();
    }
}
#endif