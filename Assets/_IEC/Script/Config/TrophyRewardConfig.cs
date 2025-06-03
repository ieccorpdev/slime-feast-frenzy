using System;
using System.Collections;
using System.Collections.Generic;
using IEC;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/TrophyReward")]
public class TrophyRewardConfig : ScriptableObject
{
    public List<TroyphyReward> listTrophyReward;
}
[Serializable]public class TroyphyReward
{
    public TrophyType type;
    public List<TroyphyRewardDetails> listTrophyReward;
}
[Serializable]public class TroyphyRewardDetails
{
    public List<Reward> listTrophyReward;
}