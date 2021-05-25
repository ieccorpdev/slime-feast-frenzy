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
    public class OguryInterstitialAd
    {
#pragma warning disable 649
        private readonly int _instanceId;
#pragma warning restore 649

#if UNITY_ANDROID
        private AndroidJavaObject insterstitialAd;
#endif

#if UNITY_IOS
        [DllImport("__Internal", CharSet = CharSet.Ansi)] 
        private static extern int ogury_interstitialAd_create(string adUnitId);
        
        [DllImport("__Internal")] 
        private static extern void ogury_interstitialAd_load(int instanceId);
        
        [DllImport("__Internal")] 
        private static extern bool ogury_interstitialAd_isLoaded(int instanceId);
        
        [DllImport("__Internal")] 
        private static extern bool ogury_interstitialAd_show(int instanceId);
        
        [DllImport("__Internal")] 
        private static extern int ogury_interstitialAd_destroy(int instanceId);
#endif
        
        public event InterstitialAdEventHandler OnAdNotAvailable;
        public event InterstitialAdEventHandler OnAdLoaded;
        public event InterstitialAdEventHandler OnAdNotLoaded;
        public event InterstitialAdEventHandler OnAdDisplayed;
        public event InterstitialAdEventHandler OnAdClosed;
        public event InterstitialAdErrorEventHandler OnAdError;

        public delegate void InterstitialAdErrorEventHandler(OguryInterstitialAd interstitialAd, OguryError error);

        public delegate void InterstitialAdEventHandler(OguryInterstitialAd interstitialAd);

        public OguryInterstitialAd(string androidAdUnitId, string iOsAdUnitId)
        {
#if UNITY_EDITOR
            // Noop
#elif UNITY_ANDROID
            insterstitialAd = new AndroidJavaObject("com.ogury.unity.interstitial.OguryInterstitialAd", androidAdUnitId);
            _instanceId = insterstitialAd.Call<int>("getInstanceId");
#elif UNITY_IOS
            _instanceId = ogury_interstitialAd_create(iOsAdUnitId);
#endif

            if (OguryCallbacks.CheckIfPresent())
            {
                // The captures references and the lambda construct for every callback is mandatory.
                // This is made in order that the OguryCallbacks singleton does not maintain a reference to this instance.
                // This will allow the GC to clean this instance when necessary and allow us to clean our native instance also.
                int capturedInstanceId = _instanceId;
                WeakReference capturedWeakInterstitialAd = new WeakReference(this);
                
                OguryCallbacks.Instance.OnAdNotAvailable += instanceId =>
                {
                    OguryInterstitialAd strong = capturedWeakInterstitialAd.Target as OguryInterstitialAd;
                    if (instanceId == capturedInstanceId 
                        && strong != null
                        && strong.OnAdNotAvailable != null)
                    {
                        strong.OnAdNotAvailable.Invoke(strong);
                    }
                };
                OguryCallbacks.Instance.OnAdLoaded += instanceId =>
                {
                    OguryInterstitialAd strong = capturedWeakInterstitialAd.Target as OguryInterstitialAd;
                    if (instanceId == capturedInstanceId 
                        && strong != null
                        && strong.OnAdLoaded != null)
                    {
                        strong.OnAdLoaded.Invoke(strong);
                    }
                };
                OguryCallbacks.Instance.OnAdNotLoaded += instanceId =>
                {
                    OguryInterstitialAd strong = capturedWeakInterstitialAd.Target as OguryInterstitialAd;
                    if (instanceId == capturedInstanceId 
                        && strong != null
                        && strong.OnAdNotLoaded != null)
                    {
                        strong.OnAdNotLoaded.Invoke(strong);
                    }
                };
                OguryCallbacks.Instance.OnAdDisplayed += instanceId =>
                {
                    OguryInterstitialAd strong = capturedWeakInterstitialAd.Target as OguryInterstitialAd;
                    if (instanceId == capturedInstanceId 
                        && strong != null
                        && strong.OnAdDisplayed != null)
                    {
                        strong.OnAdDisplayed.Invoke(strong);
                    }
                };
                OguryCallbacks.Instance.OnAdClosed += instanceId =>
                {
                    OguryInterstitialAd strong = capturedWeakInterstitialAd.Target as OguryInterstitialAd;
                    if (instanceId == capturedInstanceId 
                        && strong != null
                        && strong.OnAdClosed != null)
                    {
                        strong.OnAdClosed.Invoke(strong);
                    }
                };
                OguryCallbacks.Instance.OnAdError += (instanceId, error) =>
                {
                    OguryInterstitialAd strong = capturedWeakInterstitialAd.Target as OguryInterstitialAd;
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
                return insterstitialAd.Call<bool>("isLoaded");
#elif UNITY_IOS
                return ogury_interstitialAd_isLoaded(_instanceId);
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
            insterstitialAd.Call("load");
#elif UNITY_IOS
            ogury_interstitialAd_load(_instanceId);
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
            insterstitialAd.Call("show");
#elif UNITY_IOS
            ogury_interstitialAd_show(_instanceId);
#endif
        }

        ~OguryInterstitialAd()
        {
#if UNITY_EDITOR
            // Noop
#elif UNITY_IOS
            ogury_interstitialAd_destroy(_instanceId);
#endif
        }
    }
}