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

        OguryThumbnailAd m_ThumbnailAd;

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
            afterInitCallback?.Invoke(true);
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
            m_ThumbnailAd.OnAdLoaded += ad =>
            {
                Debug.Log("Thumbnail OnAdLoaded");
            };
            m_ThumbnailAd.OnAdClosed += ad =>
            {
                Debug.Log("Thumbnail OnAdClosed");
                m_IsThumbnailShowing = false;
            };
            m_ThumbnailAd.OnAdDisplayed += ad =>
            {
                Debug.Log("Thumbnail OnAdDisplayed");
                m_IsThumbnailShowing = true;
            };
            m_ThumbnailAd.OnAdError += (ad, error) =>
            {
                Debug.Log("Thumbnail OnAdError:" + error.ErrorCode + "-" + error.Description);
            };
            m_ThumbnailAd.OnAdNotAvailable += ad => { Debug.LogWarning("Thumbnail OnAdNotAvailable"); };
            m_ThumbnailAd.OnAdNotLoaded += ad => { Debug.LogWarning("Thumbnail OnAdNotLoaded"); };
            m_ThumbnailAd.SetBlacklistActivities(
                "com.unity3d.services.ads.adunit.AdUnitActivity",
                "com.unity3d.services.ads.adunit.AdUnitTransparentActivity",
                "com.unity3d.services.ads.adunit.AdUnitTransparentSoftwareActivity",
                "com.unity3d.services.ads.adunit.AdUnitSoftwareActivity");

        }

        void Thumbnail_OnAdLoaded(OguryThumbnailAd ad)
        {

        }
        void Thumbnail_OnAdClosed(OguryThumbnailAd ad)
        {

        }
        void Thumbnail_OnAdDisplayed(OguryThumbnailAd ad)
        {

        }
        void Thumbnail_OnAdError(OguryThumbnailAd ad, OguryError error)
        {

        }
        void Thumbnail_OnAdError(OguryThumbnailAd ad)
        {

        }
        #endregion
    }
}
#endif
