using System;
using System.Collections;
using System.Collections.Generic;
using IEC;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/RewardConfig")]
public class DataRewardConfig : ScriptableObject
{

    public List<Rewards> listRewardByRank;
}

[Serializable]
public class Rewards
{
    public List<Reward> listReward;
}

