using System.Collections;
using System.Collections.Generic;
using IEC;
using TMPro;
using UnityEngine;

public class UIItemReward : MonoBehaviour
{
    public ResourceType typeReward;
    public bool haveData = false;
    public TextMeshProUGUI valueReward;
    public Vector3 targetAnchorPos;

    public void  LoadValueReward(ResourceType type,int amount)
    {
        string reward;
        switch (type)
        {
            case ResourceType.UnlimitedLive:
            case ResourceType.TimeUnlimitedEvolveStone:
            case ResourceType.TimeUnlimitedWindSurf:
            case ResourceType.DoubleSpecialObjectCount:
                reward = amount + "m";
                break;
            default:
                reward = "x" + amount;
                break;
        }
        valueReward.text = reward;
    }
}
