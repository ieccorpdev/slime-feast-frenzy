using System;
using System.Collections.Generic;
using UnityEngine;

public class CameraHelper
{
    public static Camera MainCam { get; private set; }
    public static float MainCamHeight { get; private set; }
    public static float MainCamHalfHeight { get; private set; }
    public static float MainCamWidth { get; private set; }
    public static float MainCamHalfWidth { get; private set; }

    public static void SetMainCam(Camera camera)
    {
        MainCam = camera;
        MainCamHalfHeight = camera.orthographicSize;
        MainCamHeight = MainCamHalfHeight * 2;
        MainCamHalfWidth = camera.orthographicSize * camera.aspect;
        MainCamWidth = MainCamHalfWidth * 2;
    }

    public static float GetCamHeight(Camera cam)
    {
        return cam.orthographicSize * 2;
    }

    public static float GetCamWidth(Camera cam)
    {
        return cam.orthographicSize * cam.aspect * 2;
    }

}