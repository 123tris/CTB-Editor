using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TextBehaviour : MonoBehaviour
{
    protected TextMeshProUGUI textMesh;

    protected virtual void Awake()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
    }
}