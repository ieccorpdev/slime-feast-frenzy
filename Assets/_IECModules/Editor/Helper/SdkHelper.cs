using System;
using System.Collections.Generic;
using System.Linq;
using BestHTTP;
using Newtonsoft.Json;
using UnityEngine;
using SimpleJSON;
using UnityEditor.PackageManager;

namespace _IECModules
{
    public static class SdkHelper
    {
        public const string APPLOVIN_VERSION_CACHE = "applovin_version_cache";
        public const string KOCHAVA_VERSION_CACHE = "kochava_version_cache";
        public const string FACEBOOK_CURRENT_VERSION = "7.18.0";
        public const string IRON_SOURCE_CURRENT_VERSION = "6.13.0";
        public const string KOCHAVA_CURRENT_VERSION = "4.1.1";

        public const string FACEBOOK_SDK_BASE_URL =
            "https://lookaside.facebook.com/developers/resources/?id=facebook-unity-sdk-{0}.zip";

        public const string FACEBOOK_AUDIENCE_NETWORK_SDK_BASE_URL =
            "https://developers.facebook.com/resources/audience-network-unity-sdk-{0}.zip";

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
            "https://lookaside.facebook.com/developers/resources/?id=FacebookSDK-current.zip";

        public const string EXTERNAL_PACKAGE_MANAGER_URL =
            "https://github.com/googlesamples/unity-jar-resolver/raw/{0}/external-dependency-manager-{1}.unitypackage";



        public const string EXTERNAL_DEPENDENCY_PKG = "com.google.external-dependency-manager";
        public const string FIREBASE_REMOTE_CONFIG_PKG = "com.google.firebase.remote-config";
        public const string FIREBAE_ANALYTIC_PKG = "com.google.firebase.analytics";
        public const string IN_APP_RATING_PKG = "com.google.play.review";

        public static string FacebookSdkUrlByVersion => string.Format(FACEBOOK_SDK_BASE_URL, FACEBOOK_CURRENT_VERSION);
        public static string FirebaseSdkUrlByVersion(string version) => string.Format(FIREBASE_SDK_BASE_URL, version);
        public static string ApplovinSdkUrlByVersion(string version) => string.Format(APPLOVIN_SDK_BASE_URL, version);
        public static string FacebookAudienceNetworkSdkUrlByVersion(string version = "6.2.1") => string.Format(FACEBOOK_AUDIENCE_NETWORK_SDK_BASE_URL, version);
        public static string EDMUrlByVersion(string version) => string.Format(EXTERNAL_PACKAGE_MANAGER_URL, version, version.Substring(1));

        public static string IronSourceSdkUrlByVersion => string.Format(IRON_SOURCE_SDK_BASE_URL, IRON_SOURCE_CURRENT_VERSION);
        public static string KochavaSdkUrlByVersion(string version) => string.Format(KOCHAVA_SDK_BASE_URL, version);

        public static string[] ListApplovinVersionDefault = {
            "3.0.0-Android-9.13.2-iOS-6.13.2",
            "2.4.16-Android-9.13.1-iOS-6.13.1"
        };

        public static string[] ListKochavaVersionDefault = {
            "3.0.0"
        };

        public static string[] ListApplovinVersionFromBintray;
        public static string[] ListKochavaVersionFromBintray;

        public static string[] ListApplovinVersion =>
            ListApplovinVersionFromBintray == null ||
            ListApplovinVersionFromBintray.Length <= ListApplovinVersionDefault.Length
                ? ListApplovinVersionDefault
                : ListApplovinVersionFromBintray;

        public static string[] ListKochavaVersion =>
            ListKochavaVersionFromBintray == null ||
            ListKochavaVersionFromBintray.Length <= ListKochavaVersionDefault.Length
                ? ListKochavaVersionDefault
                : ListKochavaVersionFromBintray;

        public static string[] ListFirebaseVersion = {
            "6.15.2",
            "6.1.1"
        };

        public static Dictionary<string, UnityPackageInfo> DictUnityPackage = new Dictionary<string, UnityPackageInfo>
        {
            [EXTERNAL_DEPENDENCY_PKG] = new UnityPackageInfo { id = EXTERNAL_DEPENDENCY_PKG },
            [FIREBASE_REMOTE_CONFIG_PKG] = new UnityPackageInfo { id = FIREBASE_REMOTE_CONFIG_PKG },
            [FIREBAE_ANALYTIC_PKG] = new UnityPackageInfo { id = FIREBAE_ANALYTIC_PKG },
            [IN_APP_RATING_PKG] = new UnityPackageInfo { id = IN_APP_RATING_PKG }
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

        public static void LoadKochavaVersionList()
        {
            PlayerPrefs.DeleteKey(KOCHAVA_VERSION_CACHE);
            var httpRequest = new HTTPRequest(
                new Uri("https://api.bintray.com/packages/kochava/Generic/KochavaTracker-Unity"),
                (originalRequest, response) =>
                {
                    PlayerPrefs.SetString(KOCHAVA_VERSION_CACHE, response.DataAsText);
                }).Send();
        }


        public static bool LoadApplovinVersionFromCache()
        {
            if (ListApplovinVersionFromBintray != null) return true;
            var data = PlayerPrefs.GetString(APPLOVIN_VERSION_CACHE);
            if (string.IsNullOrEmpty(data)) return false;
            var bintrayApplovinPackageData = JsonConvert.DeserializeObject<BintrayPackageData>(data);
            ListApplovinVersionFromBintray =
                bintrayApplovinPackageData.versions.Select(x =>
                    x.Replace("unity_", "")
                        .Replace("_", "-")
                        .Replace("android", "Android")
                        .Replace("ios", "iOS")
                ).ToArray();
            return true;
        }

        public static bool LoadKochavaVersionFromCache()
        {
            if (ListKochavaVersionFromBintray != null) return true;
            var data = PlayerPrefs.GetString(KOCHAVA_VERSION_CACHE);
            if (string.IsNullOrEmpty(data)) return false;
            var bintrayKochavaPackageData = JsonConvert.DeserializeObject<BintrayPackageData>(data);
            ListKochavaVersionFromBintray =
                bintrayKochavaPackageData.versions.Select(x =>
                    x.Replace("unity_", "")
                        .Replace("_", "-")
                        .Replace("android", "Android")
                        .Replace("ios", "iOS")
                ).ToArray();
            return true;
        }
    }

    [Serializable]
    public class BintrayPackageData
    {
        public string name;
        public string repo;
        public string[] versions;
        public string latest_version;
        public string updated;
    }

    public class UnityPackageInfo
    {
        public int versionSelected;
        string versionInstalled;
        public string[] listDefaultVerion = new string[] { "1.0" };
        public string[] listVersionLoadFromUnity;
        public string[] listVersion =>
                listVersionLoadFromUnity == null ||
                listVersionLoadFromUnity.Length <= listDefaultVerion.Length
                ? listDefaultVerion
                : listVersionLoadFromUnity;

        public string id;
        public string cache
        {
            get
            {
                return id + "_cache";
            }
        }

        public void LoadPKGVersionList()
        {
            PackageManagerHelper.Search(id, (searchRequest) =>
            {
                var packageInfo = searchRequest.Result[0];
                listVersionLoadFromUnity = packageInfo.versions.compatible.Reverse().ToArray();
                PlayerPrefs.SetString(cache, JsonConvert.SerializeObject(listVersionLoadFromUnity));
            });
        }

        public bool LoadPKGVersionFromCache()
        {
            if (listVersionLoadFromUnity != null) return true;
            var data = PlayerPrefs.GetString(cache);
            if (string.IsNullOrEmpty(data)) return false;

            listVersionLoadFromUnity = JSON.Parse(data).AsStringArray;
            return true;
        }

        public void Install()
        {
            PackageManagerHelper.Add($"{id}@{listVersion[versionSelected]}");
        }

        public void Uninstall()
        {
            PackageManagerHelper.Remove(id);
        }
    }
}
