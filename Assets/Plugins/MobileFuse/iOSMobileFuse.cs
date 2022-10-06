#if UNITY_IOS
using System;
using System.Runtime.InteropServices;
using MobileFuseAds.Helpers;
using MobileFuseAds.Interfaces;
using MobileFuseAds.Privacy.Android;
using MobileFuseAds.Privacy.iOS;
using MobileFuseAds.Proxies;
using UnityEngine;

namespace MobileFuseAds
{
    public class iOSMobileFuse : IMobileFuseCore
    {
        [DllImport("__Internal")]
        private static extern void MobileFuse_setPrivacyPreferences(uint instanceId);
        
        public void Init(int publisherId, int appId, MobileFuse.NoArgsDelegate onInitSuccess, MobileFuse.NoArgsDelegate onInitError)
        {
            // Not yet supported on iOS
        }

        public void SetPrivacyPreferences(IPrivacyPreferences prefs)
        {
            if (prefs is iOSPrivacyPreferences)
            {
                MobileFuse_setPrivacyPreferences(((iOSPrivacyPreferences)prefs).InstanceId);
            }
            else
            {
                throw new System.PlatformNotSupportedException($"Internal SDK error - expected privacy preferences to be iOSPrivacyPreferences but received {prefs}");
            }
        }
        
        void IMobileFuseCore.SetSettingOverride(string setting, string value)
        {
            // Not yet supported on iOS
        }
    }
}
#endif