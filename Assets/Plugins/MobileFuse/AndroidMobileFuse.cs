using MobileFuseAds.Helpers;
using MobileFuseAds.Interfaces;
using MobileFuseAds.Privacy.Android;
using MobileFuseAds.Proxies;
using UnityEngine;

namespace MobileFuseAds
{
    public class AndroidMobileFuse : IMobileFuseCore
    {
        public void Init(int publisherId, int appId, MobileFuse.NoArgsDelegate onInitSuccess, MobileFuse.NoArgsDelegate onInitError)
        {
            var proxy = new InitProxy();
            proxy.InitSuccess += onInitSuccess.Invoke;
            proxy.InitError += onInitError.Invoke;
            
            JavaHelpers.ExecuteOnMainThread(() =>
            {
                new AndroidJavaClass("com.mobilefuse.sdk.MobileFuse").CallStatic("init", CurrentActivity, publisherId, appId, proxy);
            });
        }

        public void SetPrivacyPreferences(IPrivacyPreferences prefs)
        {
            if (prefs is AndroidPrivacyPreferences)
            {
                new AndroidJavaClass("com.mobilefuse.sdk.MobileFuse").CallStatic("setPrivacyPreferences", ((AndroidPrivacyPreferences)prefs).JavaObject);
            }
            else
            {
                throw new System.PlatformNotSupportedException($"Internal SDK error - expected privacy preferences to be AndroidPrivacyPreferences but received {prefs}");
            }
        }

        void IMobileFuseCore.SetSettingOverride(string setting, string value)
        {
            var settingEnum = new AndroidJavaClass("com.mobilefuse.sdk.MobileFuseSetting").GetStatic<AndroidJavaObject>(setting);
            new AndroidJavaClass("com.mobilefuse.sdk.MobileFuseSettings").CallStatic("setOverride", settingEnum, value);
        }

        private static AndroidJavaObject _bridge;
        internal static AndroidJavaObject Bridge
        {
            get
            {
                if (_bridge != null) return _bridge;
                _bridge = new AndroidJavaObject("com.mobilefuse.unity.AndroidBridge");
                return _bridge;
            }
        }

        internal static AndroidJavaObject CurrentActivity => 
            new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
    }
}