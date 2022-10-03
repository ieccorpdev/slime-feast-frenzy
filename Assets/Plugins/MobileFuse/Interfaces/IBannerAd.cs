using MobileFuseAds.Ads;
using MobileFuseAds.Proxies;
using UnityEngine;

namespace MobileFuseAds.Interfaces
{
    public interface IBannerAd
    {
        AdSize Size { get; }
        Color BackgroundColor { get; set; }
        LayoutGravity Position { get; set; }
        float GetRelativeHeight(float fullHeight = 0);
        float GetRelativePositionY(float fullHeight = 0);
        void ShowAd();
        void LoadAd();
        bool IsLoaded { get; }
        void SetListener(BannerAdListenerProxy listener);
    }
}