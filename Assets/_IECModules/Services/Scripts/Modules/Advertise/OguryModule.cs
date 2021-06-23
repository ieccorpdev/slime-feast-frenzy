#if (PluginOgury)
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OgurySdk;

namespace IEC
{
    public class OguryModule : IAdsService
    {
        OguryAdsConfig m_Config;
        OguryAdsPlatformConfig m_PlatformConfig;

        bool m_IsAutoReload = false;
        bool m_IsAcceptConsent = false;
        bool m_IsThumbnailShowing = false;
        bool m_IsInit = false;

        OguryThumbnailAd m_ThumbnailAd;
        AdsStateData m_ThumbnailState = new AdsStateData();
        public void Init(OguryAdsConfig adsConfig, bool isAcceptConsent, bool isAutoReload, Action<bool> afterInitCallback = null)
        {
            var oguryCallbackObj = new GameObject("OguryCallbacks");
            oguryCallbackObj.AddComponent<OguryCallbacks>();

            m_PlatformConfig = (Application.platform == RuntimePlatform.Android) ? adsConfig.android : adsConfig.ios;

            Debug.LogWarning("Ogury App Id:" + m_PlatformConfig.appID.Trim());
            Ogury.Start(adsConfig.android.appID.Trim(), adsConfig.ios.appID.Trim());
            Ogury.StartAds();

            m_Config = adsConfig;
            m_IsAutoReload = isAutoReload;
            m_IsAcceptConsent = isAcceptConsent;
            InitThumbnail();
            afterInitCallback?.Invoke(true);

            m_IsInit = true;
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
            throw new NotImplementedException();
        }

        public void Hide(AdsEnum adsType)
        {

        }

        public bool IsAcceptGDPR()
        {
            return true;
        }

        public bool IsLoaded(AdsEnum adsType)
        {
            return false;
        }

        public void Load(AdsEnum adsType, Action<AdsEventArgs> loadCallback)
        {

        }

        public void SetOnClick(AdsEnum adsType, Action clickCallback)
        {

        }

        public void Show(AdsEnum adsType, Action<AdsEventArgs> showCallback)
        {

        }

        #region Thumbnail
        void InitThumbnail()
        {
            m_ThumbnailAd = new OguryThumbnailAd(m_Config.android.thumbnailID.Trim(), m_Config.ios.thumbnailID.Trim());
            m_ThumbnailAd.OnAdLoaded += Thumbnail_OnAdLoaded;
            m_ThumbnailAd.OnAdClosed += Thumbnail_OnAdClosed;
            m_ThumbnailAd.OnAdDisplayed += Thumbnail_OnAdDisplayed;
            m_ThumbnailAd.OnAdError += Thumbnail_OnAdError;
            m_ThumbnailAd.OnAdNotAvailable += Thumbnail_OnAdNotAvailable;
            m_ThumbnailAd.OnAdNotLoaded += Thumbnail_OnAdNotLoaded;
            m_ThumbnailAd.SetBlacklistActivities(
                "com.unity3d.services.ads.adunit.AdUnitActivity",
                "com.unity3d.services.ads.adunit.AdUnitTransparentActivity",
                "com.unity3d.services.ads.adunit.AdUnitTransparentSoftwareActivity",
                "com.unity3d.services.ads.adunit.AdUnitSoftwareActivity");

        }

        void Thumbnail_OnAdLoaded(OguryThumbnailAd ad)
        {
            Debug.Log("Thumbnail OnAdLoaded");
            var adsState = m_ThumbnailState;
            adsState.onLoad = AdsStateData.SafeInvoke(adsState.onLoad, AdsEventArgs.DONE);
            adsState.isLoaded = true;
            adsState.isLoading = false;
        }
        void Thumbnail_OnAdClosed(OguryThumbnailAd ad)
        {
            Debug.Log("Thumbnail OnAdClosed");
            m_IsThumbnailShowing = false;
            var adsState = m_ThumbnailState;
            adsState.onShow = AdsStateData.SafeInvoke(adsState.onShow, AdsEventArgs.DONE);
            adsState.isLoaded = false;
            adsState.isLoading = false;
            if(m_IsAutoReload)
            {
                LoadThumbnail(null);
            }
        }
        void Thumbnail_OnAdDisplayed(OguryThumbnailAd ad)
        {
            Debug.Log("Thumbnail OnAdDisplayed");
            m_IsThumbnailShowing = true;
        }
        void Thumbnail_OnAdError(OguryThumbnailAd ad, OguryError error)
        {
            Debug.Log("Thumbnail OnAdError:" + error.ErrorCode + "-" + error.Description);
            var adsState = m_ThumbnailState;
            adsState.isLoaded = false;
            adsState.isLoading = false;
            adsState.onLoad = AdsStateData.SafeInvoke(adsState.onLoad, AdsEventArgs.FAIL);

            if (m_IsAutoReload)
            {
                LoadThumbnail(null);
            }
        }
        void Thumbnail_OnAdNotAvailable(OguryThumbnailAd ad)
        {
            Debug.Log("Thumbnail OnAdNotAvailable");
        }

        void Thumbnail_OnAdNotLoaded(OguryThumbnailAd ad)
        {
            Debug.Log("Thumbnail OnAdNotLoaded");
        }

        public void LoadThumbnail(Action<AdsEventArgs> loadCallback)
        {
            var adsState = m_ThumbnailState;
            adsState.onLoad = loadCallback;

            if (adsState.isLoading)
            {
                return;
            }

            if (IsThumbnailLoaded())
            {
                AdsStateData.SafeInvoke(loadCallback, AdsEventArgs.DONE);
                return;
            }

            adsState.isLoading = true;
            adsState.isLoaded = false;
            adsState.isWaitToShow = false;
            m_ThumbnailState = adsState;

            m_ThumbnailAd.Load(180, 180);
        }

        public void ShowThumbnail(Action<AdsEventArgs> showCallback)
        {
            var adsState = m_ThumbnailState;
            adsState.isWaitToShow = true;

            if (IsThumbnailLoaded())
            {
                adsState.onShow = showCallback;
                m_ThumbnailAd.Show(OguryThumbnailAdGravity.BottomLeft, 0, 0);
            }
            else
            {
                AdsStateData.SafeInvoke(showCallback, AdsEventArgs.FAIL);
            }
        }

        public bool IsThumbnailLoaded()
        {
            return m_ThumbnailAd.Loaded &&
                Application.internetReachability != NetworkReachability.NotReachable;
        }

        public bool IsThumbnailShowing()
        {
            return m_IsThumbnailShowing;
        }
        #endregion
    }
}
#endif
