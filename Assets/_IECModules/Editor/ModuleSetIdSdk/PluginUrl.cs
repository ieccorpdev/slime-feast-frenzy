using System.Collections.Generic;
using System.Linq;

namespace _IECModules
{
    public class PluginUrl
    {

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
        public static string ApplovinSdkUrlByVersion(string version) => string.Format(APPLOVIN_SDK_BASE_URL, ApplovinSdkNameByVersion[version]);
        
        public static string IronSourceSdkUrlByVersion => string.Format(IRON_SOURCE_SDK_BASE_URL, IRON_SOURCE_CURRENT_VERSION);
        public static string KochavaSdkUrlByVersion => string.Format(KOCHAVA_SDK_BASE_URL, KOCHAVA_CURRENT_VERSION);

        public static Dictionary<string, string> ApplovinSdkNameByVersion = new Dictionary<string, string>
        {
            {"3.0.0", "3.0.0-Android-9.13.2-iOS-6.13.2"},
            {"2.4.16", "2.4.16-Android-9.13.1-iOS-6.13.1"},
            {"2.3.5", "2.4.16-Android-9.11.6-iOS-6.11.6"}
        };

        private static string[] m_ListApplovinVersion;
        public static string[] ListApplovinVersion => m_ListApplovinVersion ?? (m_ListApplovinVersion = ApplovinSdkNameByVersion.Keys.ToArray());

        public static string[] ListFirebaseVersion = {
            "6.15.2",
            "6.1.1"
        };
    }
}