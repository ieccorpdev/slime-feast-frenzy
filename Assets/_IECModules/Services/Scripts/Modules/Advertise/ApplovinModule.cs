#if (PluginApplovin)
#if (PluginMaxAmazonNetwork)
using AmazonAds;
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if (PluginAdjust)
using com.adjust.sdk;
#endif

namespace IEC
{
    public class ApplovinModule : IAdsService
    {
        public const string APPLOVIN_VERBOSE_LOGGING = "APPLOVIN_VERBOSE_LOGGING";

        AdsPlatformConfig m_Config;
        AdsPlatformConfig m_amazonConfig;
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

        Action<string, float, Dictionary<string, object>> pushEventAction;
        public Action<AdsEnum, float> onAdRevenueAction;
#if (PluginMaxAmazonNetwork)
        private APSBannerAdRequest bannerAdRequest;
        private APSVideoAdRequest interstitialVideoAdRequest;
        private APSVideoAdRequest rewardedVideoAdRequest;
        private bool isFirstLoadInter = true, isFirstLoadReward = true;
#endif

        public void Init(AdsConfig adsConfig, AmazonConfig amazonConfig, bool isAcceptConsent, bool isAutoReload, Action<bool> afterInitCallback, Action<string, float, Dictionary<string, object>> pushEventAction)
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

#if (PluginMaxAmazonNetwork)
            var amzConfig = (Application.platform == RuntimePlatform.Android) ? amazonConfig.android : amazonConfig.ios;
            amzConfig.appID = amzConfig.appID.Trim();
            amzConfig.bannerID = amzConfig.bannerID.Trim();
            amzConfig.interstitialID = amzConfig.interstitialID.Trim();
            amzConfig.rewardID = amzConfig.rewardID.Trim();

            Debug.LogWarning("Amazon AppID:" + amzConfig.appID);
            Debug.LogWarning("Amazon Banner ID:" + amzConfig.bannerID);
            Debug.LogWarning("Amazon Inter ID:" + amzConfig.interstitialID);
            Debug.LogWarning("Amazon Reward ID:" + amzConfig.rewardID);

            if (string.IsNullOrEmpty(amzConfig.appID.Trim()) == false)
            {
                Amazon.Initialize(amzConfig.appID);
                Amazon.EnableTesting(false);
                Amazon.EnableLogging(false);
                Amazon.UseGeoLocation(false);
                Amazon.SetMRAIDPolicy(Amazon.MRAIDPolicy.CUSTOM);
                Amazon.SetAdNetworkInfo(new AdNetworkInfo(DTBAdNetwork.MAX));
                Amazon.SetMRAIDSupportedVersions(new string[] { "1.0", "2.0", "3.0" });
            }
            m_amazonConfig = amzConfig;
#endif
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
#if (PluginMaxAmazonNetwork)
            switch (adsType)
            {
                case AdsEnum.BANNER_BOTTOM:
                case AdsEnum.BANNER_TOP:
                    RequestBannerAmazon(loadCallback, adsType);
                    break;
                case AdsEnum.INTERSTITIAL:
                    RequestInterstitialAmazon(loadCallback);
                    break;
                case AdsEnum.REWARD:
                    RequestRewardedVideoAmazon(loadCallback);
                    break;
            }
#else
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
#endif
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
            var adaptiveHeight = MaxSdkUtils.GetAdaptiveBannerHeight();
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

            MaxSdk.CreateBanner(m_Config.bannerID, ConvertBannerPos(bannerPos));
            MaxSdk.SetBannerBackgroundColor(m_Config.bannerID, Color.white);
            AdsStateData.SafeInvoke(loadCallback, AdsEventArgs.DONE);
        }

        void ShowBanner(Action<AdsEventArgs> showCallback, AdsEnum bannerPos)
        {
            MaxSdk.ShowBanner(m_Config.bannerID);
            MaxSdk.SetBannerBackgroundColor(m_Config.bannerID, Color.white);
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
            MaxSdkCallbacks.Banner.OnAdClickedEvent += BannerAd_OnAdClicked;
            MaxSdkCallbacks.Banner.OnAdLoadedEvent += BannerAd_OnAdLoaded;
            MaxSdkCallbacks.Banner.OnAdLoadFailedEvent += BannerAd_OnAdFailedToLoad;
            MaxSdkCallbacks.Banner.OnAdCollapsedEvent += BannerAd_OnAdCollapsed;
            MaxSdkCallbacks.Banner.OnAdExpandedEvent += BannerAd_OnAdExpanded;
            MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;
        }

#if(PluginMaxAmazonNetwork)
        public void RequestBannerAmazon(Action<AdsEventArgs> loadCallback, AdsEnum adsType)
        {
            var maxBannerAdId = m_Config.bannerID;
            if (string.IsNullOrEmpty(m_Config.bannerID))
            {
                AdsStateData.SafeInvoke(loadCallback, AdsEventArgs.FAIL_NO_ID);
                return;
            }

            if(string.IsNullOrEmpty(m_amazonConfig.bannerID))
            {
                LoadBannerAds(loadCallback, adsType);
                return;
            }
            if (bannerAdRequest != null) bannerAdRequest.DestroyFetchManager();
            var width = 320;
            var height = 50;
            bannerAdRequest = new APSBannerAdRequest(width, height, m_amazonConfig.bannerID);
            bannerAdRequest.onFailedWithError += (adError) =>
            {
                MaxSdk.SetBannerLocalExtraParameter(maxBannerAdId, "amazon_ad_error", adError.GetAdError());
                LoadBannerAds(loadCallback, adsType);
            };
            bannerAdRequest.onSuccess += (adResponse) =>
            {
                MaxSdk.SetBannerLocalExtraParameter(maxBannerAdId, "amazon_ad_response", adResponse.GetResponse());
                LoadBannerAds(loadCallback, adsType);
            };

            bannerAdRequest.LoadAd();
        }
#endif
        private void BannerAd_OnAdExpanded(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log("BannerAd_OnAdExpanded");
        }

        private void BannerAd_OnAdCollapsed(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log("BannerAd_OnAdCollapsed");
        }

        private void BannerAd_OnAdFailedToLoad(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            Debug.Log("BannerAd_OnAdFailedToLoad:" + errorInfo.Code);
        }

        private void BannerAd_OnAdLoaded(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log("BannerAd_OnAdLoaded");
        }

        void BannerAd_OnAdClicked(string adUnitId, MaxSdkBase.AdInfo adInfo)
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
#if(PluginMaxAmazonNetwork)

        public void RequestInterstitialAmazon(Action<AdsEventArgs> loadCallback)
        {
            var maxInterstitialAdId = m_Config.interstitialID;
            if (string.IsNullOrEmpty(m_Config.interstitialID))
            {
                AdsStateData.SafeInvoke(loadCallback, AdsEventArgs.FAIL_NO_ID);
                return;
            }
            if(string.IsNullOrEmpty(m_amazonConfig.interstitialID))
            {
                LoadFullAds(loadCallback);
                return;
            }
            if (isFirstLoadInter)
            {
                // APS LoadAd only needs to be called once.
                isFirstLoadInter = false;
                interstitialVideoAdRequest = new APSVideoAdRequest(320, 480, m_amazonConfig.interstitialID);
                interstitialVideoAdRequest.onSuccess += (adResponse) =>
                {
                    MaxSdk.SetInterstitialLocalExtraParameter(maxInterstitialAdId, "amazon_ad_response", adResponse.GetResponse());
                    LoadFullAds(loadCallback);
                };
                interstitialVideoAdRequest.onFailedWithError += (adError) =>
                {
                    MaxSdk.SetInterstitialLocalExtraParameter(maxInterstitialAdId, "amazon_ad_error", adError.GetAdError());
                    LoadFullAds(loadCallback);
                };
                interstitialVideoAdRequest.LoadAd();
            }
            else
            {
                LoadFullAds(loadCallback);
            }
        }
#endif
        private void InterstitialAd_OnAdLoaded(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            AdsStateData.DebugLog("InterstitialAd_OnAdLoaded");
            var adsState = m_FullAdsState;
            adsState.isLoaded = true;
            adsState.isLoading = false;
            adsState.onLoad = AdsStateData.SafeInvoke(adsState.onLoad, AdsEventArgs.DONE);
        }

        private void InterstitialAd_OnAdFailedToLoad(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            var adsState = m_FullAdsState;
            AdsStateData.DebugLog("InterstitialAd_OnAdFailedToLoad : " + errorInfo.Code);
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

        private void InterstitialAd_OnAdClosed(string adUnitId, MaxSdkBase.AdInfo adInfo)
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

        private void InterstitialAd_OnAdFailedToDisplay(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
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
            MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += InterstitialAd_OnAdLoaded;
            MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += InterstitialAd_OnAdFailedToLoad;
            MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += InterstitialAd_OnAdFailedToDisplay;
            MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += InterstitialAd_OnAdClosed;
            MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;
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
#if(PluginMaxAmazonNetwork)

        public void RequestRewardedVideoAmazon(Action<AdsEventArgs> loadCallback)
        {
            var maxRewardedAdId = m_Config.rewardID;
            if (string.IsNullOrEmpty(m_Config.rewardID))
            {
                AdsStateData.SafeInvoke(loadCallback, AdsEventArgs.FAIL_NO_ID);
                return;
            }
            
            if(string.IsNullOrEmpty(m_amazonConfig.rewardID))
            {
                LoadRewardAds(loadCallback);
                return;
            }

            if (isFirstLoadReward)
            {
                // APS LoadAd only needs to be called once.
                isFirstLoadReward = false;
                rewardedVideoAdRequest = new APSVideoAdRequest(320, 480, m_amazonConfig.rewardID);
                rewardedVideoAdRequest.onSuccess += (adResponse) =>
                {
                    MaxSdk.SetRewardedAdLocalExtraParameter(maxRewardedAdId, "amazon_ad_response", adResponse.GetResponse());
                    LoadRewardAds(loadCallback);
                };
                rewardedVideoAdRequest.onFailedWithError += (adError) =>
                {
                    MaxSdk.SetRewardedAdLocalExtraParameter(maxRewardedAdId, "amazon_ad_error", adError.GetAdError());
                    LoadRewardAds(loadCallback);
                };
                rewardedVideoAdRequest.LoadAd();
            }
            else
            {
                LoadRewardAds(loadCallback);
            }
        }
#endif
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

        private void RewardedAdsView_OnAdFailedToShow(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
        {
            m_RewardAdsState.isLoading = false;
            m_RewardAdsState.isLoaded = false;
            AdsStateData.DebugLog("RewardedAdsView_OnAdFailedToShow : " + errorInfo.Code);
            m_RewardAdsState.onShow = AdsStateData.SafeInvoke(m_RewardAdsState.onShow, AdsEventArgs.FAIL);
        }

        private void RewardedAdsView_OnUserEarnedReward(string adUnitId, MaxSdkBase.Reward reward, MaxSdkBase.AdInfo adInfo)
        {
            m_RewardAdsState.onShow = AdsStateData.SafeInvoke(m_RewardAdsState.onShow, AdsEventArgs.DONE_NO_SKIP);
        }

        private void RewardedAdsView_OnAdFailedToLoad(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            AdsStateData.DebugLog("RewardedAdsView_OnAdFailedToLoad : " + errorInfo.Code);
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

        private void RewardedAdsView_OnAdLoaded(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            AdsStateData.DebugLog("RewardedAdsView_OnAdLoaded");
            m_RewardAdsState.isLoaded = true;
            m_RewardAdsState.isLoading = false;
            m_RewardAdsState.onLoad = AdsStateData.SafeInvoke(m_RewardAdsState.onLoad, AdsEventArgs.DONE);
        }

        private void RewardedAdsView_OnAdClosed(string adUnitId, MaxSdkBase.AdInfo adInfo)
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
            MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += RewardedAdsView_OnAdLoaded;
            MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += RewardedAdsView_OnAdFailedToLoad;
            MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += RewardedAdsView_OnAdFailedToShow;
            MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += RewardedAdsView_OnAdClosed;
            MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += RewardedAdsView_OnUserEarnedReward;
            MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;
        }

        #endregion
        private void OnAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            double revenue = adInfo.Revenue;

            // Miscellaneous data
            string countryCode = MaxSdk.GetSdkConfiguration().CountryCode; // "US" for the United States, etc - Note: Do not confuse this with currency code which is "USD" in most cases!
            string networkName = adInfo.NetworkName; // Display name of the network that showed the ad (e.g. "AdColony")
            string adUnitIdentifier = adInfo.AdUnitIdentifier; // The MAX Ad Unit ID
            string placement = adInfo.Placement; // The placement this ad's postbacks are tied to

            Debug.LogWarning("Ads Revenue Event:" + adUnitId);
            Debug.LogWarning("rev:" + revenue);
            Debug.LogWarning("country code:" + countryCode);
            Debug.LogWarning("network name:" + networkName);
            Debug.LogWarning("ad unit identifier:" + adUnitIdentifier);
            Debug.LogWarning("placement:" + placement);

            var dict = new Dictionary<string, object>
            {
                ["revenue"] = revenue,
                ["ad_unit_id"] = adUnitId,
                ["network_name"] = networkName
            };
            var eventName = EventName(adUnitId);
            var dictParamAll = new Dictionary<string, object>
            {
                ["ad_type"] = eventName,
                ["network_name"] = networkName
            };

            var adType = AdsEnum.BANNER_BOTTOM;
            if (adUnitId.Equals(m_Config.interstitialID))
            {
                adType = AdsEnum.INTERSTITIAL;
            }
            else if (adUnitId.Equals(m_Config.rewardID))
            {
                adType = AdsEnum.REWARD;
            }

            onAdRevenueAction?.Invoke(adType, (float)revenue);
            pushEventAction?.Invoke(eventName, (float) revenue, dict);
            pushEventAction?.Invoke("rev_all_ad",(float)revenue,dictParamAll);
            AdjustTrackAdRevenue(adUnitId, adInfo);
        }

        void AdjustTrackAdRevenue(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
#if (PluginAdjust)
            var adRevenue = new AdjustAdRevenue(AdjustConfig.AdjustAdRevenueSourceAppLovinMAX);
            adRevenue.setRevenue(adInfo.Revenue, "USD");
            adRevenue.setAdRevenueNetwork(adInfo.NetworkName);
            adRevenue.setAdRevenueUnit(adInfo.AdUnitIdentifier);
            adRevenue.setAdRevenuePlacement(adInfo.Placement);
            Debug.LogWarning($"[Adjust ad track] revenue: {adRevenue.revenue}-{adRevenue.currency} | network: {adRevenue.adRevenueNetwork} | adUnit: {adRevenue.adRevenueUnit}");
            Adjust.trackAdRevenue(adRevenue);
#endif
        }

        private string EventName(string adUnitId)
        {
            if (adUnitId.Equals(m_Config.bannerID))
            {
                return "rev_banner_ad";
            }
            else if (adUnitId.Equals(m_Config.interstitialID))
            {
                return "rev_full_ad";
            }
            else if (adUnitId.Equals(m_Config.rewardID))
            {
                return "rev_reward_ad";
            }
            else
            {
                return "rev";
            }
        }

        public void ResetLoadCount()
        {
            m_LoadFullAdCount = 0;
            m_LoadRewardCount = 0;
        }
    }
}
#endif
