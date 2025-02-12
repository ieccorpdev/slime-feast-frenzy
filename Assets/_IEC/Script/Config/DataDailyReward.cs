using System.Collections;
using System.Collections.Generic;
using IEC;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/DataDailyReward")]
public class DataDailyReward : ScriptableObject
{
    public Sprite goldSprite;
    public Sprite manyGoldSprite;

    public Reward mainReward;
    public List<Reward> dailyRewards;

    public Sprite GetRewardSprite(ResourceType rewardType, bool isGoldMany = true)
    {
        switch (rewardType)
        {
            case ResourceType.Gold:
                if(!isGoldMany)
                    return goldSprite;
                return manyGoldSprite;
            default:
                return null;
        }
    }
}