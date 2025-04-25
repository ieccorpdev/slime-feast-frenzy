using System;
using System.Collections;
using System.Collections.Generic;
using IEC;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/ChestConfig")]
public class ChestConfig : ScriptableObject
{
    public int levelUnlock = 0;
    public ChestType chestType;
    public int milestoneBeginLoop;
    public List<ChestRewardData> listChestRewardData = new();

    public ChestRewardData GetRewardData(int index)
    {
        //MeoMew fix Task #67686, reduce 1 level chest to get true reward
        index = Mathf.Clamp(index-1, 0, listChestRewardData.Count - 1);

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

