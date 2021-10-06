using System;
using System.Collections.Generic;
using UnityEngine;

public class PrefabHelper
{
    public static void SetUpPrefab(GameObject prefab)
    {
        if(prefab == null)
        {
            Debug.Log("prefab is null");
            return;
        }
        prefab.CreatePool();
    }

    public static void SetUpPrefab(GameObject prefab, int initNumber)
    {
        if(prefab == null)
        {
            Debug.Log("prefab is null");
            return;
        }
        prefab.CreatePool(initNumber);
    }

    public static T Clone<T>(GameObject prefab)
    {
        return Clone(prefab).GetComponent<T>();
    }

    public static GameObject Clone(GameObject prefab)
    {
        return prefab.Spawn();
    }

    public static GameObject Clone(GameObject prefab, Transform parent)
    {
        return prefab.Spawn(parent);
    }

    public static GameObject CloneTransform(GameObject prefab)
    {
        var obj = prefab.Spawn();
        obj.transform.position = prefab.transform.position;
        obj.transform.rotation = prefab.transform.rotation;
        return obj;
    }

    public static GameObject CloneRenderer(GameObject prefab)
    {
        var obj = prefab.Spawn();
        obj.GetComponent<SpriteRenderer>().sprite = prefab.GetComponent<SpriteRenderer>().sprite;
        return obj;
    }

    public static void Destroy(GameObject obj)
    {
        obj.Recycle();
    }
}