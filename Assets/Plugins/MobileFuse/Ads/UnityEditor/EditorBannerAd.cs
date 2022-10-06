using MobileFuseAds.Interfaces;
using MobileFuseAds.Proxies;
using UnityEngine;

namespace MobileFuseAds.Ads.UnityEditor
{
    internal class EditorBannerAd : IBannerAd
    {
        public AdSize Size { get; }

        private readonly string _placementId;
        private BannerAdListenerProxy _listener;

        public void LoadAd()
        { 
            Debug.Log("[MobileFuse BannerAd] Load Ad");
        }

        public bool IsLoaded
        {
            get; private set;
        }

        public void SetListener(BannerAdListenerProxy listener)
        {
            _listener = listener;
        }

        public EditorBannerAd(string placementId, AdSize adSize)
        {
            _placementId = placementId;
            Size = adSize;
        }
        
        public Color BackgroundColor { get; set; } = Color.black;
        public LayoutGravity Position { get; set; } = LayoutGravity.TOP;

        public float GetRelativeHeight(float fullHeight = 0)
        {
            if (fullHeight <= 0f) fullHeight = Screen.height;
            var heightRelativeToScreen = fullHeight / Screen.height;
            return (
                Size == AdSize.Banner300x50  ? 50  :
                Size == AdSize.Banner320x50  ? 50  :
                Size == AdSize.Banner300x250 ? 250 :
                Size == AdSize.Banner728x90  ? 90  :
                0
            ) * heightRelativeToScreen;
        }

        public float GetRelativePositionY(float fullHeight = 0)
        {
            if (fullHeight <= 0f) fullHeight = Screen.height;

            switch (Position)
            {
                case LayoutGravity.TOP:
                    return 0;
                case LayoutGravity.CENTER:
                    return fullHeight / 2f - GetRelativeHeight(fullHeight) / 2f;
                case LayoutGravity.BOTTOM:
                    return fullHeight - GetRelativeHeight(fullHeight);
                default:
                    return 0;
            }
        }
        
        public void ShowAd() { 
            Debug.Log("[MobileFuse BannerAd] Show ad");
        }
    }
}