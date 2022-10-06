using MobileFuseAds.Proxies;

namespace MobileFuseAds.Interfaces
{
    public interface IInterstitialAd
    {
        void ShowAd();
        void LoadAd();
        bool IsLoaded { get; }
        void SetListener(InterstitialAdListenerProxy listener);
    }
}