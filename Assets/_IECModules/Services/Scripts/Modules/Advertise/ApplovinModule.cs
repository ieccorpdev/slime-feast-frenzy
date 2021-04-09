#if (PluginApplovin)
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public class ApplovinModule : IAdsService
    {
        public const string APPLOVIN_VERBOSE_LOGGING = "APPLOVIN_VERBOSE_LOGGING";

        AdsPlatformConfig m_Config;
        double m_Ecpm;
        bool m_IsAutoReload;
        bool m_IsAcceptConsent;
        bool m_IsInit = false;

        int m_LoadFullAdCount = 0;
        int m_LoadRewardCount = 0;

        //AdsStateData m_BannerAdsState = new AdsStateData();
        AdsStateData m_FullAdsState = new AdsStateData();
        AdsStateData m_RewardAdsState = new AdsStateData();

        MaxSdkBase.BannerPosition m_BannerAdsPos = MaxSdkBase.BannerPosition.BottomCenter;


        public void Init(AdsConfig adsConfig, bool isAcceptConsent, bool isAutoReload, Action<bool> afterInitCallback)
        {
            var config = (Application.platform == RuntimePlatform.Android) ? adsConfig.android : adsConfig.ios;
            config.appID = config.appID.Trim();
            config.bannerID = config.bannerID.Trim();
            config.interstitialID = config.interstitialID.Trim();
            config.rewardID = config.rewardID.Trim();

            if (string.IsNullOrEmpty(config.appID.Trim()) == false)
            {
                MaxSdk.SetSdkKey(config.appID);
                if (PlayerPrefs.GetInt(APPLOVIN_VERBOSE_LOGGING, 0) != 0)
                {
                    MaxSdk.SetVerboseLogging(true);
                }
                MaxSdk.InitializeSdk();

                MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration sdkConfiguration) =>
                {
                    m_IsInit = true;
                    MaxSdk.SetHasUserConsent(true);
                    InitBanner();
                    InitializeInterstitialAds();
                    InitializeRewardedAds();
                    afterInitCallback?.Invoke(true);
                };
            }

            m_Config = config;
            m_IsAutoReload = isAutoReload;
            m_IsAcceptConsent = isAcceptConsent;
        }

        public double GetEcpm(AdsEnum adsType)
        {
            return 0;
            //throw new NotImplementedException();
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

            return "ApplovinAds";
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
                    MaxSdk.HideBanner(m_Config.bannerID);
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
            float f = Screen.dpi / 160f;
            if (MaxSdkUtils.IsTablet())
            {
                return 90f * f;
            }
            return 50f * f;
        }

        #region bannerAds

        void LoadBannerAds(Action<AdsEventArgs> loadCallback, AdsEnum bannerPos)
        {
            if (string.IsNullOrEmpty(m_Config.bannerID))
            {
                AdsStateData.SafeInvoke(loadCallback, AdsEventArgs.FAIL_NO_ID);
                return;
            }

            MaxSdk.CreateBanner(m_Config.bannerID, ConvertBannerPos(bannerPos));
#if UNITY_ANDROID || UNITY_IOS
            MaxSdk.SetBannerBackgroundColor(m_Config.bannerID, Color.white);
#endif
            AdsStateData.SafeInvoke(loadCallback, AdsEventArgs.DONE);
        }

        void ShowBanner(Action<AdsEventArgs> showCallback, AdsEnum bannerPos)
        {
            MaxSdk.ShowBanner(m_Config.bannerID);
            AdsStateData.SafeInvoke(showCallback, AdsEventArgs.DONE);
        }

        MaxSdkBase.BannerPosition ConvertBannerPos(AdsEnum bannerPos)
        {
            switch (bannerPos)
            {
                case AdsEnum.BANNER_BOTTOM:
                    return MaxSdkBase.BannerPosition.BottomCenter;
                case AdsEnum.BANNER_TOP:
                    return MaxSdkBase.BannerPosition.TopCenter;
                default:
                    return MaxSdkBase.BannerPosition.BottomCenter;
            }
        }

        void InitBanner()
        {
            MaxSdkCallbacks.OnBannerAdClickedEvent += BannerAd_OnAdClicked;
            MaxSdkCallbacks.OnBannerAdLoadedEvent += BannerAd_OnAdLoaded;
            MaxSdkCallbacks.OnBannerAdLoadFailedEvent += BannerAd_OnAdFailedToLoad;
            MaxSdkCallbacks.OnBannerAdCollapsedEvent += BannerAd_OnAdCollapsed;
            MaxSdkCallbacks.OnBannerAdExpandedEvent += BannerAd_OnAdExpanded;
        }

        private void BannerAd_OnAdExpanded(string adUnitId)
        {
            Debug.Log("BannerAd_OnAdExpanded");
        }

        private void BannerAd_OnAdCollapsed(string adUnitId)
        {
            Debug.Log("BannerAd_OnAdCollapsed");
        }

        private void BannerAd_OnAdFailedToLoad(string adUnitId, int errorCode)
        {
            Debug.Log("BannerAd_OnAdFailedToLoad:" + errorCode);
        }

        private void BannerAd_OnAdLoaded(string adUnitId)
        {
            Debug.Log("BannerAd_OnAdLoaded");
        }

        void BannerAd_OnAdClicked(string adUnitId)
        {
            Debug.Log("BannerAd_OnAdClicked");
        }

        #endregion
        #region full ads

        bool IsFullAdsLoaded()
        {
            return MaxSdk.IsInterstitialReady(m_Config.interstitialID) &&
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

            MaxSdk.LoadInterstitial(m_Config.interstitialID);
        }

        void ShowFullAds(Action<AdsEventArgs> showCallback)
        {
            var adsState = m_FullAdsState;
            adsState.isWaitToShow = true;

            if (IsFullAdsLoaded())
            {
                adsState.onShow = showCallback;
                MaxSdk.ShowInterstitial(m_Config.interstitialID);
            }
            else
            {
                AdsStateData.SafeInvoke(showCallback, AdsEventArgs.FAIL);
            }
        }

        private void InterstitialAd_OnAdLoaded(string adUnitId)
        {
            AdsStateData.DebugLog("InterstitialAd_OnAdLoaded");
            var adsState = m_FullAdsState;
            adsState.isLoaded = true;
            adsState.isLoading = false;
            adsState.onLoad = AdsStateData.SafeInvoke(adsState.onLoad, AdsEventArgs.DONE);
        }

        private void InterstitialAd_OnAdFailedToLoad(string adUnitId, int errorCode)
        {
            var adsState = m_FullAdsState;
            AdsStateData.DebugLog("InterstitialAd_OnAdFailedToLoad : " + errorCode);
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

        private void InterstitialAd_OnAdClosed(string adUnitId)
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

        private void InterstitialAd_OnAdFailedToDisplay(string adUnitId, int errorCode)
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

        private void InterstitialAd_OnAdLeavingApplication(object sender, EventArgs e)
        {
            AdsStateData.DebugLog("InterstitialAd_OnAdLeavingApplication");
            AdsStateData.SafeInvoke(m_FullAdsState.onClick);
        }

        public void InitializeInterstitialAds()
        {
            MaxSdkCallbacks.OnInterstitialLoadedEvent += InterstitialAd_OnAdLoaded;
            MaxSdkCallbacks.OnInterstitialLoadFailedEvent += InterstitialAd_OnAdFailedToLoad;
            MaxSdkCallbacks.OnInterstitialAdFailedToDisplayEvent += InterstitialAd_OnAdFailedToDisplay;
            MaxSdkCallbacks.OnInterstitialHiddenEvent += InterstitialAd_OnAdClosed;
        }

        #endregion

        #region rewardAds

        bool IsRewardLoaded()
        {
            return MaxSdk.IsRewardedAdReady(m_Config.rewardID) &&
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

            MaxSdk.LoadRewardedAd(m_Config.rewardID);
        }

        void ShowRewardAds(Action<AdsEventArgs> showCallback)
        {
            var adsState = m_RewardAdsState;

            if (IsRewardLoaded())
            {
                adsState.onShow = showCallback;
                MaxSdk.ShowRewardedAd(m_Config.rewardID);
            }
            else
            {
                AdsStateData.SafeInvoke(showCallback, AdsEventArgs.FAIL_NOT_LOADED);
            }
        }

        private void RewardedAdsView_OnAdFailedToShow(string adUnitId, int errorCode)
        {
            m_RewardAdsState.isLoading = false;
            m_RewardAdsState.isLoaded = false;
            AdsStateData.DebugLog("RewardedAdsView_OnAdFailedToShow : " + errorCode);
            m_RewardAdsState.onShow = AdsStateData.SafeInvoke(m_RewardAdsState.onShow, AdsEventArgs.FAIL);
        }

        private void RewardedAdsView_OnUserEarnedReward(string adUnitId, MaxSdk.Reward reward)
        {
            m_RewardAdsState.onShow = AdsStateData.SafeInvoke(m_RewardAdsState.onShow, AdsEventArgs.DONE_NO_SKIP);
        }

        private void RewardedAdsView_OnAdFailedToLoad(string adUnitId, int errorCode)
        {
            AdsStateData.DebugLog("RewardedAdsView_OnAdFailedToLoad : " + errorCode);
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

        private void RewardedAdsView_OnAdLoaded(string adUnitId)
        {
            AdsStateData.DebugLog("RewardedAdsView_OnAdLoaded");
            m_RewardAdsState.isLoaded = true;
            m_RewardAdsState.isLoading = false;
            m_RewardAdsState.onLoad = AdsStateData.SafeInvoke(m_RewardAdsState.onLoad, AdsEventArgs.DONE);
        }

        private void RewardedAdsView_OnAdClosed(string adUnitId)
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

        public void InitializeRewardedAds()
        {
            // Attach callback
            MaxSdkCallbacks.OnRewardedAdLoadedEvent += RewardedAdsView_OnAdLoaded;
            MaxSdkCallbacks.OnRewardedAdLoadFailedEvent += RewardedAdsView_OnAdFailedToLoad;
            MaxSdkCallbacks.OnRewardedAdFailedToDisplayEvent += RewardedAdsView_OnAdFailedToShow;
            MaxSdkCallbacks.OnRewardedAdHiddenEvent += RewardedAdsView_OnAdClosed;
            MaxSdkCallbacks.OnRewardedAdReceivedRewardEvent += RewardedAdsView_OnUserEarnedReward;
        }

        #endregion

        public void ResetLoadCount()
        {
            m_LoadFullAdCount = 0;
            m_LoadRewardCount = 0;
        }
    }
}
#endif
