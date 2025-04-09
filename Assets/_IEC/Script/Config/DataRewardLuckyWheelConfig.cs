using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/DataRewardLuckyWheelConfig")]
public class DataRewardLuckyWheelConfig : ScriptableObject
{
    public int numberSpinAdsEachDay = 1;
    public int levelUnlock = 10;
    public int levelUnlockV2 = 11;
    public int goldStart = 10;
    public int goldPerSpin = 5;
    public List<RewardsLuckyWheel> listRewardLuckyWheel;
    public List<RewardsLuckyWheel> listRewardTutorial;
    public List<RewardsLuckyWheel> listRewardOverrideABTest;
    public List<RewardsLuckyWheel> listRewardTutorialOverrideABTest;
    public int GoldCostToSpin(int timeSpin)
    {
        return goldStart + goldPerSpin * timeSpin;
    }
}

[Serializable]
public class RewardsLuckyWheel
{
    public Sprite icon;
    public int percentageRandom;
    public Reward dataReward;
}

