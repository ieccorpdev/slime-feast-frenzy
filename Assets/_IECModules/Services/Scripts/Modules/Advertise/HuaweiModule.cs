using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HmsPlugin;
using HuaweiConstants;
using HuaweiMobileServices.Ads;
using static HuaweiConstants.UnityBannerAdPositionCode;

namespace IEC
{
    public class HuaweiModule : IAdsService
    {
        private const string TEST_BANNER_AD_ID = "testw6vs28auh3";
        private const string TEST_INTERSTITIAL_AD_ID = "testb4znbuh3n2";
        private const string TEST_REWARDED_AD_ID = "testx9dtjwj8hp";

        private bool m_IsInit = false;
        private BannerAd m_BannerView;
        private InterstitialAd m_InterstitialView;
        private RewardAd m_RewardView;
        private HMSSettings m_AdsKitSetting;

        AdsStateData m_FullAdsState = new AdsStateData();
        AdsStateData m_RewardAdsState = new AdsStateData();
        private bool m_IsBannerAdLoaded;

        public void Init()
        {
            HwAds.Init();
            m_IsInit = true;
            m_AdsKitSetting = HMSAdsKitSettings.Instance.Settings;
        }

        public float GetBannerHeight()
        {
            return 0f;
        }

        public double GetEcpm(AdsEnum adsType)
        {
            return 0f;
        }

        public string GetInfo(AdsEnum adsType)
        {
            switch (adsType)
            {
                case AdsEnum.BANNER_TOP:
                case AdsEnum.BANNER_BOTTOM:
                    return m_AdsKitSetting.GetBool(HMSAdsKitSettings.UseTestAds) ? TEST_BANNER_AD_ID : m_AdsKitSetting.Get(HMSAdsKitSettings.BannerAdID);
                case AdsEnum.INTERSTITIAL:
                    return m_AdsKitSetting.GetBool(HMSAdsKitSettings.UseTestAds) ? TEST_INTERSTITIAL_AD_ID : m_AdsKitSetting.Get(HMSAdsKitSettings.InterstitialAdID);
                case AdsEnum.REWARD:
                    return m_AdsKitSetting.GetBool(HMSAdsKitSettings.UseTestAds) ? TEST_REWARDED_AD_ID : m_AdsKitSetting.Get(HMSAdsKitSettings.RewardedAdID);
            }
            return "HuaweiAds";
        }

        public void Hide(AdsEnum adsType)
        {
            if (!m_IsInit) return;
            if (m_BannerView == null) return;
            m_BannerView.HideBanner();
        }

        public bool IsAcceptGDPR()
        {
            return true;
        }

        public bool IsLoaded(AdsEnum adsType)
        {
            if (!m_IsInit) return false;
            switch (adsType)
            {
                case AdsEnum.BANNER_TOP:
                case AdsEnum.BANNER_BOTTOM:
                    return true;
                case AdsEnum.INTERSTITIAL:
                    return IsFullAdsLoaded();
                case AdsEnum.REWARD:
                    return IsRewardAdsLoaded();
            }
            return false;
        }

        public void Load(AdsEnum adsType, Action<AdsEventArgs> loadCallback)
        {
            if (m_IsInit)
            {
                AdsStateData.SafeInvoke(loadCallback, AdsEventArgs.FAIL);
                return;
            }
            switch (adsType)
            {
                case AdsEnum.BANNER_BOTTOM:
                case AdsEnum.BANNER_TOP:
                    LoadBannerAd(adsType == AdsEnum.BANNER_BOTTOM ? UnityBannerAdPositionCodeType.POSITION_BOTTOM : UnityBannerAdPositionCodeType.POSITION_TOP);
                    break;
                case AdsEnum.INTERSTITIAL:
                    LoadInterstitialAd();
                    break;
                case AdsEnum.REWARD:
                    LoadRewardedAd();
                    break;
            }
        }

        public void SetOnClick(AdsEnum adsType, Action clickCallback)
        {
            if (!m_IsInit) return;
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
                    ShowBannerAd();
                    break;
                case AdsEnum.INTERSTITIAL:
                    ShowInterstitialAd();
                    break;
                case AdsEnum.REWARD:
                    ShowRewardedAd();
                    break;
            }
        }

        #region BANNER

        #region PUBLIC METHODS

        public void LoadBannerAd(UnityBannerAdPositionCodeType position, string bannerSize = UnityBannerAdSize.BANNER_SIZE_320_50)
        {
            if (m_IsInit || !m_AdsKitSetting.GetBool(HMSAdsKitSettings.EnableBannerAd)) return;
            Debug.Log("[HMS] HMSAdsKitManager Loading Banner Ad.");
            var bannerAdStatusListener = new AdStatusListener();
            bannerAdStatusListener.mOnAdLoaded += BannerAdStatusListener_mOnAdLoaded;
            bannerAdStatusListener.mOnAdClosed += BannerAdStatusListener_mOnAdClosed;
            bannerAdStatusListener.mOnAdImpression += BannerAdStatusListener_mOnAdImpression;
            bannerAdStatusListener.mOnAdClicked += BannerAdStatusListener_mOnAdClicked;
            bannerAdStatusListener.mOnAdOpened += BannerAdStatusListener_mOnAdOpened;
            bannerAdStatusListener.mOnAdFailed += BannerAdStatusListener_mOnAdFailed;

            m_BannerView = new BannerAd(bannerAdStatusListener);
            m_BannerView.AdId = m_AdsKitSetting.GetBool(HMSAdsKitSettings.UseTestAds) ? TEST_BANNER_AD_ID : m_AdsKitSetting.Get(HMSAdsKitSettings.BannerAdID);
            m_BannerView.PositionType = (int)position;
            m_BannerView.SizeType = bannerSize;
            m_BannerView.AdStatusListener = bannerAdStatusListener;
            var bannerRefreshInterval = m_AdsKitSetting.Get(HMSAdsKitSettings.BannerRefreshInterval);
            if (string.IsNullOrEmpty(bannerRefreshInterval))
            {
                m_BannerView.BannerRefresh = long.Parse(bannerRefreshInterval);
            }
            m_IsBannerAdLoaded = false;
            m_BannerView.LoadBanner(new AdParam.Builder().Build());
        }

        public void ShowBannerAd()
        {
            if (m_BannerView == null)
            {
                Debug.LogError("[HMS] HMSAdsKitManager Banner Ad is Null.");
                return;
            }
            m_BannerView.ShowBanner();
        }

        #endregion

        #region LISTENERS

        public event Action OnBannerLoadEvent;
        public event Action OnBannerFailedToLoadEvent;

        private void BannerAdStatusListener_mOnAdFailed(object sender, AdLoadErrorCodeEventArgs e)
        {
            Debug.LogError("[HMS] HMSAdsKitManager BannerAdLoadFailed. Error Code: " + e.ErrorCode);
            OnBannerFailedToLoadEvent?.Invoke();
        }

        private void BannerAdStatusListener_mOnAdOpened(object sender, EventArgs e)
        {
            Debug.Log("[HMS] HMSAdsKitManager BannerAdOpened : ");
        }

        private void BannerAdStatusListener_mOnAdClicked(object sender, EventArgs e)
        {
            Debug.Log("[HMS] HMSAdsKitManager BannerAdClicked : ");
        }

        private void BannerAdStatusListener_mOnAdImpression(object sender, EventArgs e)
        {
            Debug.Log("[HMS] HMSAdsKitManager BannerAdImpression : ");
        }

        private void BannerAdStatusListener_mOnAdClosed(object sender, EventArgs e)
        {
            Debug.Log("[HMS] HMSAdsKitManager BannerAdClosed : ");
        }

        private void BannerAdStatusListener_mOnAdLoaded(object sender, EventArgs e)
        {
            Debug.Log("[HMS] HMSAdsKitManager BannerAdLoadSuccess : ");
            m_IsBannerAdLoaded = true;
            OnBannerLoadEvent?.Invoke();
        }

        #endregion

        #endregion

        #region INTERSTITIAL

        private bool IsFullAdsLoaded()
        {
            if (m_InterstitialView == null) return false;
            return m_InterstitialView.Loaded;
        }

        #region PUBLIC METHODS

        public void LoadInterstitialAd()
        {
            if (!m_IsInit || !m_AdsKitSetting.GetBool(HMSAdsKitSettings.EnableInterstitialAd)) return;
            Debug.Log("[HMS] HMSAdsKitManager Loading Interstitial Ad.");
            m_InterstitialView = new InterstitialAd
            {
                AdId = m_AdsKitSetting.GetBool(HMSAdsKitSettings.UseTestAds) ? TEST_INTERSTITIAL_AD_ID : m_AdsKitSetting.Get(HMSAdsKitSettings.InterstitialAdID),
                AdListener = new InterstitialAdListener(this)
            };
            m_InterstitialView.LoadAd(new AdParam.Builder().Build());
        }

        public void ShowInterstitialAd()
        {
            Debug.Log("[HMS] HMSAdsKitManager ShowInterstitialAd called");
            if (m_InterstitialView?.Loaded == true)
            {
                Debug.Log("[HMS] HMSAdsKitManager Showing Interstitial Ad");
                m_InterstitialView.Show();
            }
            else
                Debug.LogError("[HMS] HMSAdsKitManager Interstitial Ad Still Not Loaded Yet!");
        }

        #endregion

        #region LISTENERS

        private class InterstitialAdListener : IAdListener
        {
            private readonly HuaweiModule m_HuaweiModule; 

            public InterstitialAdListener(HuaweiModule huaweiModule)
            {
                m_HuaweiModule = huaweiModule;
            }

            public void OnAdClicked()
            {
                Debug.Log("[HMS] HMSAdsKitManager OnInterstitialAdClicked");
                m_HuaweiModule.OnInterstitialAdClicked?.Invoke();
            }

            public void OnAdClosed()
            {
                Debug.Log("[HMS] HMSAdsKitManager OnInterstitialAdClosed");
                m_HuaweiModule.OnInterstitialAdClosed?.Invoke();
                m_HuaweiModule.LoadInterstitialAd();
            }

            public void OnAdFailed(int reason)
            {
                Debug.Log("[HMS] HMSAdsKitManager OnInterstitialAdFailed");
                m_HuaweiModule.OnInterstitialAdFailed?.Invoke(reason);
            }

            public void OnAdImpression()
            {
                Debug.Log("[HMS] HMSAdsKitManager OnInterstitialAdImpression");
                m_HuaweiModule.OnInterstitialAdImpression?.Invoke();
            }

            public void OnAdLeave()
            {
                Debug.Log("[HMS] HMSAdsKitManager OnInterstitialAdLeave");
                m_HuaweiModule.OnInterstitialAdLeave?.Invoke();
            }

            public void OnAdLoaded()
            {
                Debug.Log("[HMS] HMSAdsKitManager OnInterstitialAdLoaded");
                m_HuaweiModule.OnInterstitialAdLoaded?.Invoke();
            }

            public void OnAdOpened()
            {
                Debug.Log("[HMS] HMSAdsKitManager OnInterstitialAdOpened");
                m_HuaweiModule.OnInterstitialAdOpened?.Invoke();
            }
        }

        public Action OnInterstitialAdClicked { get; set; }
        public Action OnInterstitialAdClosed { get; set; }
        public Action<int> OnInterstitialAdFailed { get; set; }
        public Action OnInterstitialAdImpression { get; set; }
        public Action OnInterstitialAdLeave { get; set; }
        public Action OnInterstitialAdLoaded { get; set; }
        public Action OnInterstitialAdOpened { get; set; }

        #endregion

        #endregion

        #region REWARDED

        private bool IsRewardAdsLoaded()
        {
            if (m_RewardView == null) return false;
            return m_RewardView.Loaded;
        }

        #region PUBLIC METHODS

        public void LoadRewardedAd()
        {
            if (!m_IsInit || !m_AdsKitSetting.GetBool(HMSAdsKitSettings.EnableRewardedAd)) return;
            Debug.Log("[HMS] HMSAdsKitManager LoadRewardedAd");
            m_RewardView = new RewardAd(m_AdsKitSetting.GetBool(HMSAdsKitSettings.UseTestAds) ? TEST_REWARDED_AD_ID : m_AdsKitSetting.Get(HMSAdsKitSettings.RewardedAdID));
            m_RewardView.RewardAdListener = new RewardAdListener(this);
            m_RewardView.LoadAd(new AdParam.Builder().Build());
        }

        public void ShowRewardedAd()
        {
            Debug.Log("[HMS] HMSAdsKitManager ShowRewardedAd called");
            if (m_RewardView?.Loaded == true)
            {
                Debug.Log("[HMS] HMSAdsKitManager Showing Rewarded Ad");
                m_RewardView.Show();
            }
            else
            {
                Debug.LogError("[HMS] HMSAdsKitManager Rewarded Ad still not loaded");
            }
        }

        #endregion

        #region LISTENERS

        private class RewardAdListener : IRewardAdListener
        {
            private readonly HuaweiModule m_HuaweiModule;

            public RewardAdListener(HuaweiModule huaweiModule)
            {
                m_HuaweiModule = huaweiModule;
            }
            public void OnRewardAdClosed()
            {
                Debug.Log("[HMS] HMSAdsKitManager OnRewardAdClosed");
                m_HuaweiModule.OnRewardAdClosed?.Invoke();
                m_HuaweiModule.LoadRewardedAd();
            }

            public void OnRewardAdCompleted()
            {
                Debug.Log("[HMS] HMSAdsKitManager OnRewardAdCompleted!");
                m_HuaweiModule.OnRewardAdCompleted?.Invoke();
            }

            public void OnRewardAdFailedToLoad(int errorCode)
            {
                Debug.Log($"[HMS] HMSAdsKitManager Rewarded ad loading failed with error ${errorCode}");
                m_HuaweiModule.OnRewardedAdFailedToLoad?.Invoke(errorCode);
            }

            public void OnRewardAdLeftApp()
            {
                Debug.Log("[HMS] HMSAdsKitManager OnRewardAdLeftApp!");
                m_HuaweiModule.OnRewardAdLeftApp?.Invoke();
            }

            public void OnRewardAdLoaded()
            {
                Debug.Log("[HMS] HMSAdsKitManager Rewarded ad loaded!");
                m_HuaweiModule.OnRewardedAdLoaded?.Invoke();
            }

            public void OnRewardAdOpened()
            {
                Debug.Log("[HMS] HMSAdsKitManager OnRewardAdOpened");
                m_HuaweiModule.OnRewardAdOpened?.Invoke();
            }

            public void OnRewardAdStarted()
            {
                Debug.Log("[HMS] HMSAdsKitManager OnRewardAdStarted!");
                m_HuaweiModule.OnRewardAdStarted?.Invoke();
            }

            public void OnRewarded(Reward reward)
            {
                Debug.Log("[HMS] HMSAdsKitManager OnRewarded " + reward);
                m_HuaweiModule.OnRewarded?.Invoke(reward);
            }
        }

        public Action OnRewardAdClosed { get; set; }
        public Action OnRewardAdLeftApp { get; set; }
        public Action OnRewardAdStarted { get; set; }
        public Action OnRewardAdOpened { get; set; }
        public Action OnRewardAdCompleted { get; set; }
        public Action<Reward> OnRewarded { get; set; }
        public Action OnRewardedAdLoaded { get; set; }
        public Action<int> OnRewardedAdFailedToLoad { get; set; }

        #endregion

        #endregion
    }
}
