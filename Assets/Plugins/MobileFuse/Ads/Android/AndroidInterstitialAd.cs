using MobileFuseAds.Interfaces;
using MobileFuseAds.Proxies;
using Plugins.MobileFuse.Helpers;

namespace MobileFuseAds.Ads.Android
{
    public class AndroidInterstitialAd : ProxiedJavaInstance, IInterstitialAd
    {
        public AndroidInterstitialAd(string placementId) : base("com.mobilefuse.sdk.MobileFuseInterstitialAd", AndroidMobileFuse.CurrentActivity, placementId) { }
        public void LoadAd() => Call("loadAd");
        public bool IsLoaded => Call<bool>("isLoaded");
        public void ShowAd() => Call("showAd");
        public void SetListener(InterstitialAdListenerProxy listener) => Call("setListener", listener);
    }
}