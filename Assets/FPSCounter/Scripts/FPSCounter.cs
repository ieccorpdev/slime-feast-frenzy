using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FPSCounter : MonoBehaviour
{
    public float updateInterval = 0.2f; 

    TMP_Text txt;
    float time = 0.0f;
    int frames = 0;

    float maxFps = float.MinValue;
    float minFps = float.MaxValue;
    float totalFps = 0f;
    int totalIntervals = 0;
    [LunaPlaygroundField("showFPS", 0, "Config")]
    public bool showFPS = true;
    void Start()
    {
        txt = GetComponent<TMP_Text>();
        txt.enabled = showFPS;
        Application.targetFrameRate = 60;
    }

    void Update()
    {
        time += Time.unscaledDeltaTime;
        ++frames;

        if (time >= updateInterval)
        {
            float fps = frames / time;

            // Update min, max, and average
            if (fps > maxFps) maxFps = fps;
            if (fps < minFps) minFps = fps;
            totalFps += fps;
            totalIntervals++;

            float avgFps = totalFps / totalIntervals;
            if (showFPS)
            {
                txt.text = string.Format(
                "FPS: {0:0}\nMax: {1:0}\nMin: {2:0}\nAvg: {3:0}",
                fps, maxFps, minFps, avgFps
                );
            }
            if (fps < 30 && PlayableController.Instance.fixedTimeStep != 0)
            {
                Time.fixedDeltaTime = PlayableController.Instance.fixedTimeStep;
            }
            else
            {
                Time.fixedDeltaTime = 0.04f;
            }
            time = 0.0f;
            frames = 0;
        }
    }
}
