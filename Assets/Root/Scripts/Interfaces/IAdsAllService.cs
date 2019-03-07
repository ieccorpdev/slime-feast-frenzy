using System;

namespace Root
{

    public interface IAdsAllService
    {

        void Init(AdsConfig adsConfig);

        void ShowFullAds(Action closeCallback = null);

        void LoadFullAds(Action loadedCallback = null);

        bool IsFullAdsLoaded();

        void LoadAndShowBannerBot();

        void LoadAndShowBannerTop();

        void LoadBannerBot();

        void LoadBannerTop();

        void ShowBanner(bool isRefreshBanner = false);

        void HideBanner();

        void RefreshBanner();

        bool IsRewardAdsLoaded();

        void ShowRewardAds(Action completeCallback = null, Action skipCallback = null);

        void ShutDown();

    }
}

