using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/DataRewardLuckyWheelConfig")]
public class DataRewardLuckyWheelConfig : ScriptableObject
{
    public int numberSpinAdsEachDay = 2;
    public int levelUnlock = 1;
    public List<RewardsLuckyWheel> listRewardLuckyWheel;
}

[Serializable]
public class RewardsLuckyWheel
{
    public Sprite icon;
    public int percentageRandom;
    public Reward dataReward;
}

