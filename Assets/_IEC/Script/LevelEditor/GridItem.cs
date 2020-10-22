using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridItem : MonoBehaviour
{
    public Transform[] groupItem;
    public Transform[] groupSourcestyle;
    public Transform[] groupTargetstyle;
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

    public int GetSourceStyle()
    {
        var count = groupSourcestyle.Length;
        for (int i = 0; i < count; i++)
        {
            if (groupSourcestyle[i].gameObject.activeSelf) return i;
        }

        return 0;
    }

    public int GetTargetStyle()
    {
        var count = groupTargetstyle.Length;
        for (int i = 0; i < count; i++)
        {
            if (groupTargetstyle[i].gameObject.activeSelf) return i;
        }

        return 0;
    }
}
