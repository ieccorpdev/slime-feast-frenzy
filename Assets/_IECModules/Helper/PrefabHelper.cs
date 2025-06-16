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
    public static T GetObjectPool<T>(GameObject prefab)
    {
        return GetObjectPool(prefab).GetComponent<T>();
    }
    public static GameObject Clone(GameObject prefab)
    {
        return prefab.Spawn();
    }   
    public static GameObject GetObjectPool(GameObject prefab)
    {
        return prefab.GetObjectPool();
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

    public static GameObject CloneRectTransform(GameObject prefab)
    {
        var obj = Clone(prefab, prefab.transform.parent);
        
        obj.transform.position = prefab.transform.position;
        obj.transform.rotation = prefab.transform.rotation;
        obj.transform.localScale = prefab.transform.localScale;

        var rect = prefab.GetComponent<RectTransform>();
        var rectClone = obj.GetComponent<RectTransform>();
        rectClone.anchoredPosition3D = rect.anchoredPosition3D;
        rectClone.sizeDelta = rect.sizeDelta;

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