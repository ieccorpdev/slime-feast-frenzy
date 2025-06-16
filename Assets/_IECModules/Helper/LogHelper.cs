using System;
using System.Collections.Generic;
using UnityEngine;

public static class LogHelper
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

    public static string Log(this Vector2 v)
    {
        return string.Format($"({v.x}, {v.y})");
    }

    public static string Log(this Vector3 v)
    {
        return string.Format($"({v.x}, {v.y}, {v.z})");
    }
}