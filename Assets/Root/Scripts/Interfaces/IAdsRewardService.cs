using System;

namespace Root
{

    public interface IAdsRewardService
    {

        void Init();

        void ShowAds(Action rewardCallback = null, Action notRewardCallback = null);

        bool IsAdsAvailable();
    }

}

