using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/RewardConfig")]
public class BuildCityRewardConfig : ScriptableObject
{
    public List<CityRewards> listRewardByRank;
}

[Serializable]
public class CityRewards
{
    public List<CityReward> listReward;
}

[Serializable]
public class CityReward
{
    public int rewardAmount;
    public BonusDataCityReward typeCityReward;
    public CityReward()
    {
    }
    public CityReward(int rewardAmount, BonusDataCityReward typeCityReward)
    {
        this.rewardAmount = rewardAmount;
        this.typeCityReward = typeCityReward;
    }
}

public enum BonusDataCityReward
{
    Gold,
}
