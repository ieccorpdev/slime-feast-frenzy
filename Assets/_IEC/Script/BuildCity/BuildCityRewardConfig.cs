using System;
using System.Collections;
using System.Collections.Generic;
using IEC;
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
    public ResourceType typeCityReward;
    public CityReward()
    {
    }
    public CityReward(int rewardAmount, ResourceType typeCityReward)
    {
        this.rewardAmount = rewardAmount;
        this.typeCityReward = typeCityReward;
    }
}
