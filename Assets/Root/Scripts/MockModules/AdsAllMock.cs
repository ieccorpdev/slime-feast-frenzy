using System;
using System.Collections.Generic;

namespace Root
{
    public class AdsAllMock : IAdsAllService
    {
        public void HideBanner()
        {
        }

        public void Init(AdsConfig adsConfig)
        {
        }

        public bool IsFullAdsLoaded()
        {
            return true;
        }

        public bool IsRewardAdsLoaded()
        {
            return true;
        }

        public void LoadAndShowBannerBot()
        {
        }

        public void LoadAndShowBannerTop()
        {
        }

        public void LoadBannerBot()
        {
        }

        public void LoadBannerTop()
        {
        }

        public void LoadFullAds(Action loadedCallback = null)
        {
            if (loadedCallback != null) loadedCallback();
        }

        public void RefreshBanner()
        {
        }

        public void ShowBanner(bool isRefreshBanner = false)
        {
        }

        public void ShowFullAds(Action closeCallback = null)
        {
            if (closeCallback != null) closeCallback();
        }

        public void ShowRewardAds(Action completeCallback = null, Action skipCallback = null)
        {
            if (completeCallback != null) completeCallback();
        }

        public void ShutDown()
        {
        }
    }
}

