using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridItem : MonoBehaviour
{
    public Transform[] groupItem;
    public int color;

    public int GetGridType()
    {
        var count = groupItem.Length;
        for (int i = 0; i < count; i++)
        {
            if (groupItem[i].gameObject.activeSelf) return (i + 1);
        }

        return 0;
    }
}
