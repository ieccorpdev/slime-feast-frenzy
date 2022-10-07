#if PluginIronSource
using System;
using System.Collections;
using System.Collections.Generic;
using IEC;
using UnityEngine;

namespace _IECModules
{
    public class IronSourceModule : IAdsService
    {
        AdsPlatformConfig m_Config;
        bool m_IsAutoReload;
        bool m_IsAcceptConsent;
        bool m_IsInit = false;
        
        AdsStateData m_FullAdsState = new AdsStateData();
        AdsStateData m_RewardAdsState = new AdsStateData();

        private IronSourceBannerPosition m_BannerAdsPos = IronSourceBannerPosition.BOTTOM;
        
        int m_LoadFullAdCount = 0;
        int m_LoadRewardCount = 0;
        
        Action<string, float, Dictionary<string, object>> pushEventAction;
        public Action<AdsEnum, float> onAdRevenueAction;
        
        
        public void Init(AdsConfig adsConfig, bool isAcceptConsent, bool isAutoReload, Action<bool> afterInitCallback,
            Action<string, float, Dictionary<string, object>> pushEventAction)
        {
            var config = (Application.platform == RuntimePlatform.Android) ? adsConfig.android : adsConfig.ios;
            config.appID = config.appID.Trim();
            config.bannerID = config.bannerID.Trim();
            config.interstitialID = config.interstitialID.Trim();
            config.rewardID = config.rewardID.Trim();

            Debug.LogWarning("IronSource App ID:" + config.appID);
            // Debug.LogWarning("Applovin Banner ID:" + config.bannerID);
            // Debug.LogWarning("Applovin Inter ID:" + config.interstitialID);
            // Debug.LogWarning("Applovin Reward ID:" + config.rewardID);

            this.pushEventAction = pushEventAction;
            
            IronSource.Agent.setConsent(isAcceptConsent);
            
            if (string.IsNullOrEmpty(config.appID.Trim()) == false)
            {
                IronSource.Agent.init(config.appID, IronSourceAdUnits.BANNER);
                IronSource.Agent.init (config.appID, IronSourceAdUnits.INTERSTITIAL);
                IronSource.Agent.init (config.appID, IronSourceAdUnits.BANNER);
            }

            IronSourceEvents.onSdkInitializationCompletedEvent += () =>
            {
                m_IsInit = true;
                IronSource.Agent.validateIntegration();
                InitBanner();
                InitializeInterstitialAds();
                InitializeRewardedAds();
                Debug.LogWarning("Init IronSource Done");
                afterInitCallback?.Invoke(true);
            };

            m_Config = config;
            m_IsAutoReload = isAutoReload;
            m_IsAcceptConsent = isAcceptConsent;
        }

        public double GetEcpm(AdsEnum adsType)
        {
            return 0;
            // throw new NotImplementedException();
        }
        
        public string GetInfo(AdsEnum adsType)
        {
            switch (adsType)
            {
                case AdsEnum.BANNER_TOP:
                case AdsEnum.BANNER_BOTTOM:
                    return m_Config.bannerID;
                case AdsEnum.INTERSTITIAL:
                    return m_Config.interstitialID;
                case AdsEnum.REWARD:
                    return m_Config.rewardID;
            }

            return "IronSource";
        }
        
        public void Hide(AdsEnum adsType)
        {
            if (!m_IsInit)
            {
                return;
            }
            switch (adsType)
            {
                case AdsEnum.BANNER_BOTTOM:
                case AdsEnum.BANNER_TOP:
                    IronSource.Agent.hideBanner();
                    break;
            }
        }
        
        public bool IsAcceptGDPR()
        {
            return m_IsAcceptConsent;
        }
        
        public bool IsLoaded(AdsEnum adsType)
        {
            if (!m_IsInit)
            {
                return false;
            }

            switch (adsType)
            {
                case AdsEnum.BANNER_TOP:
                case AdsEnum.BANNER_BOTTOM:
                    return true;
                case AdsEnum.INTERSTITIAL:
                    return IsFullAdsLoaded();
                case AdsEnum.REWARD:
                    return IsRewardLoaded();
            }

            return false;
        }

        public void Load(AdsEnum adsType, Action<AdsEventArgs> loadCallback)
        {
            if (!m_IsInit)
            {
                AdsStateData.SafeInvoke(loadCallback, AdsEventArgs.FAIL);
                return;
            }
            switch (adsType)
            {
                case AdsEnum.BANNER_BOTTOM:
                case AdsEnum.BANNER_TOP:
                    LoadBannerAds(loadCallback, adsType);
                    break;
                case AdsEnum.INTERSTITIAL:
                    LoadFullAds(loadCallback);
                    break;
                case AdsEnum.REWARD:
                    LoadRewardAds(loadCallback);
                    break;
            }
        }

        public void SetOnClick(AdsEnum adsType, Action clickCallback)
        {
            if (!m_IsInit)
            {
                return;
            }
        }
        
        public void Show(AdsEnum adsType, Action<AdsEventArgs> showCallback)
        {
            if (!m_IsInit)
            {
                AdsStateData.SafeInvoke(showCallback, AdsEventArgs.FAIL);
                return;
            }

            switch (adsType)
            {
                case AdsEnum.BANNER_BOTTOM:
                case AdsEnum.BANNER_TOP:
                    ShowBanner(showCallback, adsType);
                    break;
                case AdsEnum.INTERSTITIAL:
                    ShowFullAds(showCallback);
                    break;
                case AdsEnum.REWARD:
                    ShowRewardAds(showCallback);
                    break;
            }
        }

        public float GetBannerHeight()
        {
            var adaptiveHeight = 50f;
            float f = GetScreenDensity();
            Debug.LogWarning("GetAdaptiveBannerHeight: " + adaptiveHeight);
            Debug.LogWarning("GetScreenDensity: " + f);
            return adaptiveHeight * f;
        }

        float GetScreenDensity()
        {
            if (Application.platform != RuntimePlatform.Android)
            {
                return MaxSdkUtils.GetScreenDensity();
            }
            AndroidJavaClass activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject activity = activityClass.GetStatic<AndroidJavaObject>("currentActivity");

            AndroidJavaObject metrics = new AndroidJavaObject("android.util.DisplayMetrics");
            activity.Call<AndroidJavaObject>("getWindowManager").Call<AndroidJavaObject>("getDefaultDisplay").Call("getMetrics", metrics);

            return metrics.Get<float>("ydpi") / 160f;
        }
        
#region bannerAds

        void LoadBannerAds(Action<AdsEventArgs> loadCallback, AdsEnum bannerPos)
        {
            if (string.IsNullOrEmpty(m_Config.bannerID))
            {
                AdsStateData.SafeInvoke(loadCallback, AdsEventArgs.FAIL_NO_ID);
                return;
            }

            IronSource.Agent.loadBanner(IronSourceBannerSize.BANNER, ConvertBannerPos(bannerPos));
            AdsStateData.SafeInvoke(loadCallback, AdsEventArgs.DONE);
        }

        void ShowBanner(Action<AdsEventArgs> showCallback, AdsEnum bannerPos)
        {
            IronSource.Agent.displayBanner();
            AdsStateData.SafeInvoke(showCallback, AdsEventArgs.DONE);
        }

        IronSourceBannerPosition ConvertBannerPos(AdsEnum bannerPos)
        {
            switch (bannerPos)
            {
                case AdsEnum.BANNER_BOTTOM:
                    return IronSourceBannerPosition.BOTTOM;
                case AdsEnum.BANNER_TOP:
                    return IronSourceBannerPosition.TOP;
                default:
                    return IronSourceBannerPosition.BOTTOM;
            }
        }

        void InitBanner()
        {
            IronSourceEvents.onBannerAdLoadedEvent += BannerAd_OnAdLoaded;
            IronSourceEvents.onBannerAdLoadFailedEvent += BannerAd_OnAdFailedToLoad;
            IronSourceEvents.onBannerAdClickedEvent += BannerAd_OnAdClicked;
            IronSourceEvents.onBannerAdScreenPresentedEvent += BannerAd_ScreenPresentedEvent;
            IronSourceEvents.onBannerAdScreenDismissedEvent += BannerAd_ScreenDismissedEvent;
            IronSourceEvents.onBannerAdLeftApplicationEvent += BannerAd_LeftApplicationEvent;
        }

        void BannerAd_ScreenPresentedEvent () 
        {
            Debug.Log("BannerAd_ScreenPresentedEvent");
        }
        
        void BannerAd_ScreenDismissedEvent() 
        {
            Debug.Log("BannerAd_ScreenDismissedEvent");
        }
        
        void BannerAd_LeftApplicationEvent() 
        {
            Debug.Log("BannerAd_LeftApplicationEvent");
        }

        private void BannerAd_OnAdFailedToLoad(IronSourceError error)
        {
            Debug.Log($"BannerAd_OnAdFailedToLoad:{error.getErrorCode()}-{error.getDescription()}");
        }

        private void BannerAd_OnAdLoaded()
        {
            Debug.Log("BannerAd_OnAdLoaded");
        }

        void BannerAd_OnAdClicked()
        {
            Debug.Log("BannerAd_OnAdClicked");
        }

#endregion
#region full ads

        bool IsFullAdsLoaded()
        {
            return IronSource.Agent.isInterstitialReady() &&
                Application.internetReachability != NetworkReachability.NotReachable;
        }

        void LoadFullAds(Action<AdsEventArgs> loadCallback)
        {
            if (string.IsNullOrEmpty(m_Config.interstitialID))
            {
                AdsStateData.SafeInvoke(loadCallback, AdsEventArgs.FAIL_NO_ID);
                return;
            }

            var adsState = m_FullAdsState;
            adsState.onLoad = loadCallback;

            if (adsState.isLoading)
            {
                return;
            }

            if (IsFullAdsLoaded())
            {
                AdsStateData.SafeInvoke(loadCallback, AdsEventArgs.DONE);
                return;
            }

            adsState.isLoading = true;
            adsState.isLoaded = false;
            adsState.isWaitToShow = false;
            m_FullAdsState = adsState;

            IronSource.Agent.loadInterstitial();
        }

        void ShowFullAds(Action<AdsEventArgs> showCallback)
        {
            var adsState = m_FullAdsState;
            adsState.isWaitToShow = true;

            if (IsFullAdsLoaded())
            {
                adsState.onShow = showCallback;
                IronSource.Agent.showInterstitial();
            }
            else
            {
                AdsStateData.SafeInvoke(showCallback, AdsEventArgs.FAIL);
            }
        }

        private void InterstitialAd_OnAdLoaded()
        {
            AdsStateData.DebugLog("InterstitialAd_OnAdLoaded");
            var adsState = m_FullAdsState;
            adsState.isLoaded = true;
            adsState.isLoading = false;
            adsState.onLoad = AdsStateData.SafeInvoke(adsState.onLoad, AdsEventArgs.DONE);
        }

        private void InterstitialAd_OnAdFailedToLoad(IronSourceError error)
        {
            var adsState = m_FullAdsState;
            AdsStateData.DebugLog($"InterstitialAd_OnAdFailedToLoad : {error.getErrorCode()}-{error.getDescription()}");
            adsState.isLoaded = false;
            adsState.isLoading = false;
            adsState.onLoad = AdsStateData.SafeInvoke(adsState.onLoad, AdsEventArgs.FAIL);

            if (m_LoadFullAdCount < 3)
            {
                m_LoadFullAdCount++;
                Debug.Log("Reload full ad after failed to load:" + m_LoadFullAdCount);
                Load(AdsEnum.INTERSTITIAL, null);
            }
        }

        private void InterstitialAd_OnAdClosed()
        {
            AdsStateData.DebugLog("InterstitialAd_OnAdClosed");
            var adsState = m_FullAdsState;
            adsState.isLoaded = false;
            adsState.isLoading = false;
            adsState.onShow = AdsStateData.SafeInvoke(adsState.onShow, AdsEventArgs.DONE);

            if (m_IsAutoReload)
            {
                Load(AdsEnum.INTERSTITIAL, null);
            }
        }

        private void InterstitialAd_OnAdFailedToDisplay(IronSourceError error)
        {
            AdsStateData.DebugLog("InterstitialAd_OnAdFailedToDisplay");
            var adsState = m_FullAdsState;
            adsState.onShow = AdsStateData.SafeInvoke(adsState.onShow, AdsEventArgs.FAIL);
            adsState.isLoaded = false;
            adsState.isLoading = false;
            if (m_IsAutoReload)
            {
                Load(AdsEnum.INTERSTITIAL, null);
            }
        }

        private void InterstitialAd_ClickedEvent()
        {
            AdsStateData.DebugLog("InterstitialAd_ClickedEvent");
            AdsStateData.SafeInvoke(m_FullAdsState.onClick);
        }

        private void InitializeInterstitialAds()
        {
            IronSourceEvents.onInterstitialAdReadyEvent += InterstitialAd_OnAdLoaded;
            IronSourceEvents.onInterstitialAdLoadFailedEvent += InterstitialAd_OnAdFailedToLoad;
            IronSourceEvents.onInterstitialAdShowFailedEvent += InterstitialAd_OnAdFailedToDisplay;
            IronSourceEvents.onInterstitialAdClosedEvent += InterstitialAd_OnAdClosed;
        }

#endregion
#region rewardAds

        bool IsRewardLoaded()
        {
            return IronSource.Agent.isRewardedVideoAvailable() &&
                Application.internetReachability != NetworkReachability.NotReachable;
        }

        void LoadRewardAds(Action<AdsEventArgs> loadCallback)
        {
            if (string.IsNullOrEmpty(m_Config.rewardID))
            {
                AdsStateData.SafeInvoke(loadCallback, AdsEventArgs.FAIL_NO_ID);
                return;
            }

            var adsState = m_RewardAdsState;
            adsState.onLoad = loadCallback;

            if (adsState.isLoading)
            {
                return;
            }

            if (IsRewardLoaded())
            {
                return;
            }

            adsState.isLoading = true;
            adsState.isLoaded = false;
            m_RewardAdsState = adsState;

        }

        void ShowRewardAds(Action<AdsEventArgs> showCallback)
        {
            var adsState = m_RewardAdsState;

            if (IsRewardLoaded())
            {
                adsState.onShow = showCallback;
                IronSource.Agent.showRewardedVideo();
            }
            else
            {
                AdsStateData.SafeInvoke(showCallback, AdsEventArgs.FAIL_NOT_LOADED);
            }
        }

        private void RewardedAdsView_OnAdFailedToShow(IronSourceError error)
        {
            m_RewardAdsState.isLoading = false;
            m_RewardAdsState.isLoaded = false;
            AdsStateData.DebugLog($"RewardedAdsView_OnAdFailedToShow : {error.getErrorCode()}-{error.getDescription()}");
            m_RewardAdsState.onShow = AdsStateData.SafeInvoke(m_RewardAdsState.onShow, AdsEventArgs.FAIL);
        }

        private void RewardedAdsView_OnUserEarnedReward(IronSourcePlacement placement)
        {
            m_RewardAdsState.onShow = AdsStateData.SafeInvoke(m_RewardAdsState.onShow, AdsEventArgs.DONE_NO_SKIP);
        }

        private void RewardedAdsView_OnAdFailedToLoad(IronSourceError error)
        {
            AdsStateData.DebugLog($"RewardedAdsView_OnAdFailedToLoad : {error.getErrorCode()}-{error.getDescription()}");
            m_RewardAdsState.isLoading = false;
            m_RewardAdsState.isLoaded = false;
            m_RewardAdsState.onLoad = AdsStateData.SafeInvoke(m_RewardAdsState.onLoad, AdsEventArgs.FAIL);

            if (m_LoadRewardCount < 3)
            {
                m_LoadRewardCount++;
                Debug.Log("Reload reward after failed to load:" + m_LoadRewardCount);
                Load(AdsEnum.REWARD, null);
            }
        }

        private void RewardedAdsView_OnAdLoaded()
        {
            AdsStateData.DebugLog("RewardedAdsView_OnAdLoaded");
            m_RewardAdsState.isLoaded = true;
            m_RewardAdsState.isLoading = false;
            m_RewardAdsState.onLoad = AdsStateData.SafeInvoke(m_RewardAdsState.onLoad, AdsEventArgs.DONE);
        }

        private void RewardedAdsView_OnAdClosed()
        {
            AdsStateData.DebugLog("RewardedAdsView_OnAdClosed");
            m_RewardAdsState.isLoading = false;
            m_RewardAdsState.isLoaded = false;
            if (m_IsAutoReload)
            {
                Load(AdsEnum.REWARD, null);
            }
            else
            {
                m_RewardAdsState.isLoaded = false;
            }
        }

        private void InitializeRewardedAds()
        {
            // Attach callback
            IronSourceEvents.onRewardedVideoAdReadyEvent += RewardedAdsView_OnAdLoaded;
            IronSourceEvents.onRewardedVideoAdLoadFailedEvent += RewardedAdsView_OnAdFailedToLoad;
            IronSourceEvents.onRewardedVideoAdShowFailedEvent += RewardedAdsView_OnAdFailedToShow;
            IronSourceEvents.onRewardedVideoAdClosedEvent += RewardedAdsView_OnAdClosed;
            IronSourceEvents.onRewardedVideoAdRewardedEvent += RewardedAdsView_OnUserEarnedReward;
            // MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;
        }

#endregion
    }
}
#endif
