#if (PluginAppodeal)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AppodealAds.Unity.Api;
using AppodealAds.Unity.Common;
using System;

namespace Root
{
    public class AppodealModule : IAdsService, IBannerAdListener, IInterstitialAdListener, IRewardedVideoAdListener
    {
        AdsPlatformConfig m_Config;
        bool m_IsAcceptConsent;

        AdsStateData m_BannerAdsState = new AdsStateData();
        AdsStateData m_FullAdsState = new AdsStateData();
        AdsStateData m_RewardAdsState = new AdsStateData();
        AdsStateData m_NullAdsState = new AdsStateData();

        AdsEnum m_BannerAdsPos = AdsEnum.BANNER_BOTTOM;

        public void InitAppodeal(AdsConfig adsConfig, string[] disableNetworks, bool isAcceptConsent, bool isHasBanner,
            bool isHasFullAds)
        {
            var config = (Application.platform == RuntimePlatform.Android) ? adsConfig.android : adsConfig.ios;
            config.appID = config.appID.Trim();
            config.bannerID = config.bannerID.Trim();
            config.interstitialID = config.interstitialID.Trim();
            config.rewardID = config.rewardID.Trim();

            m_Config = config;
            m_IsAcceptConsent = isAcceptConsent;

            foreach (var network in disableNetworks)
            {
                Appodeal.disableNetwork(network);
            }

            Appodeal.setAutoCache(Appodeal.REWARDED_VIDEO, true);
            int adType = Appodeal.REWARDED_VIDEO;
            if (isHasBanner)
            {
                Appodeal.setAutoCache(Appodeal.BANNER, true);
                adType = adType | Appodeal.BANNER;
            }

            if (isHasFullAds)
            {
                Appodeal.setAutoCache(Appodeal.INTERSTITIAL, true);
                adType = adType | Appodeal.INTERSTITIAL;
            }

            Appodeal.disableLocationPermissionCheck();
            Appodeal.initialize(config.appID, adType, isAcceptConsent);

            Appodeal.setInterstitialCallbacks(this);
            Appodeal.setRewardedVideoCallbacks(this);
            Appodeal.setBannerCallbacks(this);
            Appodeal.setSmartBanners(false);
        }

        public double GetEcpm(AdsEnum adsType)
        {
            var type = GetAppodealType(adsType);
            if (Appodeal.isLoaded(type))
            {
                return Appodeal.getPredictedEcpm(type);
            }

            return 0;
        }

        public string GetInfo(AdsEnum adsType)
        {
            return m_Config.appID;
        }

        public void Hide(AdsEnum adsType)
        {
            GetAdsState(adsType).isWaitToShow = false;
            var type = GetAppodealType(adsType);
            Appodeal.hide(type);
        }

        public bool IsAcceptGDPR()
        {
            return m_IsAcceptConsent;
        }

        public bool IsLoaded(AdsEnum adsType)
        {
            var type = GetAppodealType(adsType);
            return Appodeal.isLoaded(type);
        }

        public void Load(AdsEnum adsType, Action<AdsEventArgs> loadCallback)
        {
            var adsState = GetAdsState(adsType);
            adsState.onLoad = loadCallback;
        }

        public void SetOnClick(AdsEnum adsType, Action clickCallback)
        {
            switch (adsType)
            {
                case AdsEnum.BANNER_TOP:
                case AdsEnum.BANNER_BOTTOM:
                    m_BannerAdsState.onClick = clickCallback;
                    break;
                case AdsEnum.INTERSTITIAL:
                    m_FullAdsState.onClick = clickCallback;
                    break;
                case AdsEnum.REWARD:
                    m_RewardAdsState.onClick = clickCallback;
                    break;
            }
        }

        public void Show(AdsEnum adsType, Action<AdsEventArgs> showCallback)
        {
            var adsState = GetAdsState(adsType);
            adsState.isWaitToShow = true;
            
            var type = GetAppodealType(adsType);
            adsState.onShow = showCallback;
            if (IsBannerType(adsType))
            {
                m_BannerAdsPos = adsType;
                Appodeal.show(type);
                return;
            }

            var isCanShow = (adsType == AdsEnum.INTERSTITIAL) ? Appodeal.canShow(type) : true;

            if (Appodeal.isLoaded(type) && isCanShow)
            {
                AdsStateData.DebugLog("Show appodeal " + adsType);
                Appodeal.show(type);
            }
            else
            {
                AdsStateData.SafeInvoke(showCallback, AdsEventArgs.FAIL_NOT_LOADED);
                AdsStateData.DebugLog("Show but not loaded appodeal " + adsType);
            }
        }

        int GetAppodealType(AdsEnum adsType)
        {
            switch (adsType)
            {
                case AdsEnum.BANNER_BOTTOM:
                    return Appodeal.BANNER_BOTTOM;
                case AdsEnum.BANNER_TOP:
                    return Appodeal.BANNER_TOP;
                case AdsEnum.INTERSTITIAL:
                    return Appodeal.INTERSTITIAL;
                case AdsEnum.REWARD:
                    return Appodeal.REWARDED_VIDEO;
            }

            return Appodeal.NONE;
        }

        bool IsBannerType(AdsEnum adsType)
        {
            return (adsType == AdsEnum.BANNER_BOTTOM || adsType == AdsEnum.BANNER_TOP);
        }

        AdsStateData GetAdsState(AdsEnum adsType)
        {
            switch (adsType)
            {
                case AdsEnum.BANNER_BOTTOM:
                case AdsEnum.BANNER_TOP:
                    return m_BannerAdsState;
                case AdsEnum.INTERSTITIAL:
                    return m_FullAdsState;
                case AdsEnum.REWARD:
                    return m_RewardAdsState;
            }

            return m_NullAdsState;
        }

        #region bannerAds

        public void onBannerLoaded(bool isPrecache)
        {
            var adsState = m_BannerAdsState;
            adsState.isLoaded = true;
            adsState.isLoading = false;
            adsState.onLoad = AdsStateData.SafeInvoke(adsState.onLoad, AdsEventArgs.DONE);

            if (adsState.isWaitToShow)
            {
                Show(m_BannerAdsPos, adsState.onShow);
            }
            else
            {
                Hide(AdsEnum.BANNER_BOTTOM);
                Hide(AdsEnum.BANNER_TOP);
            }
        }

        public void onBannerFailedToLoad()
        {
            var adsState = m_BannerAdsState;
            AdsStateData.DebugLog("Appodeal onBannerFailedToLoad");
            adsState.isLoading = false;
            adsState.onLoad = AdsStateData.SafeInvoke(adsState.onLoad, AdsEventArgs.FAIL);
        }

        public void onBannerShown()
        {
            AdsStateData.SafeInvoke(m_BannerAdsState.onShow, AdsEventArgs.DONE);
        }

        public void onBannerClicked()
        {
            AdsStateData.DebugLog("Appodeal onBannerClicked");
            AdsStateData.SafeInvoke(m_BannerAdsState.onClick);
        }

        public void onBannerExpired()
        {
            AdsStateData.DebugLog("Appodeal onBannerExpired");
        }

        #endregion

        #region fullAds

        public void onInterstitialLoaded(bool isPrecache)
        {
            AdsStateData.DebugLog("Appodeal onInterstitialLoaded");
            m_FullAdsState.onLoad = AdsStateData.SafeInvoke(m_FullAdsState.onLoad, AdsEventArgs.DONE);
        }

        public void onInterstitialFailedToLoad()
        {
            AdsStateData.DebugLog("Appodeal onInterstitialFailedToLoad");
            m_FullAdsState.onLoad = AdsStateData.SafeInvoke(m_FullAdsState.onLoad, AdsEventArgs.FAIL);
        }

        public void onInterstitialShown()
        {
            AdsStateData.DebugLog("Appodeal onInterstitialShown");
            AdsStateData.SafeInvoke(m_FullAdsState.onShow, AdsEventArgs.DONE);
        }

        public void onInterstitialClosed()
        {
            AdsStateData.DebugLog("Appodeal onInterstitialClosed");
            m_FullAdsState.onShow = AdsStateData.SafeInvoke(m_FullAdsState.onShow, AdsEventArgs.DONE);
        }

        public void onInterstitialClicked()
        {
            AdsStateData.DebugLog("Appodeal onInterstitialClicked");
            AdsStateData.SafeInvoke(m_FullAdsState.onClick);
        }

        public void onInterstitialExpired()
        {
            AdsStateData.DebugLog("Appodeal onInterstitialExpired");
        }

        #endregion

        #region rewardAds

        public void onRewardedVideoLoaded(bool precache)
        {
            AdsStateData.DebugLog("Appodeal onRewardedVideoLoaded");
            m_RewardAdsState.onLoad = AdsStateData.SafeInvoke(m_RewardAdsState.onLoad, AdsEventArgs.DONE);
        }

        public void onRewardedVideoFailedToLoad()
        {
            AdsStateData.DebugLog("Appodeal onRewardedVideoFailedToLoad");
            m_RewardAdsState.onLoad = AdsStateData.SafeInvoke(m_RewardAdsState.onLoad, AdsEventArgs.FAIL);
        }

        public void onRewardedVideoShown()
        {
        }

        public void onRewardedVideoFinished(double amount, string name)
        {
        }

        public void onRewardedVideoClosed(bool finished)
        {
            AdsStateData.DebugLog("Appodeal onRewardedVideoClosed: " + finished);
            if (finished)
            {
                m_RewardAdsState.onShow = AdsStateData.SafeInvoke(m_RewardAdsState.onShow, AdsEventArgs.DONE_NO_SKIP);
            }
            else
            {
                m_RewardAdsState.onShow = AdsStateData.SafeInvoke(m_RewardAdsState.onShow, AdsEventArgs.DONE_SKIP);
            }
        }

        public void onRewardedVideoExpired()
        {
            AdsStateData.DebugLog("Appodeal onRewardedVideoExpired");
        }

        public void onRewardedVideoClicked()
        {
            AdsStateData.DebugLog("Appodeal onRewardedVideoClicked");
            AdsStateData.SafeInvoke(m_RewardAdsState.onClick);
        }

        #endregion
    }
}

#endif