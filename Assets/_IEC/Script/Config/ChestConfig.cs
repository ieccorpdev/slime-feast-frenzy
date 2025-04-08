using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/ChestConfig")]
public class ChestConfig : ScriptableObject
{
    public ChestType chestType;
    public int milestoneBeginLoop;
    public List<ChestRewardData> listChestRewardData = new();

    public ChestRewardData GetRewardData(int index)
    {
        index = Mathf.Clamp(index, 0, listChestRewardData.Count - 1);

        return listChestRewardData[index];
    }
    
}

[Serializable]
public class ChestRewardData
{
    public int progressToReward;
    public List<Reward> listReward = new();
}

public enum ChestType
{
    Level,
    Star
}

