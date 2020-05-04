using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonBehaviour : MonoBehaviour
{
    protected Button button;

    protected virtual void Awake()
    {
        button = GetComponent<Button>();
    }
}
