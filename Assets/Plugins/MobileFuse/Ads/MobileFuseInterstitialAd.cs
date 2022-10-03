using MobileFuseAds.Ads.Android;
using MobileFuseAds.Interfaces;
using MobileFuseAds.Proxies;

namespace MobileFuseAds.Ads
{
    // This class is a special type of proxy, it forwards all of the implementation methods from IBannerAd to an 
    // internal instance of IBannerAd. This is preferable to having users implement IBannerAd directly as it doesn't
    // require any factory pattern:
    public class MobileFuseInterstitialAd : IInterstitialAd
    {
        private readonly IInterstitialAd _implementation;
        
        public MobileFuseInterstitialAd(string placementId)
        {
            #if UNITY_ANDROID
                _implementation = new AndroidInterstitialAd(placementId);
            #else
                throw new System.PlatformNotSupportedException("The current platform is not supported");
            #endif
        }
        
        // Method forwarding:
        public void ShowAd() => _implementation.ShowAd();
        public void LoadAd() => _implementation.LoadAd();
        public bool IsLoaded => _implementation.IsLoaded;
        public void SetListener(InterstitialAdListenerProxy listener) => _implementation.SetListener(listener);
    }
}