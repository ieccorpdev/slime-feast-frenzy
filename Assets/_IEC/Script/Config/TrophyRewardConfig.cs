using System;
using System.Collections;
using System.Collections.Generic;
using IEC;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/TrophyReward")]
public class TrophyRewardConfig : ScriptableObject
{
    public List<TroyphyReward> listTrophyReward;

    public TroyphyReward GetReward(TrophyType type)
    {
        return listTrophyReward.Find(x => x.type == type);
    }
}
[Serializable]public class TroyphyReward
{
    public TrophyType type;
    public List<int> listTimeToCollect;
    public List<TroyphyRewardDetails> listTrophyReward;
}
[Serializable]public class TroyphyRewardDetails
{
    public List<Reward> listTrophyReward;
}