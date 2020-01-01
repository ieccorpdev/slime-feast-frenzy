#if (PluginGoogleAds)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GoogleMobileAds.Api;

namespace Root
{
    public class AdmobModule : IAdsService
    {
        AdsPlatformConfig m_Config;
        double m_Ecpm;
        bool m_IsAutoReload;
        bool m_IsAcceptConsent;

        BannerView m_BannerAdsView = null;
        InterstitialAd m_FullAdsView = null;
        RewardedAd m_RewardAdsView = null;

        AdsStateData m_BannerAdsState = new AdsStateData();
        AdsStateData m_FullAdsState = new AdsStateData();
        AdsStateData m_RewardAdsState = new AdsStateData();

        AdPosition m_BannerAdsPos = AdPosition.Bottom;

        public void Init(AdsConfig adsConfig, bool isAcceptConsent, bool isAutoReload, double ecpm)
        {
            MobileAds.SetiOSAppPauseOnBackground(true);

            var config = (Application.platform == RuntimePlatform.Android) ? adsConfig.android : adsConfig.ios;
            config.appID = config.appID.Trim();
            config.bannerID = config.bannerID.Trim();
            config.interstitialID = config.interstitialID.Trim();
            config.rewardID = config.rewardID.Trim();

            if (string.IsNullOrEmpty(config.appID.Trim()) == false)
            {
                MobileAds.Initialize(config.appID);
            }

            m_Config = config;
            m_Ecpm = ecpm;
            m_IsAutoReload = isAutoReload;
            m_IsAcceptConsent = isAcceptConsent;
        }

        public double GetEcpm(AdsEnum adsType)
        {
            return m_Ecpm;
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

            return "GoogleMobileAds";
        }

        public void Hide(AdsEnum adsType)
        {
            switch (adsType)
            {
                case AdsEnum.BANNER_TOP:
                case AdsEnum.BANNER_BOTTOM:
                    if (m_BannerAdsView != null)
                    {
                        m_BannerAdsView.Hide();
                    }

                    break;
            }
        }

        public bool IsAcceptGDPR()
        {
            return m_IsAcceptConsent;
        }

        public bool IsLoaded(AdsEnum adsType)
        {
            switch (adsType)
            {
                case AdsEnum.BANNER_TOP:
                case AdsEnum.BANNER_BOTTOM:
                    return m_BannerAdsState.isLoaded;
                case AdsEnum.INTERSTITIAL:
                    return m_FullAdsState.isLoaded;
                case AdsEnum.REWARD:
                    return m_RewardAdsState.isLoaded;
            }

            return false;
        }

        public void Load(AdsEnum adsType, Action<AdsEventArgs> loadCallback)
        {
            switch (adsType)
            {
                case AdsEnum.BANNER_TOP:
                    m_BannerAdsPos = AdPosition.Top;
                    LoadBannerAds(loadCallback);
                    break;
                case AdsEnum.BANNER_BOTTOM:
                    m_BannerAdsPos = AdPosition.Bottom;
                    LoadBannerAds(loadCallback);
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
            switch (adsType)
            {
                case AdsEnum.BANNER_TOP:
                    m_BannerAdsPos = AdPosition.Top;
                    ShowBanner(showCallback);
                    break;
                case AdsEnum.BANNER_BOTTOM:
                    m_BannerAdsPos = AdPosition.Bottom;
                    ShowBanner(showCallback);
                    break;
                case AdsEnum.INTERSTITIAL:
                    ShowFullAds(showCallback);
                    break;
                case AdsEnum.REWARD:
                    ShowRewardAds(showCallback);
                    break;
            }
        }

        AdRequest GetAdRequest()
        {
            AdRequest request;
            if (m_IsAcceptConsent)
            {
                request = new AdRequest.Builder().Build();
            }
            else
            {
                request = new AdRequest.Builder().AddExtra("npa", "1").Build();
            }

            return request;
        }

        #region bannerAds

        void LoadBannerAds(Action<AdsEventArgs> loadCallback)
        {
            if (string.IsNullOrEmpty(m_Config.bannerID))
            {
                AdsStateData.SafeInvoke(loadCallback, AdsEventArgs.FAIL_NO_ID);
                return;
            }

            var adsView = m_BannerAdsView;
            var adsState = m_BannerAdsState;
            adsState.onLoad = loadCallback;

            if (adsState.isLoading)
            {
                return;
            }

            if (adsView != null || adsState.isLoaded)
            {
                AdsStateData.SafeInvoke(loadCallback, AdsEventArgs.DONE);
                return;
            }

            adsView = new BannerView(m_Config.bannerID, AdSize.Banner, AdPosition.Bottom);
            adsView.OnAdLoaded += BannerView_OnAdLoaded;
            adsView.OnAdFailedToLoad += BannerView_OnAdFailedToLoad;
            adsView.OnAdLeavingApplication += BannerView_OnAdLeavingApplication;

            adsState.isLoading = true;
            adsState.isLoaded = false;
            adsState.isWaitToShow = false;
            m_BannerAdsView = adsView;
            m_BannerAdsState = adsState;

            adsView.LoadAd(GetAdRequest());
        }

        void ShowBanner(Action<AdsEventArgs> showCallback)
        {
            var adsView = m_BannerAdsView;
            var adsState = m_BannerAdsState;
            adsState.isWaitToShow = true;

            if (adsView != null)
            {
                if (adsState.isLoaded)
                {
                    adsView.SetPosition(m_BannerAdsPos);
                    adsView.Show();
                    AdsStateData.SafeInvoke(showCallback, AdsEventArgs.DONE);
                }
                else
                {
                    adsState.onShow = showCallback;
                }
            }
            else
            {
                AdsStateData.SafeInvoke(showCallback, AdsEventArgs.FAIL);
            }
        }

        private void BannerView_OnAdLoaded(object sender, EventArgs e)
        {
            var adsState = m_BannerAdsState;
            adsState.isLoaded = true;
            adsState.isLoading = false;
            adsState.onLoad = AdsStateData.SafeInvoke(adsState.onLoad, AdsEventArgs.DONE);

            if (adsState.isWaitToShow)
            {
                ShowBanner(adsState.onShow);
            }
            else
            {
                Hide(AdsEnum.BANNER_BOTTOM);
            }
        }

        private void BannerView_OnAdFailedToLoad(object sender, AdFailedToLoadEventArgs e)
        {
            var adstate = m_BannerAdsState;
            AdsStateData.DebugLog("BannerView_OnAdFailedToLoad : " + e.Message);
            adstate.isLoading = false;
            adstate.onLoad = AdsStateData.SafeInvoke(adstate.onLoad, AdsEventArgs.FAIL);
        }

        private void BannerView_OnAdLeavingApplication(object sender, EventArgs e)
        {
            var adstate = m_BannerAdsState;
            AdsStateData.DebugLog("BannerView_OnAdLeavingApplication");
            AdsStateData.SafeInvoke(adstate.onClick);
        }

        #endregion

        #region fullAds

        void LoadFullAds(Action<AdsEventArgs> loadCallback)
        {
            if (string.IsNullOrEmpty(m_Config.bannerID))
            {
                AdsStateData.SafeInvoke(loadCallback, AdsEventArgs.FAIL_NO_ID);
                return;
            }

            var adsView = m_FullAdsView;
            var adsState = m_FullAdsState;
            adsState.onLoad = loadCallback;

            if (adsState.isLoading)
            {
                return;
            }

            if (adsView != null)
            {
                adsView.Destroy();
            }

            adsView = new InterstitialAd(m_Config.bannerID);
            adsView.OnAdClosed += InterstitialAd_OnAdClosed;
            adsView.OnAdLoaded += InterstitialAd_OnAdLoaded;
            adsView.OnAdFailedToLoad += InterstitialAd_OnAdFailedToLoad;
            adsView.OnAdLeavingApplication += InterstitialAd_OnAdLeavingApplication;

            adsState.isLoading = true;
            adsState.isLoaded = false;
            adsState.isWaitToShow = false;
            m_FullAdsView = adsView;
            m_FullAdsState = adsState;

            adsView.LoadAd(GetAdRequest());
        }

        void ShowFullAds(Action<AdsEventArgs> showCallback)
        {
            var adsView = m_FullAdsView;
            var adsState = m_FullAdsState;
            adsState.isWaitToShow = true;

            if (adsView != null)
            {
                if (adsState.isLoaded)
                {
                    adsView.Show();
                    AdsStateData.SafeInvoke(showCallback, AdsEventArgs.DONE);
                }
                else
                {
                    AdsStateData.SafeInvoke(showCallback, AdsEventArgs.FAIL);
                }
            }
            else
            {
                AdsStateData.SafeInvoke(showCallback, AdsEventArgs.FAIL);
            }
        }

        private void InterstitialAd_OnAdLoaded(object sender, EventArgs e)
        {
            var adsState = m_FullAdsState;
            adsState.isLoaded = true;
            adsState.isLoading = false;
            adsState.onLoad = AdsStateData.SafeInvoke(adsState.onLoad, AdsEventArgs.DONE);
        }

        private void InterstitialAd_OnAdFailedToLoad(object sender, AdFailedToLoadEventArgs e)
        {
            var adsState = m_FullAdsState;
            AdsStateData.DebugLog("InterstitialAd_OnAdFailedToLoad : " + e.Message);
            adsState.isLoading = false;
            adsState.onLoad = AdsStateData.SafeInvoke(adsState.onLoad, AdsEventArgs.FAIL);
        }

        private void InterstitialAd_OnAdClosed(object sender, EventArgs args)
        {
            var adsView = m_FullAdsView;
            var adsState = m_FullAdsState;
            adsState.onShow = AdsStateData.SafeInvoke(adsState.onShow, AdsEventArgs.DONE);

            if (m_IsAutoReload)
            {
                Load(AdsEnum.INTERSTITIAL, null);
            }
            else if (adsView != null)
            {
                adsView.Destroy();
                m_FullAdsView = null;
                adsState.isLoaded = false;
            }
        }

        private void InterstitialAd_OnAdLeavingApplication(object sender, EventArgs e)
        {
            AdsStateData.DebugLog("InterstitialAd_OnAdLeavingApplication");
            AdsStateData.SafeInvoke(m_FullAdsState.onClick);
        }

        #endregion

        #region rewardAds

        void LoadRewardAds(Action<AdsEventArgs> loadCallback)
        {
            if (string.IsNullOrEmpty(m_Config.bannerID))
            {
                AdsStateData.SafeInvoke(loadCallback, AdsEventArgs.FAIL_NO_ID);
                return;
            }

            var adsView = m_RewardAdsView;
            var adsState = m_RewardAdsState;
            adsState.onLoad = loadCallback;

            if (adsState.isLoading)
            {
                return;
            }

            adsView = new RewardedAd(m_Config.bannerID);
            adsView.OnAdClosed += RewardedAdsView_OnAdClosed;
            adsView.OnAdFailedToShow += RewardedAdsView_OnAdFailedToShow;
            adsView.OnUserEarnedReward += RewardedAdsView_OnUserEarnedReward;
            adsView.OnAdLoaded += RewardedAdsView_OnAdLoaded;
            adsView.OnAdFailedToLoad += RewardedAdsView_OnAdFailedToLoad;

            adsState.isLoading = true;
            adsState.isLoaded = false;
            m_RewardAdsView = adsView;
            m_RewardAdsState = adsState;

            adsView.LoadAd(GetAdRequest());
        }

        void ShowRewardAds(Action<AdsEventArgs> showCallback)
        {
            var adsView = m_RewardAdsView;
            var adsState = m_RewardAdsState;

            if (adsView != null)
            {
                if (adsState.isLoaded)
                {
                    adsState.onShow = showCallback;
                    adsView.Show();
                }
                else
                {
                    AdsStateData.SafeInvoke(showCallback, AdsEventArgs.FAIL_NOT_LOADED);
                }
            }
            else
            {
                AdsStateData.SafeInvoke(showCallback, AdsEventArgs.FAIL);
            }
        }

        private void RewardedAdsView_OnAdFailedToShow(object sender, AdErrorEventArgs e)
        {
            AdsStateData.DebugLog("RewardedAdsView_OnAdFailedToShow : " + e.Message);
            m_RewardAdsState.onShow = AdsStateData.SafeInvoke(m_RewardAdsState.onShow, AdsEventArgs.DONE_SKIP);
        }

        private void RewardedAdsView_OnUserEarnedReward(object sender, Reward e)
        {
            m_RewardAdsState.onShow = AdsStateData.SafeInvoke(m_RewardAdsState.onShow, AdsEventArgs.DONE_NO_SKIP);
        }

        private void RewardedAdsView_OnAdFailedToLoad(object sender, AdErrorEventArgs e)
        {
            AdsStateData.DebugLog("RewardedAdsView_OnAdFailedToLoad : " + e.Message);
            m_RewardAdsState.isLoading = false;
            m_RewardAdsState.onLoad = AdsStateData.SafeInvoke(m_RewardAdsState.onLoad, AdsEventArgs.FAIL);
        }

        private void RewardedAdsView_OnAdLoaded(object sender, EventArgs e)
        {
            m_RewardAdsState.isLoaded = true;
            m_RewardAdsState.isLoading = false;
            m_RewardAdsState.onLoad = AdsStateData.SafeInvoke(m_RewardAdsState.onLoad, AdsEventArgs.DONE);
        }

        private void RewardedAdsView_OnAdClosed(object sender, EventArgs e)
        {
            if (m_IsAutoReload)
            {
                Load(AdsEnum.REWARD, null);
            }
            else
            {
                m_RewardAdsState.isLoaded = false;
            }
        }

        #endregion
    }
}

#endif