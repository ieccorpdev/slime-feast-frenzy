using System;
using System.Runtime.InteropServices;
using UnityEngine;

// ReSharper disable UsePatternMatching
// ReSharper disable UseNullPropagation
// ReSharper disable InlineOutVariableDeclaration
// ReSharper disable MergeSequentialChecks

namespace OgurySdk
{
    public class OguryOptinVideoAd
    {
#pragma warning disable 649
        private readonly int _instanceId;
#pragma warning restore 649

#if UNITY_ANDROID
        private AndroidJavaObject _optinVideoAd;
#endif

#if UNITY_IOS
        [DllImport("__Internal", CharSet = CharSet.Ansi)]
        private static extern int ogury_optinVideoAd_create(string adUnitId);

        [DllImport("__Internal")]
        private static extern void ogury_optinVideoAd_load(int instanceId);

        [DllImport("__Internal")]
        private static extern bool ogury_optinVideoAd_isLoaded(int instanceId);

        [DllImport("__Internal")]
        private static extern bool ogury_optinVideoAd_show(int instanceId);

        [DllImport("__Internal")]
        private static extern int ogury_optinVideoAd_destroy(int instanceId);
#endif

        public event OptinVideoAdEventHandler OnAdNotAvailable;
        public event OptinVideoAdEventHandler OnAdLoaded;
        public event OptinVideoAdEventHandler OnAdNotLoaded;
        public event OptinVideoAdEventHandler OnAdDisplayed;
        public event OptinVideoAdRewardedEventHandler OnAdRewarded;
        public event OptinVideoAdEventHandler OnAdClosed;
        public event OptinVideoAdErrorEventHandler OnAdError;

        public delegate void OptinVideoAdErrorEventHandler(OguryOptinVideoAd optinVideoAd, OguryError error);

        public delegate void OptinVideoAdRewardedEventHandler(OguryOptinVideoAd optinVideoAd,
            OguryRewardItem rewardItem);

        public delegate void OptinVideoAdEventHandler(OguryOptinVideoAd optinVideoAd);

        public OguryOptinVideoAd(string androidAdUnitId, string iOsAdUnitId)
        {
#if UNITY_EDITOR
            // Noop
#elif UNITY_ANDROID
            _optinVideoAd = new AndroidJavaObject("com.ogury.unity.optinvideo.OguryOptinVideoAd", androidAdUnitId);
            _instanceId = _optinVideoAd.Call<int>("getInstanceId");
#elif UNITY_IOS
            _instanceId = ogury_optinVideoAd_create(iOsAdUnitId);
#endif

            if (OguryCallbacks.CheckIfPresent())
            {
                // The captures references and the lambda construct for every callback is mandatory.
                // This is made in order that the OguryCallbacks singleton does not maintain a reference to this instance.
                // This will allow the GC to clean this instance when necessary and allow us to clean our native instance also.
                int capturedInstanceId = _instanceId;
                WeakReference capturedWeakOptinVideoAd = new WeakReference(this);
                
                OguryCallbacks.Instance.OnAdNotAvailable += instanceId =>
                {
                    OguryOptinVideoAd strong = capturedWeakOptinVideoAd.Target as OguryOptinVideoAd;
                    if (instanceId == capturedInstanceId
                        && strong != null
                        && strong.OnAdNotAvailable != null)
                    {
                        strong.OnAdNotAvailable.Invoke(strong);
                    }
                };
                OguryCallbacks.Instance.OnAdLoaded += instanceId =>
                {
                    OguryOptinVideoAd strong = capturedWeakOptinVideoAd.Target as OguryOptinVideoAd;
                    if (instanceId == capturedInstanceId
                        && strong != null
                        && strong.OnAdLoaded != null)
                    {
                        strong.OnAdLoaded.Invoke(strong);
                    }
                };
                OguryCallbacks.Instance.OnAdNotLoaded += instanceId =>
                {
                    OguryOptinVideoAd strong = capturedWeakOptinVideoAd.Target as OguryOptinVideoAd;
                    if (instanceId == capturedInstanceId
                        && strong != null
                        && strong.OnAdNotLoaded != null)
                    {
                        strong.OnAdNotLoaded.Invoke(strong);
                    }
                };
                OguryCallbacks.Instance.OnAdDisplayed += instanceId =>
                {
                    OguryOptinVideoAd strong = capturedWeakOptinVideoAd.Target as OguryOptinVideoAd;
                    if (instanceId == capturedInstanceId
                        && strong != null
                        && strong.OnAdDisplayed != null)
                    {
                        strong.OnAdDisplayed.Invoke(strong);
                    }
                };
                OguryCallbacks.Instance.OnAdRewarded += (instanceId, rewardItem) =>
                {
                    OguryOptinVideoAd strong = capturedWeakOptinVideoAd.Target as OguryOptinVideoAd;
                    if (instanceId == capturedInstanceId
                        && strong != null
                        && strong.OnAdRewarded != null)
                    {
                        strong.OnAdRewarded.Invoke(strong, rewardItem);
                    }
                };
                OguryCallbacks.Instance.OnAdClosed += instanceId =>
                {
                    OguryOptinVideoAd strong = capturedWeakOptinVideoAd.Target as OguryOptinVideoAd;
                    if (instanceId == capturedInstanceId
                        && strong != null
                        && strong.OnAdClosed != null)
                    {
                        strong.OnAdClosed.Invoke(strong);
                    }
                };
                OguryCallbacks.Instance.OnAdError += (instanceId, error) =>
                {
                    OguryOptinVideoAd strong = capturedWeakOptinVideoAd.Target as OguryOptinVideoAd;
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
                return _optinVideoAd.Call<bool>("isLoaded");
#elif UNITY_IOS
                return ogury_optinVideoAd_isLoaded(_instanceId);
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
            _optinVideoAd.Call("load");
#elif UNITY_IOS
            ogury_optinVideoAd_load(_instanceId);
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
            _optinVideoAd.Call("show");
#elif UNITY_IOS
            ogury_optinVideoAd_show(_instanceId);
#endif
        }

        ~OguryOptinVideoAd()
        {
#if UNITY_EDITOR
            // Noop
#elif UNITY_IOS
            ogury_optinVideoAd_destroy(_instanceId);
#endif
        }
    }
}