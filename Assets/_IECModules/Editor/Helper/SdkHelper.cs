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
        public const string GITLAB_GROUP_NAME = "iec-public-registry";

        public const string IRON_SOURCE_CURRENT_VERSION = "6.13.0";

        public const string FACEBOOK_AUDIENCE_NETWORK_SDK_BASE_URL =
            "https://developers.facebook.com/resources/audience-network-unity-sdk-{0}.zip";

        public const string FIREBASE_SDK_BASE_URL =
            "https://gitlab.com/iec-unity-plugins/firebase-unity-plugin/raw/master/{0}/FirebaseAnalytics.unitypackage";

        public const string APPLOVIN_SDK_BASE_URL =
            // "https://gitlab.com/iec-unity-plugins/applovin-unity-plugin/raw/master/AppLovin-MAX-Unity-Plugin-{0}-Android-9.11.1-iOS-6.11.1.unitypackage";
            "https://bintray.com/applovin/Unity/download_file?file_path=AppLovin-MAX-Unity-Plugin-{0}.unitypackage";

        public const string IRON_SOURCE_SDK_BASE_URL =
            "https://dl.bintray.com/ironsource-mobile/unity-sdk/IronSource_v{0}.unitypackage";

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
        public const string I2_LOCALIZATION_PKG = "com.i2.localization";

        public static string FacebookAudienceNetworkSdkUrlByVersion(string version = "6.2.1") =>
            string.Format(FACEBOOK_AUDIENCE_NETWORK_SDK_BASE_URL, version);

        public static string EDMUrlByVersion(string version) =>
            string.Format(EXTERNAL_PACKAGE_MANAGER_URL, version, version.Substring(1));

        public static string ApplovinLatestDownloadLink = string.Empty;

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
                },
                [I2_LOCALIZATION_PKG] = new GitlabPackageInfo
                {
                    id = I2_LOCALIZATION_PKG
                }
            };

        public static readonly FacebookSdkLoader FacebookSdkLoader = new FacebookSdkLoader();
        public static readonly AdjustSdkLoader AdjustSdkLoader = new AdjustSdkLoader();
        public static readonly KochavaSdkLoader KochavaSdkLoader = new KochavaSdkLoader();
        public static readonly OgurySdkLoader OgurySdkLoader = new OgurySdkLoader();
        public static readonly ApplovinMaxLoader ApplovinMaxLoader = new ApplovinMaxLoader();

        public static void LoadSdkVersions()
        {
            FacebookSdkLoader.Init();
            AdjustSdkLoader.Init();
            KochavaSdkLoader.Init();
            OgurySdkLoader.Init();
            ApplovinMaxLoader.Init();
            
            // LoadApplovinLatestVersion();
            
            foreach (var pkg in DictGitlabPluginVersion.Values)
            {
                pkg.LoadPKGVersionList();
            }

            var unityPackages = DictUnityPackage.Values;
            foreach (var pkg in unityPackages)
            {
                pkg.LoadPKGVersionList();
            }
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
                        if (version != "master" && version != "main")
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
            EditorApplication.ExecuteMenuItem("Assets/Refresh");
        }

        string GetGitlabUrlByVersion(string version)
        {
            return $"https://gitlab.com/{SdkHelper.GITLAB_GROUP_NAME}/{id}.git#{version}";
        }

        public virtual void InstallDependencies(string version)
        {
            var url = $"{SdkHelper.GITLAB_API}projects/{SdkHelper.GITLAB_GROUP_NAME}%2F{id}/repository/files/package.json/raw?ref={version}";
            var httpRequest = new HTTPRequest(new Uri(url), (request, response) =>
            {
                var dataItem = JSONNode.Parse(response.DataAsText);
                if (dataItem.HasKey("dependencies"))
                {
                    var dependencies = JsonConvert.DeserializeObject<Dictionary<string,string>>(dataItem["dependencies"].ToString());
                    foreach (var key in dependencies.Keys)
                    {
                        var dependency = SdkHelper.DictGitlabPluginVersion[key];
                        var versionDependency = dependencies[key];
                        dependency.Install(versionDependency);
                    }
                }
            }).Send();
        }

        public override void InstallPKG(string version)
        {
            SdkHelper.AddDependency(id,version);
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
