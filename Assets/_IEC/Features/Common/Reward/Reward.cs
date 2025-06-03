using System;
using System.Collections.Generic;
using System.Globalization;

namespace IEC
{
    [Serializable]
    public class Reward
    {
        public ResourceType typeReward;
        public int rewardAmount;

        public Reward(int rewardAmount, ResourceType typeReward)
        {
            this.rewardAmount = rewardAmount;
            this.typeReward = typeReward;
        }

        public Reward()
        {
            rewardAmount = 0;
        }

        public static string GetInifiniteLivesString(Reward reward)
        {
            var rewardString = reward.rewardAmount + "m";
            if (reward.rewardAmount >= 60)
            {
                rewardString = ((float)reward.rewardAmount / 60).ToString(CultureInfo.InvariantCulture) + "h";
            }

            return rewardString;
        }

    }

    [Serializable]
    public class Rewards
    {
        public List<ProgressRewardData> listRewards;
    }

    public enum RewardTypes
    {
        NoAds,
        Gold,
        InfiniteLife ,
        ScaleUp,
        Magnet,
        Compass,
        FrozenTime,     
        BonusTime,
        BonusExp
    }
}