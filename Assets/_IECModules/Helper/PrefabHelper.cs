using _IEC.Features.ExportLevel.OriginalLevel;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PrefabHelper
{
    public static GameObject Clone(GameObject prefab, Transform parent)
    {

        return new GameObject(prefab.name)
        {
            transform =
            {
                parent = parent,
                position = prefab.transform.position,
                rotation = prefab.transform.rotation,
                localScale = prefab.transform.localScale
            }
        };
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
}