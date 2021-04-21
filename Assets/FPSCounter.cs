using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSCounter : TextBehaviour
{
    [SerializeField]
    private float refreshRate = 0.2f;

    float counter;
    float startTime;

    void ResetTimer()
    {
        counter = 0;
        startTime = Time.unscaledTime;
    }

    private void Update()
    {
        counter++;
        if (startTime + refreshRate <= Time.unscaledTime)
        {
            int fps = (int)(counter / (Time.unscaledTime - startTime));
            textMesh.text = $"FPS: {fps}";
            ResetTimer();
        }
    }
}
