using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSCounter : TextBehaviour
{
    private float timer;
    private float refreshRate = 0.5f;

    private void Update()
    {
        if (Time.unscaledTime > timer)
        {
            int fps = (int)(1f / Time.unscaledDeltaTime);
            textMesh.text = $"FPS: {fps}";
            timer = Time.unscaledTime + refreshRate;
        }
    }
}
