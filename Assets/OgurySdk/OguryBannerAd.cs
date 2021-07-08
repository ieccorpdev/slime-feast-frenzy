using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using OgurySdk.Internal;
using UnityEngine;

// ReSharper disable UseNullPropagation
// ReSharper disable InlineOutVariableDeclaration
// ReSharper disable MergeSequentialChecks
// ReSharper disable UsePatternMatching

namespace OgurySdk
{
    public class OguryBannerAd
    {
#pragma warning disable 649
        private readonly int _instanceId;
#pragma warning restore 649

#if UNITY_ANDROID
        private AndroidJavaObject bannerAd;
#endif

#if UNITY_IOS
        [DllImport("__Internal", CharSet = CharSet.Ansi)]
        private static extern int ogury_bannerAd_create(string adUnitId, string size);

        [DllImport("__Internal")]
        private static extern void ogury_bannerAd_load(int instanceId);

        [DllImport("__Internal")]
        private static extern bool ogury_bannerAd_show(int instanceId, string gravity, int xOffset, int yOffset);

        [DllImport("__Internal")]
        private static extern bool ogury_bannerAd_hide(int instanceId);

        [DllImport("__Internal")]
        private static extern int ogury_bannerAd_destroy(int instanceId);
#endif

        public event BannerAdEventHandler OnAdNotAvailable;
        public event BannerAdEventHandler OnAdLoaded;
        public event BannerAdEventHandler OnAdNotLoaded;
        public event BannerAdEventHandler OnAdDisplayed;
        public event BannerAdEventHandler OnAdClicked;
        public event BannerAdEventHandler OnAdClosed;
        public event BannerAdErrorEventHandler OnAdError;

        public delegate void BannerAdErrorEventHandler(OguryBannerAd bannerAd, OguryError error);

        public delegate void BannerAdEventHandler(OguryBannerAd bannerAd);

        public OguryBannerAd(string androidAdUnitId, string iOsAdUnitId, OguryBannerAdSize bannerAdSize)
        {
#if UNITY_EDITOR
            // Noop
#elif UNITY_ANDROID
            bannerAd =
 new AndroidJavaObject("com.ogury.unity.banner.OguryBannerAd", androidAdUnitId, bannerAdSize.ToString());
            _instanceId = bannerAd.Call<int>("getInstanceId");
#elif UNITY_IOS
            _instanceId = ogury_bannerAd_create(iOsAdUnitId, bannerAdSize.ToString());
#endif

            if (OguryCallbacks.CheckIfPresent())
            {
                // The captures references and the lambda construct for every callback is mandatory.
                // This is made in order that the OguryCallbacks singleton does not maintain a reference to this instance.
                // This will allow the GC to clean this instance when necessary and allow us to clean our native instance also.
                int capturedInstanceId = _instanceId;
                WeakReference capturedWeakBannerAd = new WeakReference(this);

                OguryCallbacks.Instance.OnAdNotAvailable += instanceId =>
                {
                    OguryBannerAd strong = capturedWeakBannerAd.Target as OguryBannerAd;
                    if (instanceId == capturedInstanceId
                        && strong != null
                        && strong.OnAdNotAvailable != null)
                    {
                        strong.OnAdNotAvailable.Invoke(strong);
                    }
                };
                OguryCallbacks.Instance.OnAdLoaded += instanceId =>
                {
                    OguryBannerAd strong = capturedWeakBannerAd.Target as OguryBannerAd;
                    if (instanceId == capturedInstanceId
                        && strong != null
                        && strong.OnAdLoaded != null)
                    {
                        strong.OnAdLoaded.Invoke(strong);
                    }
                };
                OguryCallbacks.Instance.OnAdNotLoaded += instanceId =>
                {
                    OguryBannerAd strong = capturedWeakBannerAd.Target as OguryBannerAd;
                    if (instanceId == capturedInstanceId
                        && strong != null
                        && strong.OnAdNotLoaded != null)
                    {
                        strong.OnAdNotLoaded.Invoke(strong);
                    }
                };
                OguryCallbacks.Instance.OnAdDisplayed += instanceId =>
                {
                    OguryBannerAd strong = capturedWeakBannerAd.Target as OguryBannerAd;
                    if (instanceId == capturedInstanceId
                        && strong != null
                        && strong.OnAdDisplayed != null)
                    {
                        strong.OnAdDisplayed.Invoke(strong);
                    }
                };
                OguryCallbacks.Instance.OnAdClicked += instanceId =>
                {
                    OguryBannerAd strong = capturedWeakBannerAd.Target as OguryBannerAd;
                    if (instanceId == capturedInstanceId
                        && strong != null
                        && strong.OnAdClicked != null)
                    {
                        strong.OnAdClicked.Invoke(strong);
                    }
                };
                OguryCallbacks.Instance.OnAdClosed += instanceId =>
                {
                    OguryBannerAd strong = capturedWeakBannerAd.Target as OguryBannerAd;
                    if (instanceId == capturedInstanceId
                        && strong != null
                        && strong.OnAdClosed != null)
                    {
                        strong.OnAdClosed.Invoke(strong);
                    }
                };
                OguryCallbacks.Instance.OnAdError += (instanceId, error) =>
                {
                    OguryBannerAd strong = capturedWeakBannerAd.Target as OguryBannerAd;
                    if (instanceId == capturedInstanceId
                        && strong != null
                        && strong.OnAdError != null)
                    {
                        strong.OnAdError.Invoke(strong, error);
                    }
                };
            }
        }

        public void Load()
        {
#if UNITY_EDITOR
            if (OnAdNotAvailable != null)
            {
                OnAdNotAvailable(this);
            }
#elif UNITY_ANDROID
            bannerAd.Call("load");
#elif UNITY_IOS
            ogury_bannerAd_load(_instanceId);
#endif
        }

        public void Show(OguryAdGravity gravity, int xOffsetInPixels = 0, int yOffsetInPixels = 0)
        {
#if UNITY_EDITOR
            if (OnAdNotLoaded != null)
            {
                OnAdNotLoaded(this);
            }
#elif UNITY_ANDROID
            bannerAd.Call("show", gravity.ToString(), xOffsetInPixels, yOffsetInPixels);
#elif UNITY_IOS
            ogury_bannerAd_show(_instanceId, gravity.ToString(), xOffsetInPixels, yOffsetInPixels);
#endif
        }

        public void Hide()
        {
#if UNITY_EDITOR
            // Noop
#elif UNITY_ANDROID
            bannerAd.Call("hide");
#elif UNITY_IOS
            ogury_bannerAd_hide(_instanceId);
#endif
        }

        ~OguryBannerAd()
        {
#if UNITY_EDITOR
            // Noop
#elif UNITY_IOS
            ogury_bannerAd_destroy(_instanceId);
#endif
        }
    }
}