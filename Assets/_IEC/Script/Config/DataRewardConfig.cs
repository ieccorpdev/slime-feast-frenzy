using System;
using System.Collections;
using System.Collections.Generic;
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

[Serializable]
public class Reward
{
    public int rewardAmount;
    public BonusDataReward typeReward;
    public Reward()
    {
    }
    public Reward(int rewardAmount, BonusDataReward typeReward)
    {
        this.rewardAmount = rewardAmount;
        this.typeReward = typeReward;
    }
}
//must containt all reward type of game here
public enum BonusDataReward
{
    Gold,
    UnlimitedLives,
}
