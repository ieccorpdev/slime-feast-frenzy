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

        public bool isNoAds;

        Dictionary<string, OguryThumbnailAd> m_DictThumbnailAdById;
        private Dictionary<OguryThumbnailAd, string> m_DictIdByThumbnailAd;
        Dictionary<string, bool> m_DictThumbnailLoaded;
        public Action onShowThumbnail;
        public Action onCloseThumbnail;

        private List<string> m_ListThumbnailID = new List<string>()
        {
            "b63dcc40-9e8c-0139-4726-0242ac120004",
            "bce6f180-9e8c-0139-f23a-0242ac120004"
        };

        AdsStateData m_ThumbnailState = new AdsStateData();

        public void Init(OguryAdsConfig adsConfig, bool isAcceptConsent, bool isAutoReload,
            Action<bool> afterInitCallback = null)
        {
            m_PlatformConfig = (Application.platform == RuntimePlatform.Android) ? adsConfig.android : adsConfig.ios;

            Debug.LogWarning("Ogury App Id:" + m_PlatformConfig.appID.Trim());
            Ogury.Start(adsConfig.android.appID.Trim(), adsConfig.ios.appID.Trim());
            m_DictThumbnailAdById = new Dictionary<string, OguryThumbnailAd>();
            m_DictIdByThumbnailAd = new Dictionary<OguryThumbnailAd, string>();
            m_DictThumbnailLoaded = new Dictionary<string, bool>();
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
            for (int i = 0; i < m_ListThumbnailID.Count; i++)
            {
                var id = m_ListThumbnailID[i];
                var thumbnailAd = new OguryThumbnailAd(id, id);
                thumbnailAd.OnAdLoaded += Thumbnail_OnAdLoaded;
                thumbnailAd.OnAdClosed += Thumbnail_OnAdClosed;
                thumbnailAd.OnAdDisplayed += Thumbnail_OnAdDisplayed;
                thumbnailAd.OnAdError += Thumbnail_OnAdError;
                thumbnailAd.OnAdNotAvailable += Thumbnail_OnAdNotAvailable;
                thumbnailAd.OnAdNotLoaded += Thumbnail_OnAdNotLoaded;
                thumbnailAd.SetBlacklistActivities(
                    "com.unity3d.services.ads.adunit.AdUnitActivity",
                    "com.unity3d.services.ads.adunit.AdUnitTransparentActivity",
                    "com.unity3d.services.ads.adunit.AdUnitTransparentSoftwareActivity",
                    "com.unity3d.services.ads.adunit.AdUnitSoftwareActivity");
                m_DictThumbnailAdById[id] = thumbnailAd;
                m_DictIdByThumbnailAd[thumbnailAd] = id;
                m_DictThumbnailLoaded[id] = false;
                Debug.LogWarning("Init thumbnail:" + id);
            }
        }

        void Thumbnail_OnAdLoaded(OguryThumbnailAd ad)
        {
            var id = m_DictIdByThumbnailAd[ad];
            Debug.LogWarning("Thumbnail OnAdLoaded:" + id);
            m_DictThumbnailLoaded[id] = true;
            ShowThumbnail();
        }

        void Thumbnail_OnAdClosed(OguryThumbnailAd ad)
        {
            var id = m_DictIdByThumbnailAd[ad];
            Debug.LogWarning("Thumbnail OnAdClosed:" + id);
            m_IsThumbnailShowing = false;
            m_DictThumbnailLoaded[id] = false;
            onCloseThumbnail?.Invoke();
            ShowThumbnail();
        }

        void Thumbnail_OnAdDisplayed(OguryThumbnailAd ad)
        {
            Debug.LogWarning("Thumbnail OnAdDisplayed");
            onShowThumbnail?.Invoke();
        }

        void Thumbnail_OnAdError(OguryThumbnailAd ad, OguryError error)
        {
            Debug.Log("Thumbnail OnAdError:" + error.ErrorCode + "-" + error.Description);
            // LoadThumbnail(ad);
        }

        void Thumbnail_OnAdNotAvailable(OguryThumbnailAd ad)
        {
            Debug.Log("Thumbnail OnAdNotAvailable:" + m_DictIdByThumbnailAd[ad]);
        }

        void Thumbnail_OnAdNotLoaded(OguryThumbnailAd ad)
        {
            Debug.Log("Thumbnail OnAdNotLoaded:" + m_DictIdByThumbnailAd[ad]);
        }

        void LoadThumbnail(OguryThumbnailAd ad)
        {
            Debug.LogWarning("Load thumbnail:" + m_DictIdByThumbnailAd[ad]);
            ad.Load(180, 180);
        }

        public void LoadThumbnail()
        {
            for (int i = 0; i < m_ListThumbnailID.Count; i++)
            {
                var id = m_ListThumbnailID[i];
                if (!IsThumbnailLoaded(id)) LoadThumbnail(m_DictThumbnailAdById[id]);
                else Debug.LogWarning("thumbnail is loaded:" + id);
            }
        }

        public void ShowThumbnail()
        {
            Debug.LogWarning("Call show thumbnail");
            Debug.LogWarning("isThumbnailShow:" + m_IsThumbnailShowing);
            Debug.LogWarning("isNoAds:" + isNoAds);
            if (m_IsThumbnailShowing || isNoAds) return;
            var id = string.Empty;
            for (int i = 0; i < m_ListThumbnailID.Count; i++)
            {
                id = m_ListThumbnailID[i];
                if (IsThumbnailLoaded(id)) break;
            }

            Debug.LogWarning("id:" + id);
            if (string.IsNullOrEmpty(id)) return;
            m_IsThumbnailShowing = true;
            m_DictThumbnailAdById[id].Show(OguryThumbnailAdGravity.BottomLeft, 0, 0);
        }

        public bool IsThumbnailLoaded(string id)
        {
            return m_DictThumbnailLoaded[id] &&
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