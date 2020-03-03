using System;
using System.Collections.Generic;
using UnityEngine;

public class LogHelper
{
    public static void Log(Vector2 v, string label)
    {
        Log(v.x, v.y, label);
    }

    public static void Log(float x, float y, string label)
    {
        Debug.Log(label);
        Debug.Log("x: " + x);
        Debug.Log("y: " + y);
    }
}