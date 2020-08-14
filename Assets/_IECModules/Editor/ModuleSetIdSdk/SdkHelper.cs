using System;
using System.Collections.Generic;
using System.Linq;
using BestHTTP;
using Newtonsoft.Json;
using UnityEngine;

namespace _IECModules
{
    public class SdkHelper
    {
        public const string APPLOVIN_VERSION_CACHE = "applovin_version_cache";
        
        public const string FACEBOOK_CURRENT_VERSION = "7.18.0";
        public const string IRON_SOURCE_CURRENT_VERSION = "6.13.0";
        public const string KOCHAVA_CURRENT_VERSION = "4.1.1";

        public const string FACEBOOK_SDK_BASE_URL =
            "https://origincache.facebook.com/developers/resources/?id=facebook-unity-sdk-{0}.zip";
        
        public const string KOCHAVA_SDK_BASE_URL =
            "https://bintray.com/kochava/Generic/download_file?file_path={0}%2FKochavaTrackerUnity.unitypackage";
        
        public const string FIREBASE_SDK_BASE_URL =
            "https://gitlab.com/iec-unity-plugins/firebase-unity-plugin/raw/master/{0}/FirebaseAnalytics.unitypackage";
        
        public const string APPLOVIN_SDK_BASE_URL =
            // "https://gitlab.com/iec-unity-plugins/applovin-unity-plugin/raw/master/AppLovin-MAX-Unity-Plugin-{0}-Android-9.11.1-iOS-6.11.1.unitypackage";
            "https://bintray.com/applovin/Unity/download_file?file_path=AppLovin-MAX-Unity-Plugin-{0}.unitypackage";
        
        public const string IRON_SOURCE_SDK_BASE_URL =
            "https://dl.bintray.com/ironsource-mobile/unity-sdk/IronSource_v{0}.unitypackage";

        public const string FACEBOOK_SDK_CURRENT_VERSION_URL =
            "https://origincache.facebook.com/developers/resources/?id=FacebookSDK-current.zip";
        
        public static string FacebookSdkUrlByVersion => string.Format(FACEBOOK_SDK_BASE_URL, FACEBOOK_CURRENT_VERSION);
        public static string FirebaseSdkUrlByVersion(string version) => string.Format(FIREBASE_SDK_BASE_URL, version);
        public static string ApplovinSdkUrlByVersion(string version) => string.Format(APPLOVIN_SDK_BASE_URL, version);
        
        public static string IronSourceSdkUrlByVersion => string.Format(IRON_SOURCE_SDK_BASE_URL, IRON_SOURCE_CURRENT_VERSION);
        public static string KochavaSdkUrlByVersion => string.Format(KOCHAVA_SDK_BASE_URL, KOCHAVA_CURRENT_VERSION);

        public static string[] ListApplovinVersionDefault = {
            "3.0.0-Android-9.13.2-iOS-6.13.2",
            "2.4.16-Android-9.13.1-iOS-6.13.1"
        };
        public static string[] ListApplovinVersionFromBintray;

        public static string[] ListApplovinVersion =>
            ListApplovinVersionFromBintray == null ||
            ListApplovinVersionFromBintray.Length <= ListApplovinVersionDefault.Length
                ? ListApplovinVersionDefault
                : ListApplovinVersionFromBintray;

        public static string[] ListFirebaseVersion = {
            "6.15.2",
            "6.1.1"
        };

        public static void LoadApplovinVersionList()
        {
            PlayerPrefs.DeleteKey(APPLOVIN_VERSION_CACHE);
            var httpRequest = new HTTPRequest(
                new Uri("https://api.bintray.com/packages/applovin/Unity/applovin-max-unity-plugin"),
                (originalRequest, response) =>
                {
                    PlayerPrefs.SetString(APPLOVIN_VERSION_CACHE, response.DataAsText);
                }).Send();

        }
        public static bool LoadApplovinVersionFromCache()
        {
            if (ListApplovinVersionFromBintray != null) return true;
            var data = PlayerPrefs.GetString(APPLOVIN_VERSION_CACHE);
            if (string.IsNullOrEmpty(data)) return false;
            var bintrayApplovinPackageData = JsonConvert.DeserializeObject<BintrayApplovinPackageData>(data);
            ListApplovinVersionFromBintray = 
                bintrayApplovinPackageData.versions.Select(x => 
                    x.Replace("unity_", "")
                        .Replace("_", "-")
                        .Replace("android", "Android")
                        .Replace("ios", "iOS")
                ).ToArray();
            return true;
        }
    }

    [Serializable]
    public class BintrayApplovinPackageData
    {
        public string name;
        public string repo;
        public string[] versions;
        public string latest_version;
        public string updated;
    } 
}