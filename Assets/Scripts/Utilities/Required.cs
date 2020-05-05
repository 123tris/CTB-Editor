#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.Assertions;
#endif
using UnityEngine;

public class Required : PropertyAttribute
{
    public string message;
    
    public Required() {}

    public Required(string text)
    {
        message = text;
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(Required))]
public class RequiredDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        Required required = attribute as Required;

        EditorGUI.BeginProperty(position, label, property);

        if (property.objectReferenceValue == null)
        {
            string message = required.message ?? $"{fieldInfo.Name} is required";
            EditorGUI.HelpBox(position,message,MessageType.Error);
        }

        EditorGUI.PropertyField(position, property, label);

        EditorGUI.EndProperty();
    }
}
#endif