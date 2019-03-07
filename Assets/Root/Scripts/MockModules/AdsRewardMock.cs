using System;
using System.Collections.Generic;
using UnityEngine;

namespace Root
{
    public class AdsRewardMock : IAdsRewardService
    {
        public void Init()
        {
        }

        public bool IsAdsAvailable()
        {
			return true;
        }

        public void ShowAds(Action rewardCallback = null, Action notRewardCallback = null)
        {
			if(rewardCallback != null)
			{
				rewardCallback();
			}
        }
    }
}

