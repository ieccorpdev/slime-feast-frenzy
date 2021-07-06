using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace OgurySdk
{
    public class Ogury
    {
#if UNITY_ANDROID
        private static AndroidJavaObject ogury = new AndroidJavaObject("com.ogury.unity.Ogury");
#endif

#if UNITY_IOS
        [DllImport("__Internal", CharSet = CharSet.Ansi)]
        private static extern int ogury_start(string assetKey);

        [DllImport("__Internal")]
        private static extern int ogury_startAds();

        [DllImport("__Internal")]
        private static extern string ogury_getSdkVersion();
#endif

        public static void Start(string androidAssetKey, string iosAssetKey)
        {
#if UNITY_EDITOR
            // Noop
#elif UNITY_ANDROID
            ogury.CallStatic("start", androidAssetKey);
            Debug.Log("Android SDK Version : " + SdkVersion);
#elif UNITY_IOS
            ogury_start(iosAssetKey);
            Debug.Log("iOS SDK Version : " + SdkVersion);
#endif
        }

        public static void StartAds()
        {
#if UNITY_EDITOR
            // Noop
#elif UNITY_ANDROID
            // Noop
#elif UNITY_IOS
            ogury_startAds();
#endif
        }

        public static string SdkVersion
        {
            get
            {
#if UNITY_EDITOR
                return "0.0.0";
#elif UNITY_ANDROID
                return ogury.CallStatic<string>("getSdkVersion");
#elif UNITY_IOS
                return ogury_getSdkVersion();
#endif
            }
        }
    }
}