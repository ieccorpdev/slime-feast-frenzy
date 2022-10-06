using MobileFuseAds.Helpers;
using MobileFuseAds.Interfaces;
using MobileFuseAds.Proxies;
using Plugins.MobileFuse.Helpers;
using UnityEngine;

namespace MobileFuseAds.Ads.Android
{
    internal class AndroidBannerAd : ProxiedJavaInstance, IBannerAd
    {
        public AdSize Size { get; }
        
        private Color _backgroundColor = Color.black;
        private LayoutGravity _layoutGravity = LayoutGravity.TOP;

        // Simple proxy methods:
        public void LoadAd() => Call("loadAd");
        public bool IsLoaded => Call<bool>("isLoaded");
        public void SetListener(BannerAdListenerProxy listener) => Call("setListener", listener);

        // CSharp API:
        public AndroidBannerAd(string placementId, AdSize adSize) : base("com.mobilefuse.sdk.MobileFuseBannerAd", AndroidMobileFuse.CurrentActivity, placementId, MobileFuseAdSizeHelpers.GetJavaEnum(adSize)) 
        {
            Size = adSize;
        }
        
        public Color BackgroundColor
        {
            get => _backgroundColor;
            set
            {
                _backgroundColor = value;
                var color = 
                    ((byte)(_backgroundColor.a * 255) << 24) + 
                    ((byte)(_backgroundColor.r * 255) << 16) + 
                    ((byte)(_backgroundColor.g * 255) << 8 ) + 
                    ((byte)(_backgroundColor.b * 255) << 0 );
                Call("setBackgroundColor", color);
            }
        }

        public LayoutGravity Position
        {
            get => _layoutGravity;
            set
            {
                _layoutGravity = value;
                var layoutParams = Call<AndroidJavaObject>("getLayoutParams");
                var gravityEnum = new AndroidJavaClass("android.view.Gravity").GetStatic<int>(_layoutGravity.ToString());
                layoutParams.Set("gravity", gravityEnum);
                Call("setLayoutParams", layoutParams);
            }
        }

        public float GetRelativeHeight(float fullHeight = 0)
        {
            if (fullHeight <= 0f) fullHeight = Screen.height;
            
            var screenDensity = AndroidMobileFuse.CurrentActivity.Call<AndroidJavaObject>("getResources").Call<AndroidJavaObject>("getDisplayMetrics").Get<float>("density");
            var screenDp = Screen.height/screenDensity;
            return MobileFuseAdSizeHelpers.GetHeight(Size) / screenDp * fullHeight;
        }

        public float GetRelativePositionY(float fullHeight = 0)
        {
            Debug.Log($"[POS] Get relative position Y, relative to {fullHeight}");
            if (fullHeight <= 0f) fullHeight = Screen.height;

            switch (_layoutGravity)
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
            // Get the root view.
            Debug.Log("[MobileFuseBannerAd] Trying to add myself to the root view now!");
            var activity = AndroidMobileFuse.CurrentActivity;
            var rootView = activity.Call<AndroidJavaObject>("getWindow").Call<AndroidJavaObject>("getDecorView").Call<AndroidJavaObject>("getRootView");
            
            JavaHelpers.ExecuteOnMainThread(() =>
            {
                rootView.Call("addView", JavaInstance);
                Call("requestLayout");
            });
        }
    }
}