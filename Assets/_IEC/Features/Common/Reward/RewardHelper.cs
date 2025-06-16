using _IEC.Features.Common.Extension;

namespace IEC
{
    public static class RewardHelper
    {
        public static void ClaimReward(Reward reward)
        {
            switch (reward.typeReward)
            {
                case ResourceType.Gold:
                    Pool.Get<PlayerSecret>().AddCurrency(ResourceType.Gold,reward.rewardAmount);
                    break;
                case ResourceType.UnlimitedLive:
                    Pool.Get<PlayerSecret>().AddTimeUnlimitedLife(reward.rewardAmount);
                    break;
                case ResourceType.Enlarge:
                case ResourceType.Vortex:
                case ResourceType.Compass:
                case ResourceType.TimeStop:
                    Pool.Get<PlayerSecret>().AddCurrency(reward.typeReward,reward.rewardAmount);
                    break;
                case ResourceType.BonusTime:
                case ResourceType.BonusExp:
                    Pool.Get<PlayerSecret>().AddCurrency(reward.typeReward,reward.rewardAmount);
                    break;
            }
        }
    }
}