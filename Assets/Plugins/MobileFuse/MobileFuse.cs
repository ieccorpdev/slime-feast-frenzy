using MobileFuseAds.Interfaces;
using UnityEngine;

namespace MobileFuseAds
{
    public static class MobileFuse
    {
        public delegate void NoArgsDelegate();

        private static IMobileFuseCore _core;

        static MobileFuse()
        {
            Debug.Log(MobileFuseUnitySdkConfig.ToString());
        }

        private static IMobileFuseCore PlatformSpecificCore
        {
            get
            {
                if (_core != null) return _core;

#if UNITY_ANDROID
                _core = new AndroidMobileFuse();
#elif UNITY_IOS
                _core = new iOSMobileFuse();
#endif

                return _core;
            }
        }

        public static void Init(int publisherId, int appId, NoArgsDelegate onInitSuccess, NoArgsDelegate onInitError)
        {
            PlatformSpecificCore.Init(publisherId, appId, onInitSuccess, onInitError);
        }

        public static void SetPrivacyPreferences(IPrivacyPreferences preferences)
        {
            PlatformSpecificCore.SetPrivacyPreferences(preferences);
        }

        internal static void SetSettingOverride(string setting, string value)
        {
            PlatformSpecificCore.SetSettingOverride(setting, value);
        }
    }
}