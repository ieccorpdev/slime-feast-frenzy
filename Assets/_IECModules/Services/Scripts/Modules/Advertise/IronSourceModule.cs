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

            Debug.LogWarning("Applovin App ID:" + config.appID);
            Debug.LogWarning("Applovin Banner ID:" + config.bannerID);
            Debug.LogWarning("Applovin Inter ID:" + config.interstitialID);
            Debug.LogWarning("Applovin Reward ID:" + config.rewardID);

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
                InitBanner();
                InitializeInterstitialAds();
                InitializeRewardedAds();
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
            // MaxSdkCallbacks.Banner.OnAdClickedEvent += BannerAd_OnAdClicked;
            // MaxSdkCallbacks.Banner.OnAdLoadedEvent += BannerAd_OnAdLoaded;
            // MaxSdkCallbacks.Banner.OnAdLoadFailedEvent += BannerAd_OnAdFailedToLoad;
            // MaxSdkCallbacks.Banner.OnAdCollapsedEvent += BannerAd_OnAdCollapsed;
            // MaxSdkCallbacks.Banner.OnAdExpandedEvent += BannerAd_OnAdExpanded;
            // MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;

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
    }
}
#endif
