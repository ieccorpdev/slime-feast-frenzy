using System;
using System.Collections.Generic;
using IEC;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/TrophyReward")]
public class TrophyRewardConfig : ScriptableObject
{
    public List<TroyphyReward> listTrophyReward  = new();

    public TroyphyReward GetReward(TrophyType type)
    {
        var objF = listTrophyReward.Find(x => x.type == type);
        TroyphyReward result = new TroyphyReward(objF.type,objF.listTimeToCollect,objF.listTrophyReward);
        return result;
    }
}
[Serializable]
public class TroyphyReward
{
    public TroyphyReward(TrophyType type,List<int> listTimeToCollect,List<TroyphyRewardDetails> listTrophyReward )
    {
        this.type = type;
        this.listTimeToCollect = listTimeToCollect;
        this.listTrophyReward = listTrophyReward;
    }
    public TrophyType type;
    public List<int> listTimeToCollect;
    public List<TroyphyRewardDetails> listTrophyReward = new();
}
[Serializable]
public class TroyphyRewardDetails
{
    public List<Reward> listTrophyReward = new();
}