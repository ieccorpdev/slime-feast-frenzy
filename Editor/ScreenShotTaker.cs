using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ScreenShotTaker : EditorWindow
{
    [MenuItem("IEC/Take ScreenShot")]
    public static void Init()
    {
        string name = "ScrenShot_";
        var now = DateTime.Now;
        name += Screen.width + "_" + Screen.height + "_";
        name += now.Hour.ToString() + now.Minute.ToString() + now.Second.ToString() + ".png";
        Debug.Log("New " + name);
        ScreenCapture.CaptureScreenshot(name);
    }
}