using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using BestHTTP;
using Newtonsoft.Json;
using NUnit.Framework;
using UnityEngine;
using SimpleJSON;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;

namespace _IECModules
{
    public static class SdkHelper
    {
        public const string GITLAB_API = "https://gitlab.com/api/v4/";
        public const string GITLAB_GROUP_NAME = "iec-public-google-registry";

        public const string APPLOVIN_VERSION_CACHE = "applovin_version_cache";
        public const string KOCHAVA_VERSION_CACHE = "kochava_version_cache";
        public const string ADJUST_VERSION_CACHE = "adjust_version_cache";

        public const string FACEBOOK_CURRENT_VERSION = "7.18.0";
        public const string IRON_SOURCE_CURRENT_VERSION = "6.13.0";
        public const string KOCHAVA_CURRENT_VERSION = "4.1.1";

        public const string FACEBOOK_SDK_BASE_URL =
            "https://lookaside.facebook.com/developers/resources/?id=facebook-unity-sdk-{0}.zip";

        public const string FACEBOOK_AUDIENCE_NETWORK_SDK_BASE_URL =
            "https://developers.facebook.com/resources/audience-network-unity-sdk-{0}.zip";

        public const string KOCHAVA_SDK_BASE_URL =
            "https://github.com/Kochava/unity-kochavatracker-releases/releases/download/v{0}/KochavaTrackerUnity.unitypackage";

        public const string ADJUST_SDK_BASE_URL =
            "https://github.com/adjust/unity_sdk/releases/download/v{0}/Adjust_v{0}.unitypackage";

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
        public const string FIREBASE_APP_CORE = "com.google.firebase.app";
        public const string FIREBASE_REMOTE_CONFIG_PKG = "com.google.firebase.remote-config";
        public const string FIREBASE_ANALYTIC_PKG = "com.google.firebase.analytics";
        public const string IN_APP_RATING_PKG = "com.google.play.review";
        public const string ANDROID_APPBUNDLE = "com.google.android.appbundle";
        public const string GOOGLE_PLAY_COMMON = "com.google.play.common";
        public const string GOOGLE_PLAY_CORE = "com.google.play.core";
        public const string NOTIFICATION_PKG = "com.unity.mobile.notifications";

        public static string FacebookSdkUrlByVersion => string.Format(FACEBOOK_SDK_BASE_URL, FACEBOOK_CURRENT_VERSION);
        public static string FirebaseSdkUrlByVersion(string version) => string.Format(FIREBASE_SDK_BASE_URL, version);
        public static string ApplovinSdkUrlByVersion(string version) => string.Format(APPLOVIN_SDK_BASE_URL, version);

        public static string FacebookAudienceNetworkSdkUrlByVersion(string version = "6.2.1") =>
            string.Format(FACEBOOK_AUDIENCE_NETWORK_SDK_BASE_URL, version);

        public static string EDMUrlByVersion(string version) =>
            string.Format(EXTERNAL_PACKAGE_MANAGER_URL, version, version.Substring(1));

        public static string IronSourceSdkUrlByVersion =>
            string.Format(IRON_SOURCE_SDK_BASE_URL, IRON_SOURCE_CURRENT_VERSION);

        public static string KochavaSdkUrlByVersion(string version) => string.Format(KOCHAVA_SDK_BASE_URL, version);
        public static string AdjustSdkUrlByVersion(string version) => string.Format(ADJUST_SDK_BASE_URL, version);

        public static string[] ListApplovinVersionDefault =
        {
            "3.0.0-Android-9.13.2-iOS-6.13.2",
            "2.4.16-Android-9.13.1-iOS-6.13.1"
        };

        public static string[] ListKochavaVersionDefault =
        {
            "3.0.0"
        };

        public static string[] ListAdjustVersionDefault =
        {
            "4.29.0"
        };

        public static string[] ListApplovinVersionFromBintray;
        public static string[] ListKochavaVersionFromGithub;
        public static string[] ListAdjustVersionFromGithub;

        public static string[] ListApplovinVersion =>
            ListApplovinVersionFromBintray == null ||
            ListApplovinVersionFromBintray.Length <= ListApplovinVersionDefault.Length
                ? ListApplovinVersionDefault
                : ListApplovinVersionFromBintray;

        public static string[] ListKochavaVersion =>
            ListKochavaVersionFromGithub == null ||
            ListKochavaVersionFromGithub.Length <= ListKochavaVersionDefault.Length
                ? ListKochavaVersionDefault
                : ListKochavaVersionFromGithub;

        public static string[] ListAdjustVersion =>
            ListAdjustVersionFromGithub == null ||
            ListAdjustVersionFromGithub.Length <= ListAdjustVersionDefault.Length
                ? ListAdjustVersionDefault
                : ListAdjustVersionFromGithub;


        public static string[] ListFirebaseVersion =
        {
            "6.15.2",
            "6.1.1"
        };

        public static Dictionary<string, UnityPackageInfo> DictUnityPackage = new Dictionary<string, UnityPackageInfo>
        {
            [NOTIFICATION_PKG] = new UnityPackageInfo {id = NOTIFICATION_PKG}
        };

        public static Dictionary<string, GitlabPackageInfo> DictGitlabPluginVersion =
            new Dictionary<string, GitlabPackageInfo>
            {
                [EXTERNAL_DEPENDENCY_PKG] = new GitlabPackageInfo
                {
                    id = EXTERNAL_DEPENDENCY_PKG
                }, //need for all firebase sdk, ad sdk,...
                [FIREBASE_APP_CORE] = new GitlabPackageInfo
                {
                    id = FIREBASE_APP_CORE,
                    nonContrainDependencies = new List<string>
                    {
                        EXTERNAL_DEPENDENCY_PKG
                    }
                }, //need for all firebase sdk
                [FIREBASE_REMOTE_CONFIG_PKG] = new GitlabPackageInfo
                {
                    id = FIREBASE_REMOTE_CONFIG_PKG,
                    constrainDependencies = new List<string>
                    {
                        FIREBASE_APP_CORE
                    }
                },
                [FIREBASE_ANALYTIC_PKG] = new GitlabPackageInfo
                {
                    id = FIREBASE_ANALYTIC_PKG,
                    constrainDependencies = new List<string>
                    {
                        FIREBASE_APP_CORE
                    }
                },
                [ANDROID_APPBUNDLE] = new GitlabPackageInfo {id = ANDROID_APPBUNDLE}, //need for google play review
                [GOOGLE_PLAY_COMMON] = new GitlabPackageInfo
                {
                    id = GOOGLE_PLAY_COMMON,
                    constrainDependencies = new List<string>
                    {
                        ANDROID_APPBUNDLE
                    }
                }, //need for google play review
                [GOOGLE_PLAY_CORE] = new GitlabPackageInfo
                {
                    id = GOOGLE_PLAY_CORE,
                    constrainDependencies = new List<string>
                    {
                        ANDROID_APPBUNDLE
                    }
                }, //need for google play review
                [IN_APP_RATING_PKG] = new GitlabPackageInfo
                {
                    id = IN_APP_RATING_PKG,
                    constrainDependencies = new List<string>
                    {
                        GOOGLE_PLAY_CORE,
                        GOOGLE_PLAY_COMMON
                    }
                }
            };

        public static void LoadApplovinVersionList()
        {
            PlayerPrefs.DeleteKey(APPLOVIN_VERSION_CACHE);
            var httpRequest = new HTTPRequest(
                    new Uri("https://api.bintray.com/packages/applovin/Unity/applovin-max-unity-plugin"),
                    (originalRequest, response) =>
                    {
                        PlayerPrefs.SetString(APPLOVIN_VERSION_CACHE, response.DataAsText);
                    })
                .Send();
        }

        public static void LoadKochavaVersionList()
        {
            PlayerPrefs.DeleteKey(KOCHAVA_VERSION_CACHE);
            var httpRequest = new HTTPRequest(
                new Uri("https://api.github.com/repos/Kochava/unity-kochavatracker-releases/releases"),
                (originalRequest, response) =>
                {
                    var data = JSONNode.Parse(response.DataAsText).AsArray;
                    var versions = new List<string>();
                    for (int i = 0; i < data.Count; i++)
                    {
                        versions.Add(data[i]["name"]);
                    }

                    PlayerPrefs.SetString(KOCHAVA_VERSION_CACHE, JsonConvert.SerializeObject(versions));
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
            if (ListKochavaVersionFromGithub != null) return true;
            var data = PlayerPrefs.GetString(KOCHAVA_VERSION_CACHE);
            if (string.IsNullOrEmpty(data)) return false;
            ListKochavaVersionFromGithub = JsonConvert.DeserializeObject<string[]>(data);
            return true;
        }

        public static void LoadAdjustVersionList()
        {
            PlayerPrefs.DeleteKey(ADJUST_VERSION_CACHE);
            var httpRequest = new HTTPRequest(
                new Uri("https://api.github.com/repos/adjust/unity_sdk/releases"),
                (originalRequest, response) =>
                {
                    var data = JSONNode.Parse(response.DataAsText).AsArray;
                    var versions = new List<string>();
                    for (int i = 0; i < data.Count; i++)
                    {
                        versions.Add(data[i]["name"].Value.Split(' ')[1]);
                    }

                    PlayerPrefs.SetString(ADJUST_VERSION_CACHE, JsonConvert.SerializeObject(versions));
                }).Send();
        }

        public static bool LoadAdjustVersionFromCache()
        {
            if (ListAdjustVersionFromGithub != null) return true;
            var data = PlayerPrefs.GetString(ADJUST_VERSION_CACHE);
            if (string.IsNullOrEmpty(data)) return false;
            ListAdjustVersionFromGithub = JsonConvert.DeserializeObject<string[]>(data);
            return true;
        }

        public static void AddDependency(string pkgName, string version)
        {
            var manifestFileName = "Packages/manifest.json";
            var manifestFileContent = File.ReadAllText(manifestFileName);

            var jsonNode = JSON.Parse(manifestFileContent);
            jsonNode["dependencies"][pkgName] = version;
            File.WriteAllText(manifestFileName, jsonNode.ToString(2));
        }

        public static void AddRegistries(RegistryManifest registry)
        {
            var manifestFileName = "Packages/manifest.json";
            var manifestFileContent = File.ReadAllText(manifestFileName);

            var jsonNode = JSON.Parse(manifestFileContent);
            jsonNode["scopedRegistries"] = new JSONArray();
            var scopes = new JSONArray();
            foreach (var regScope in registry.scopes)
            {
                scopes.Add(new JSONString(regScope));
            }

            var item = new JSONObject
            {
                ["name"] = registry.name,
                ["url"] = registry.url,
                ["scopes"] = scopes
            };
            jsonNode["scopedRegistries"].Add(item);
            File.WriteAllText(manifestFileName, jsonNode.ToString(2));
        }

        public static void RemoveRegistries(RegistryManifest registry)
        {
            var manifestFileName = "Packages/manifest.json";
            var manifestFileContent = File.ReadAllText(manifestFileName);

            var jsonNode = JSON.Parse(manifestFileContent);
            var scopeRegistry = jsonNode["scopedRegistries"];
            JSONNode googleRegistry = null;
            for (int i = 0; i < scopeRegistry.Count; i++)
            {
                if (scopeRegistry[i]["url"] == registry.url) googleRegistry = scopeRegistry[i];
            }

            if (googleRegistry != null) scopeRegistry.Remove(googleRegistry);
            File.WriteAllText(manifestFileName, jsonNode.ToString(2));
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

    public class GitlabPackageInfo : UnityPackageInfo
    {
        public List<string> nonContrainDependencies;
        public List<string> constrainDependencies;

        public void SetDependencies(List<string> constrainDependencies = null,
            List<string> nonContrainDependencies = null)
        {
            this.nonContrainDependencies = nonContrainDependencies;
            this.constrainDependencies = constrainDependencies;
        }

        public override void LoadPKGVersionList()
        {
            // Debug.Log("Load package:" + id);
            PlayerPrefs.DeleteKey(cache);
            var url = $"{SdkHelper.GITLAB_API}projects/{SdkHelper.GITLAB_GROUP_NAME}%2F{id}/repository/branches";
            var versions = new List<string>();
            var requestUrl100 = url + "?per_page=100";
            var httpRequest = new HTTPRequest(new Uri(requestUrl100), (request, response) =>
            {
                var numPage = int.Parse(response.Headers["x-total-pages"][0]);
                // Debug.Log(numPage);
                var listPageUrl = new List<string>();
                var data = JSONNode.Parse(response.DataAsText).AsArray;
                for (int i = 2; i <= numPage; i++)
                {
                    listPageUrl.Add($"{requestUrl100}&page={i}");
                }

                RequestListUrl(listPageUrl, 0, data, () =>
                {
                    for (int i = 0; i < data.Count; i++)
                    {
                        var version = data[i]["name"].Value;
                        if (version != "master")
                        {
                            versions.Add(version);
                        }
                    }

                    versions = versions.OrderByDescending(x => new Version(x)).ToList();
                    // Debug.Log(id);
                    // foreach (var version in versions)
                    // {
                    //     Debug.Log(version);
                    // }

                    PlayerPrefs.SetString(cache, JsonConvert.SerializeObject(versions));
                    versionSelected = 0;
                    LoadPKGVersionFromCache();
                });
            }).Send();
        }

        void RequestListUrl(List<string> listUrl, int index, JSONArray data, Action callback)
        {
            if (index < listUrl.Count)
            {
                var url = listUrl[index];
                // Debug.Log("page:" + url);
                var httpRequest = new HTTPRequest(new Uri(url), (request, response) =>
                {
                    var dataItem = JSONNode.Parse(response.DataAsText);
                    for (int i = 0; i < dataItem.Count; i++)
                    {
                        data.Add(dataItem[i]);
                    }

                    RequestListUrl(listUrl, index + 1, data, callback);
                }).Send();
            }
            else
            {
                callback?.Invoke();
            }
        }

        public void InstallHighestVersion()
        {
            var version = GetGitlabUrlByVersion(listVersion.First());
            // Debug.Log(version);
            InstallPKG(version);
        }

        public override void Install()
        {
            var version = listVersion[versionSelected];
            Install(version);
        }

        public void Install(string version)
        {
            LogEditorHelper.AddLog($"Begin install: {id}-{version}");
            InstallPKG(GetGitlabUrlByVersion(version));
            InstallDependencies(version);
        }

        string GetGitlabUrlByVersion(string version)
        {
            return $"https://gitlab.com/{SdkHelper.GITLAB_GROUP_NAME}/{id}.git#{version}";
        }

        public virtual void InstallDependencies(string pluginVersion)
        {
            if (constrainDependencies != null || nonContrainDependencies != null)
            {
                LogEditorHelper.AddLog($"Begin install dependencies");
            }

            if (constrainDependencies != null)
            {
                foreach (var constrainDependency in constrainDependencies)
                {
                    var dependency = SdkHelper.DictGitlabPluginVersion[constrainDependency];
                    dependency.Install(pluginVersion);
                }
            }

            if (nonContrainDependencies != null)
            {
                foreach (var nonContrainDependency in nonContrainDependencies)
                {
                    var dependency = SdkHelper.DictGitlabPluginVersion[nonContrainDependency];
                    dependency.InstallHighestVersion();
                }
            }
        }
    }

    public class UnityPackageInfo
    {
        public int versionSelected;
        string versionInstalled;
        public string[] listDefaultVerion = new string[] {"1.0"};
        public string[] listVersionLoad;

        public string[] listVersion =>
            listVersionLoad == null ||
            listVersionLoad.Length <= listDefaultVerion.Length
                ? listDefaultVerion
                : listVersionLoad;

        public string id;

        public string cache
        {
            get { return id + "_cache"; }
        }

        public virtual void LoadPKGVersionList()
        {
            PackageManagerHelper.Search(id, (searchRequest) =>
            {
                var packageInfo = searchRequest.Result[0];
                listVersionLoad = packageInfo.versions.compatible.Reverse().ToArray();
                versionSelected = 0;
                PlayerPrefs.SetString(cache, JsonConvert.SerializeObject(listVersionLoad));
            });
        }

        public virtual bool LoadPKGVersionFromCache()
        {
            if (listVersionLoad != null) return true;
            var data = PlayerPrefs.GetString(cache);
            if (string.IsNullOrEmpty(data)) return false;

            listVersionLoad = JSON.Parse(data).AsStringArray;
            return true;
        }

        public virtual void Install()
        {
            InstallPKG(listVersion[versionSelected]);
        }

        public virtual void InstallPKG(string version)
        {
            PackageManagerHelper.Add($"{id}@{version}");
        }

        public virtual void Uninstall()
        {
            PackageManagerHelper.Remove(id);
        }
    }
}