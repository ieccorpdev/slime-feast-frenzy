using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformHelper
{
    public static void SetX(Transform transform, float posX)
    {
        Vector3 temp = transform.position;
        temp.x = posX;
        transform.position = temp;
    }

    public static void SetLocalX(Transform transform, float posX)
    {
        Vector3 temp = transform.localPosition;
        temp.x = posX;
        transform.localPosition = temp;
    }

    public static void SetRotateX(Transform transform, float angleX)
    {
        Vector3 temp = transform.eulerAngles;
        temp.x = angleX;
        transform.eulerAngles = temp;
    }

    public static void SetY(Transform transform, float posY)
    {
        Vector3 temp = transform.position;
        temp.y = posY;
        transform.position = temp;
    }

    public static void SetLocalY(Transform transform, float posY)
    {
        Vector3 temp = transform.localPosition;
        temp.y = posY;
        transform.localPosition = temp;
    }

    public static void SetRotateY(Transform transform, float angleY)
    {
        Vector3 temp = transform.eulerAngles;
        temp.y = angleY;
        transform.eulerAngles = temp;
    }

    public static void SetZ(Transform transform, float posZ)
    {
        Vector3 temp = transform.position;
        temp.z = posZ;
        transform.position = temp;
    }

    public static void SetLocalZ(Transform transform, float posZ)
    {
        Vector3 temp = transform.localPosition;
        temp.z = posZ;
        transform.localPosition = temp;
    }

    public static void SetRotateZ(Transform transform, float angleZ)
    {
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, angleZ), 1500 * Time.deltaTime);
    }

    public static Vector3 GetSizeGridsFit(Vector2 gridSize, int width, int height, float space)
    {
        float sizeX, sizeY;
        sizeX = width * gridSize.x + (width - 1) * space * gridSize.x;
        sizeY = height * gridSize.y + (height - 1) * space * gridSize.y;
        return new Vector3(sizeX, sizeY, 0f);
    }

    public static Vector3 GetScaleSpriteFit(Sprite sprite, Vector2 gridSize, int width, int height, float space, float angleZ = 0f)
    {
        float scaleX, scaleY;
        var size = GetSizeGridsFit(gridSize, width, height, space);
        if (angleZ == 90f || angleZ == 270f || angleZ == -90f)
        {
            scaleX = 1.0f * size.y / (sprite.bounds.size.x);
            scaleY = 1.0f * size.x / (sprite.bounds.size.y);
        }
        else
        {
            scaleX = 1.0f * size.x / (sprite.bounds.size.x);
            scaleY = 1.0f * size.y / (sprite.bounds.size.y);
        }
        return new Vector3(scaleX, scaleY, 1f);
    }

}
