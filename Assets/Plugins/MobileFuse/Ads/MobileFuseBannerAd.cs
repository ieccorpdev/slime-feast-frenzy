using UnityEngine;

// ReSharper disable once RedundantUsingDirective
using MobileFuseAds.Ads;
using MobileFuseAds.Interfaces;
using MobileFuseAds.Proxies;

namespace MobileFuseAds.Ads
{
    // This class is a special type of proxy, it forwards all of the implementation methods from IBannerAd to an 
    // internal instance of IBannerAd. This is preferable to having users implement IBannerAd directly as it doesn't
    // require any factory pattern:
    public class MobileFuseBannerAd : IBannerAd
    {
        private readonly IBannerAd _implementation;
        
        public MobileFuseBannerAd(string placementId, AdSize size)
        {
            #if UNITY_EDITOR
                _implementation = new UnityEditor.EditorBannerAd(placementId, size);
            #elif UNITY_ANDROID
                _implementation = new Android.AndroidBannerAd(placementId, size);
            #else
                throw new System.PlatformNotSupportedException("The current platform is not supported");
            #endif
        }
        
        // Method forwarding:
        public AdSize Size => _implementation.Size;
        public Color BackgroundColor { get => _implementation.BackgroundColor; set => _implementation.BackgroundColor = value; }
        public LayoutGravity Position { get => _implementation.Position; set => _implementation.Position = value; }
        public float GetRelativeHeight(float fullHeight = 0) => _implementation.GetRelativeHeight(fullHeight);
        public float GetRelativePositionY(float fullHeight = 0) => _implementation.GetRelativePositionY(fullHeight);
        public void ShowAd() => _implementation.ShowAd();
        public void LoadAd() => _implementation.LoadAd();
        public bool IsLoaded => _implementation.IsLoaded;
        public void SetListener(BannerAdListenerProxy listener) => _implementation.SetListener(listener);
    }
}