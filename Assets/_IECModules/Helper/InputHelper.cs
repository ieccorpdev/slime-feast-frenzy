using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHelper
{
    public static bool IsClickDown()
    {
        return (Input.GetMouseButton(0));
    }

    public static Vector3 GetClickWorldPos(Camera camera)
    {
        if (Input.touchCount > 0)
        {
            return camera.ScreenToWorldPoint(Input.GetTouch(0).position);
        }
        else
        {
            return camera.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    public static Vector3 GetClickScreenPos()
    {
        if (Input.touchCount > 0)
        {
            return Input.GetTouch(0).position;
        }
        else
        {
            return Input.mousePosition;
        }
    }

}
