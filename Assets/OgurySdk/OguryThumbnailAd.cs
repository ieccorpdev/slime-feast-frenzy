using System;
using System.Runtime.InteropServices;
using UnityEngine;

// ReSharper disable MergeConditionalExpression
// ReSharper disable UseNullPropagation
// ReSharper disable InlineOutVariableDeclaration
// ReSharper disable MergeSequentialChecks
// ReSharper disable UsePatternMatching

namespace OgurySdk
{
    public class OguryThumbnailAd
    {
#pragma warning disable 649
        private readonly int _instanceId;
#pragma warning restore 649

#if UNITY_ANDROID
        private AndroidJavaObject _thumbnailAd;
#endif

#if UNITY_IOS
        [DllImport("__Internal", CharSet = CharSet.Ansi)]
        private static extern int ogury_thumbnailAd_create(string adUnitId);

        [DllImport("__Internal")]
        private static extern void ogury_thumbnailAd_load_default(int instanceId);

        [DllImport("__Internal")]
        private static extern void ogury_thumbnailAd_load(int instanceId, float maxWidthInPoints,
            float maxHeightInPoints);

        [DllImport("__Internal")]
        private static extern bool ogury_thumbnailAd_isLoaded(int instanceId);

        [DllImport("__Internal")]
        private static extern bool ogury_thumbnailAd_show_defaultGravityAndPosition(int instanceId);
        
        [DllImport("__Internal")]
        private static extern bool ogury_thumbnailAd_show_legacy(int instanceId, int xOffsetInPixels,
            int yOffsetInPixels);

        [DllImport("__Internal")]
        private static extern bool ogury_thumbnailAd_show(int instanceId, string gravity, int xOffsetInPixels,
            int yOffsetInPixels);

        [DllImport("__Internal")]
        private static extern bool ogury_thumbnailAd_setWhitelistBundleIdentifiers(int instanceId,
            int whitelistedBundleIdentifiersCount, string[] whitelistedBundleIdentifiers);

        [DllImport("__Internal")]
        private static extern bool ogury_thumbnailAd_setBlacklistViewControllers(int instanceId,
            int blacklistedViewControllersCount, string[] blacklistedViewControllers);

        [DllImport("__Internal")]
        private static extern int ogury_thumbnailAd_destroy(int instanceId);
#endif
        public event ThumbnailAdEventHandler OnAdNotAvailable;
        public event ThumbnailAdEventHandler OnAdLoaded;
        public event ThumbnailAdEventHandler OnAdNotLoaded;
        public event ThumbnailAdEventHandler OnAdDisplayed;
        public event ThumbnailAdEventHandler OnAdClosed;
        public event ThumbnailAdErrorEventHandler OnAdError;

        public delegate void ThumbnailAdErrorEventHandler(OguryThumbnailAd thumbnailAd, OguryError error);

        public delegate void ThumbnailAdEventHandler(OguryThumbnailAd thumbnailAd);

        public OguryThumbnailAd(string androidAdUnitId, string iOsAdUnitId)
        {
#if UNITY_EDITOR
            // Noop
#elif UNITY_ANDROID
            _thumbnailAd = new AndroidJavaObject("com.ogury.unity.thumbnailad.OguryThumbnailAd", androidAdUnitId);
            _instanceId = _thumbnailAd.Call<int>("getInstanceId");
#elif UNITY_IOS
            _instanceId = ogury_thumbnailAd_create(iOsAdUnitId);
#endif

            if (OguryCallbacks.CheckIfPresent())
            {
                // The captures references and the lambda construct for every callback is mandatory.
                // This is made in order that the OguryCallbacks singleton does not maintain a reference to this instance.
                // This will allow the GC to clean this instance when necessary and allow us to clean our native instance also.
                int capturedInstanceId = _instanceId;
                WeakReference capturedWeakThumbnailAd = new WeakReference(this);

                OguryCallbacks.Instance.OnAdNotAvailable += instanceId =>
                {
                    OguryThumbnailAd strong = capturedWeakThumbnailAd.Target as OguryThumbnailAd;
                    if (instanceId == capturedInstanceId
                        && strong != null
                        && strong.OnAdNotAvailable != null)
                    {
                        strong.OnAdNotAvailable.Invoke(strong);
                    }
                };
                OguryCallbacks.Instance.OnAdLoaded += instanceId =>
                {
                    OguryThumbnailAd strong = capturedWeakThumbnailAd.Target as OguryThumbnailAd;
                    if (instanceId == capturedInstanceId
                        && strong != null
                        && strong.OnAdLoaded != null)
                    {
                        strong.OnAdLoaded.Invoke(strong);
                    }
                };
                OguryCallbacks.Instance.OnAdNotLoaded += instanceId =>
                {
                    OguryThumbnailAd strong = capturedWeakThumbnailAd.Target as OguryThumbnailAd;
                    if (instanceId == capturedInstanceId
                        && strong != null
                        && strong.OnAdNotLoaded != null)
                    {
                        strong.OnAdNotLoaded.Invoke(strong);
                    }
                };
                OguryCallbacks.Instance.OnAdDisplayed += instanceId =>
                {
                    OguryThumbnailAd strong = capturedWeakThumbnailAd.Target as OguryThumbnailAd;
                    if (instanceId == capturedInstanceId
                        && strong != null
                        && strong.OnAdDisplayed != null)
                    {
                        strong.OnAdDisplayed.Invoke(strong);
                    }
                };
                OguryCallbacks.Instance.OnAdClosed += instanceId =>
                {
                    OguryThumbnailAd strong = capturedWeakThumbnailAd.Target as OguryThumbnailAd;
                    if (instanceId == capturedInstanceId
                        && strong != null
                        && strong.OnAdClosed != null)
                    {
                        strong.OnAdClosed.Invoke(strong);
                    }
                };
                OguryCallbacks.Instance.OnAdError += (instanceId, error) =>
                {
                    OguryThumbnailAd strong = capturedWeakThumbnailAd.Target as OguryThumbnailAd;
                    if (instanceId == capturedInstanceId
                        && strong != null
                        && strong.OnAdError != null)
                    {
                        strong.OnAdError.Invoke(strong, error);
                    }
                };
            }
        }

        public bool Loaded
        {
            get
            {
#if UNITY_EDITOR
                return false;
#elif UNITY_ANDROID
                return _thumbnailAd.Call<bool>("isLoaded");
#elif UNITY_IOS
                return ogury_thumbnailAd_isLoaded(_instanceId);
#endif
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
            _thumbnailAd.Call("load");
#elif UNITY_IOS
            ogury_thumbnailAd_load_default(_instanceId);
#endif
        }

        public void Load(float maxWidthInPoints, float maxHeightInPoints)
        {
#if UNITY_EDITOR
            if (OnAdNotAvailable != null)
            {
                OnAdNotAvailable(this);
            }
#elif UNITY_ANDROID
            _thumbnailAd.Call("load", maxWidthInPoints, maxHeightInPoints);
#elif UNITY_IOS
            ogury_thumbnailAd_load(_instanceId, maxWidthInPoints, maxHeightInPoints);
#endif
        }

        public void Show()
        {
#if UNITY_EDITOR
            if (OnAdNotLoaded != null)
            {
                OnAdNotLoaded(this);
            }
#elif UNITY_ANDROID
            _thumbnailAd.Call("show");
#elif UNITY_IOS
            ogury_thumbnailAd_show_defaultGravityAndPosition(_instanceId);
#endif
        }

        [ObsoleteAttribute("Deprecated in favor of Show(gravity, xMarginInPoints, yMarginInPoints).", false)]
        public void Show(int xMarginInPoints, int yMarginInPoints)
        {
#if UNITY_EDITOR
            if (OnAdNotLoaded != null)
            {
                OnAdNotLoaded(this);
            }
#elif UNITY_ANDROID
            _thumbnailAd.Call("show", xMarginInPoints, yMarginInPoints);
#elif UNITY_IOS
            ogury_thumbnailAd_show_legacy(_instanceId, xMarginInPoints, yMarginInPoints);
#endif
        }

        public void Show(OguryThumbnailAdGravity gravity, int xMarginInPoints, int yMarginInPoints)
        {
#if UNITY_EDITOR
            if (OnAdNotLoaded != null)
            {
                OnAdNotLoaded(this);
            }
#elif UNITY_ANDROID
            _thumbnailAd.Call("show", gravity.ToString(), xMarginInPoints, yMarginInPoints);
#elif UNITY_IOS
            ogury_thumbnailAd_show(_instanceId, gravity.ToString(), xMarginInPoints, yMarginInPoints);
#endif
        }

        public void SetWhitelistPackages(params string[] whitelistedPackages)
        {
#if UNITY_EDITOR
            // Noop
#elif UNITY_ANDROID
            _thumbnailAd.Call("setWhitelistPackages", (object) whitelistedPackages);
#endif
        }

        public void SetBlacklistActivities(params string[] blacklistedActivities)
        {
#if UNITY_EDITOR
            // Noop
#elif UNITY_ANDROID
            _thumbnailAd.Call("setBlacklistActivities", (object) blacklistedActivities);
#endif
        }

        public void SetWhitelistBundleIdentifiers(params string[] whitelistedBundleIdentifiers)
        {
#if UNITY_EDITOR
            // Noop
#elif UNITY_IOS
            int count = (whitelistedBundleIdentifiers != null) ? whitelistedBundleIdentifiers.Length : 0;
            ogury_thumbnailAd_setWhitelistBundleIdentifiers(_instanceId, count, whitelistedBundleIdentifiers);
#endif
        }

        public void SetBlacklistViewControllers(params string[] blacklistedViewControllers)
        {
#if UNITY_EDITOR
            // Noop
#elif UNITY_IOS
            int count = (blacklistedViewControllers != null) ? blacklistedViewControllers.Length : 0;
            ogury_thumbnailAd_setBlacklistViewControllers(_instanceId, count, blacklistedViewControllers);
#endif
        }

        ~OguryThumbnailAd()
        {
#if UNITY_EDITOR
            // Noop
#elif UNITY_IOS
            ogury_thumbnailAd_destroy(_instanceId);
#endif
        }
    }
}