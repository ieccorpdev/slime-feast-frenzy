using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using BestHTTP;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using ICSharpCode.SharpZipLib.BZip2;
using ICSharpCode.SharpZipLib.Zip;


namespace _IECModules
{
    public class ModuleSetIdSdk : EditorWindow
    {
        [MenuItem("IEC/Set ID SDK")]
        public static void ShowWindow()
        {
            //Show existing window instance. If one doesn't exist, make one.
            GetWindow(typeof(ModuleSetIdSdk));
        }

        private const string TempIECDirectory = "Temp/IEC/";
        
        private string m_GoogleSheetUrl =
            "https://docs.google.com/spreadsheets/d/1L7XrEic88trSrhg3drmlA5X5YkbP5ZMxRx3u1gUWcYg/edit#gid=927018210";

        private const string FacebookCurrentVersion = "7.18.0";
        private const string FirebaseCurrentVersion = "6.1.1";
        private const string ApplovinCurrentVersion = "2.3.0";
        private const string IronSourceCurrentVersion = "6.13.0";

        private const string FacebookSdkBaseUrl =
            "https://origincache.facebook.com/developers/resources/?id=facebook-unity-sdk-{0}.zip";
        
        private const string FirebaseSdkBaseUrl =
            "https://gitlab.com/iec-unity-plugins/firebase-unity-plugin/raw/master/{0}/FirebaseAnalytics.unitypackage";
        
        private const string ApplovinSdkBaseUrl =
            "https://gitlab.com/iec-unity-plugins/applovin-unity-plugin/raw/master/AppLovin-MAX-Unity-Plugin-{0}-Android-9.11.1-iOS-6.11.1.unitypackage";
        
        private const string IronSourceSdkBaseUrl =
            "https://dl.bintray.com/ironsource-mobile/unity-sdk/IronSource_v{0}.unitypackage";

        private const string FacebookSdkCurrentVersionUrl =
            "https://origincache.facebook.com/developers/resources/?id=FacebookSDK-current.zip";
        private static string FacebookSdkUrlByVersion => string.Format(FacebookSdkBaseUrl, FacebookCurrentVersion);
        private static string FirebaseSdkUrlByVersion => string.Format(FirebaseSdkBaseUrl, FirebaseCurrentVersion);
        private static string ApplovinSdkUrlByVersion => string.Format(ApplovinSdkBaseUrl, ApplovinCurrentVersion);
        private static string IronSourceSdkUrlByVersion => string.Format(IronSourceSdkBaseUrl, IronSourceCurrentVersion);

        private string m_SetSdkLog = "";

        public enum PluginDefineSymbol
        {
            PluginFacebook,
            PluginFirebase,
            PluginFirebaseAnalytic,
            PluginFirebaseRemote,
            PluginKochava,
            PluginApplovin,
            PluginIronSource,
            PluginUnityIAP,
        }

        public enum ChecklistItemName
        {
            GameName,
            BundleID_,
            KanboardSwimlane,
            DiscordChannel,
            FirebaseProject,
            UnityCloudProject,
            UnityCloudBuildKanboardWebhooks,
            UnityCloudBuildDiscordWebhooks,
            GitlabProject,
            GitlabIntegration,
            UnityCloudBuildTargetConfig,
            TrackingEventList,
            FacebookID,
            FirebaseConfigFileIOS,
            FirebaseConfigFileAndroid,
            KochavaIDIOS,
            KochavaIDAndroid,
            AppodealIDIOS,
            AppodealIDAndroid,
            AdmobIDIOS,
            AdmobIDAndroid,
            MarketingChecklist,
            IAPChecklist,
            IAPIOSList,
            IAPAndroidList,
            AdPosition,
            ApplovinIOSID,
            ApplovinAndroidID,
            ApplovinIOSInterID,
            ApplovinIOSBannerID,
            ApplovinIOSRewardID,
            ApplovinAndroidInterID,
            ApplovinAndroidBannerID,
            ApplovinAndroidRewardID,
            IronSourceIOSID,
            IronSourceAndroidID
        }

        private static Dictionary<string, ChecklistItemName> _checklistKey = new Dictionary<string, ChecklistItemName>
        {
            {"Tên game", ChecklistItemName.GameName},
            {"BundleID", ChecklistItemName.BundleID_},
            {"Kanboard Swimlane", ChecklistItemName.KanboardSwimlane},
            {"Discord Channel", ChecklistItemName.DiscordChannel},
            {"Firebase Project", ChecklistItemName.FirebaseProject},
            {"Unity Cloud Project", ChecklistItemName.UnityCloudProject},
            {"Unity Cloud Build Kanboard Webhooks", ChecklistItemName.UnityCloudBuildKanboardWebhooks},
            {"Unity Cloud Build Discord Webhooks", ChecklistItemName.UnityCloudBuildDiscordWebhooks},
            {"Gitlab project", ChecklistItemName.GitlabProject},
            {"Gitlab integration", ChecklistItemName.GitlabIntegration},
            {"Unity Cloud Build Target Config", ChecklistItemName.UnityCloudBuildTargetConfig},
            {"Tracking Event List", ChecklistItemName.TrackingEventList},
            {"Facebook ID", ChecklistItemName.FacebookID},
            {"Firebase Config File IOS", ChecklistItemName.FirebaseConfigFileIOS},
            {"Firebase Config File Android", ChecklistItemName.FirebaseConfigFileAndroid},
            {"Kochava ID IOS", ChecklistItemName.KochavaIDIOS},
            {"Kochava ID Android", ChecklistItemName.KochavaIDAndroid},
            {"Appodeal ID IOS", ChecklistItemName.AppodealIDIOS},
            {"Appodeal ID Android", ChecklistItemName.AppodealIDAndroid},
            {"Admob ID IOS", ChecklistItemName.AdmobIDIOS},
            {"Admob ID Android", ChecklistItemName.AdmobIDAndroid},
            {"Marketing Checklist", ChecklistItemName.MarketingChecklist},
            {"IAP Checklist", ChecklistItemName.IAPChecklist},
            {"IAP IOS list", ChecklistItemName.IAPIOSList},
            {"IAP Android list", ChecklistItemName.IAPAndroidList},
            {"Vị trí đặt quảng cáo", ChecklistItemName.AdPosition},
            {"Applovin IOS ID", ChecklistItemName.ApplovinIOSID},
            {"Applovin Android ID", ChecklistItemName.ApplovinAndroidID},
            {"Applovin IOS Inter ID", ChecklistItemName.ApplovinIOSInterID},
            {"Applovin IOS Banner ID", ChecklistItemName.ApplovinIOSBannerID},
            {"Applovin IOS Reward ID", ChecklistItemName.ApplovinIOSRewardID},
            {"Applovin Android Inter ID", ChecklistItemName.ApplovinAndroidInterID},
            {"Applovin Android Banner ID", ChecklistItemName.ApplovinAndroidBannerID},
            {"Applovin Android Reward ID", ChecklistItemName.ApplovinAndroidRewardID},
            {"Iron Source IOS ID", ChecklistItemName.IronSourceIOSID},
            {"Iron Source Android ID", ChecklistItemName.IronSourceAndroidID},
        };

        public static Dictionary<ChecklistItemName, string> dictionaryCheckListValue =
            new Dictionary<ChecklistItemName, string>();
        
        private void OnGUI()
        {
            m_GoogleSheetUrl = EditorGUILayout.TextField("Google Sheet Info URL", m_GoogleSheetUrl);
            // if (GUILayout.Button("Install Facebook SDK"))
            // {
            //     DownloadPackage(FacebookSdkCurrentVersionUrl);
            //     AddDefineSymbol(PluginDefineSymbol.PluginFacebook);
            // }
            // if (GUILayout.Button("Install Firebase SDK"))
            // {
            //     DownloadPackage(FirebaseSdkUrlByVersion); 
            //     AddDefineSymbol(PluginDefineSymbol.PluginFirebase);
            //     AddDefineSymbol(PluginDefineSymbol.PluginFirebaseAnalytic);
            // }
            // if (GUILayout.Button("Install Applovin SDK"))
            // {
            //     DownloadPackage(ApplovinSdkUrlByVersion);
            //     AddDefineSymbol(PluginDefineSymbol.PluginApplovin);
            // }
            // if (GUILayout.Button("Install Applovin SDK"))
            // {
            //     DownloadPackage(ApplovinSdkUrlByVersion); 
            // }
            // if (GUILayout.Button("Apply Setting"))
            // {
            //     OnClickApplySetting();
            // }
            if (GUILayout.Button("Set SDK Id From Project Checklist"))
            {
                OnClickSetSDKId();
            }

            if (m_SetSdkLog.Length > 0) m_SetSdkLog = EditorGUILayout.TextArea(m_SetSdkLog);
        }

        private void OnClickApplySetting()
        {
            Directory.CreateDirectory("Assets/Plugins/Android/");
            FileUtil.CopyFileOrDirectory("Assets/_IECModules/Template/mainTemplate.gradle.template", "Assets/Plugins/Android/mainTemplate.gradle");
        }
            
        private void OnClickSetSDKId()
        {
            m_SetSdkLog = "";

            var checklistData = DownloadTextFromUrl(GoogleSheetToCSV.ParseGoogleSheetUrl(m_GoogleSheetUrl));

            ApplySdkSetting(GoogleSheetToCSV.ParseCSV(checklistData, false));
        }

        private void DownloadPackage(string url, Action onComplete  = null)
        {
            AddLog($"Start Download SDK: {url}");

            HTTPManager.Setup();
            var httpRequest = new HTTPRequest(new Uri(url), (originalRequest, response) =>
            {
                EditorUtility.ClearProgressBar();
                onComplete?.Invoke();
                
                AddLog($"Download SDK Complete: {response.Data.Length} bytes");
                // using (var fs = File.Create("Assets/facebook.zip")) 
                // {
                //     fs.Write(response.Data, 0, response.Data.Length);
                // }
                
                // var byteArray = File.ReadAllBytes("Assets/facebook.zip");
                // AddLog($"Assets/facebook.zip: {byteArray.Length} bytes");

                if (url.EndsWith(".unitypackage"))
                {
                    ImportPackage(response.Data, url);
                }
                else
                {
                    ZipUtils.GetZipFileFromByteArray(response.Data, zipFile =>
                    {
                        foreach (ZipEntry entry in zipFile)
                        {
                            if(entry.IsDirectory) continue;
                            if(!entry.Name.EndsWith(".unitypackage")) continue;
                            AddLog(entry.Name);
                            var zippedStream = zipFile.GetInputStream(entry);
                            var fileByteArray = ZipUtils.ReadFully(zippedStream);
                    
                            Directory.CreateDirectory(TempIECDirectory);

                            var filePath = $"{TempIECDirectory}{entry.Name.Split('/').Last()}";
                        
                            AddLog($"Start Unzip To: {filePath}");

                            ImportPackage(fileByteArray, entry.Name);
                        
                            break;
                        }

                    });
                }
            }).Send();
            httpRequest.OnProgress += (request, downloaded, length) =>
            {
                EditorUtility.DisplayProgressBar("Downloading", $"{downloaded}/{length}", (float) downloaded/ length);
            }; 
        }

        private void ImportPackage(byte[] packageByteArray, string packageName)
        {
            var filePath = $"{TempIECDirectory}{packageName.Split('/').Last()}";
            using (var fs = File.Create(filePath)) 
            {
                fs.Write(packageByteArray, 0, packageByteArray.Length);
            }
                        
            AddLog($"Waiting Import Package");

            AssetDatabase.importPackageStarted += ImportPackageStart;
            AssetDatabase.importPackageCompleted += ImportPackageComplete;
            AssetDatabase.ImportPackage(filePath, false);

        }

        private void ImportPackageComplete(string packageName)
        {
            AddLog($"Import Package Done: {packageName}");
            AssetDatabase.importPackageCompleted -= ImportPackageComplete;
        }
        private void ImportPackageStart(string packageName)
        {
            AddLog($"Import Package Start: {packageName}");
            AssetDatabase.importPackageStarted -= ImportPackageStart;
        }
        private byte[] DownloadDataFromUrl(string url)
        {
            var uwr = DownloadFromUrl(url);
            var data = uwr.downloadHandler.data;
            uwr.Dispose();
            return data;
        }
        private string DownloadTextFromUrl(string url)
        {
            var uwr = DownloadFromUrl(url);
            var text = uwr.downloadHandler.text;
            uwr.Dispose();
            return text;
        }
        private UnityWebRequest DownloadFromUrl(string url)
        {
            var uwr = UnityWebRequest.Get(url);
            
            // uwr.SetRequestHeader("accept","text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9" ); 
            // // uwr.SetRequestHeader("accept-encoding", "gzip, deflate, br");
            // uwr.SetRequestHeader("accept-language", "ru-RU,ru;q=0.9,en-US;q=0.8,en;q=0.7");

            uwr.SendWebRequest();
            while (!uwr.isDone)
            {
                // Debug.Log(uwr.downloadProgress);
            }

            if (uwr.isNetworkError)
            {
                Debug.LogError("Download Error");
                AddLogError($"Downloading: {url}");
            }
            return uwr;
        }
        

        private void ApplySdkSetting(List<List<string>> sdkData)
        {

            dictionaryCheckListValue = sdkData.Where(checklistKey => _checklistKey.ContainsKey(checklistKey[0]))
                .ToDictionary(checklistKey => _checklistKey[checklistKey[0]], checklistKey => checklistKey[1]);


#if PluginFacebook
            ServicesConfig serviceConfig;
            try
            {
                var assets = AssetDatabase.FindAssets("t:ServicesConfig");
                var serviceConfigPath = AssetDatabase.GUIDToAssetPath(assets[0]);
                serviceConfig = AssetDatabase.LoadAssetAtPath<ServicesConfig>(serviceConfigPath);
                AddLog("Service Config Load Success: " + serviceConfigPath);
            }
            catch (Exception e)
            {
                AddLog("Error Loading Service Config: " + e.Message + "\n" + e.StackTrace);
                throw;
            }
#else
            AddLogError("PluginFacebook NOT Defined!!!!!");
#endif

            sdkData.ForEach(line =>
            {
                if (line[0] == "Tên game")
                {
                    AddLog($"Product Name: {line[1]}");
                    PlayerSettings.productName = line[1];
                }

                if (line[0] == "BundleID")
                {
                    var bundleId = line[1];
                    AddLog($"Bundle ID: {bundleId}");
                    PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Standalone, bundleId);
                    PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, bundleId);
                    PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, bundleId);

                    if (bundleId.ToLower().Contains(".gma.")) PlayerSettings.companyName = "GMA";
                    else if (bundleId.ToLower().Contains(".iec.")) PlayerSettings.companyName = "IEC";
                    else if (bundleId.ToLower().Contains(".dta.")) PlayerSettings.companyName = "DTA";
                    else
                    {
                        AddLog($"ERROR: Unknown Company Name");
                    }
                }

                if (line[0] == "Facebook ID")
                {
                    AddLog($"Facebook ID: {line[1]}");
#if PluginFacebook
                    if (Facebook.Unity.Settings.FacebookSettings.NullableInstance == null)
                    {
                        var instance = CreateInstance<Facebook.Unity.Settings.FacebookSettings>();
                        var path1 = Path.Combine(Application.dataPath, "FacebookSDK/SDK/Resources");
                        if (!Directory.Exists(path1))
                            Directory.CreateDirectory(path1);
                        var path2 = Path.Combine(Path.Combine("Assets", "FacebookSDK/SDK/Resources"), "FacebookSettings.asset");
                        AssetDatabase.CreateAsset(instance, path2);
                    }
                    Selection.activeObject = Facebook.Unity.Settings.FacebookSettings.Instance;
                    Facebook.Unity.Settings.FacebookSettings.AppIds = new List<string> {line[1]};
                    Facebook.Unity.Settings.FacebookSettings.AppLabels = new List<string> {PlayerSettings.productName};
                    Facebook.Unity.Editor.ManifestMod.GenerateManifest();
#endif
                }

#if PluginFacebook
                if (line[0] == "Kochava ID IOS")
                {
                    if(!string.IsNullOrEmpty(line[1])) AddLog($"Kochava ID IOS: {line[1]}");
                    serviceConfig.kochavaConfig.iosID = line[1];
                }
                if (line[0] == "Kochava ID Android")
                {
                    if(!string.IsNullOrEmpty(line[1])) AddLog($"Kochava ID Android: {line[1]}");
                    serviceConfig.kochavaConfig.androidID = line[1];
                }
                if (line[0] == "Appodeal ID IOS")
                {
                    if(!string.IsNullOrEmpty(line[1])) AddLog($"Appodeal ID IOS: {line[1]}");
                    serviceConfig.AppodealConfig.android.appID = line[1];
                }
                if (line[0] == "Appodeal ID Android")
                {
                    if(!string.IsNullOrEmpty(line[1])) AddLog($"Appodeal ID Android: {line[1]}");
                    serviceConfig.AppodealConfig.android.appID = line[1];
                }
                if (line[0] == "Applovin Android ID")
                {
                    if(!string.IsNullOrEmpty(line[1])) AddLog($"Applovin Android ID: {line[1]}");
                    serviceConfig.ApplovinConfig.android.appID = line[1];
                }
                if (line[0] == "Applovin IOS ID")
                {
                    if(!string.IsNullOrEmpty(line[1])) AddLog($"Applovin IOS ID: {line[1]}");
                    serviceConfig.ApplovinConfig.ios.appID = line[1];
                }
                if (line[0] == "Applovin Android Inter ID")
                {
                    if(!string.IsNullOrEmpty(line[1])) AddLog($"Applovin Android Inter ID: {line[1]}");
                    serviceConfig.ApplovinConfig.android.interstitialID = line[1];
                }
                if (line[0] == "Applovin Android Banner ID")
                {
                    if(!string.IsNullOrEmpty(line[1])) AddLog($"Applovin Android Banner ID: {line[1]}");
                    serviceConfig.ApplovinConfig.android.bannerID = line[1];
                }
                if (line[0] == "Applovin Android Reward ID")
                {
                    if(!string.IsNullOrEmpty(line[1])) AddLog($"Applovin Android Reward ID: {line[1]}");
                    serviceConfig.ApplovinConfig.android.rewardID = line[1];
                }
                if (line[0] == "Applovin IOS Inter ID")
                {
                    if(!string.IsNullOrEmpty(line[1])) AddLog($"Applovin IOS Inter ID: {line[1]}");
                    serviceConfig.ApplovinConfig.ios.interstitialID = line[1];
                }
                if (line[0] == "Applovin IOS Banner ID")
                {
                    if(!string.IsNullOrEmpty(line[1])) AddLog($"Applovin IOS Banner ID: {line[1]}");
                    serviceConfig.ApplovinConfig.ios.bannerID = line[1];
                }
                if (line[0] == "Applovin IOS Reward ID")
                {
                    if(!string.IsNullOrEmpty(line[1])) AddLog($"Applovin IOS Reward ID: {line[1]}");
                    serviceConfig.ApplovinConfig.ios.rewardID = line[1];
                }
                if (line[0] == "Iron Source IOS ID")
                {
                    if(!string.IsNullOrEmpty(line[1])) AddLog($"Iron Source IOS ID: {line[1]}");
                    serviceConfig.IronSourceConfig.ios.appID = line[1];
                    serviceConfig.IronSourceConfig.ios.interstitialID = line[1];
                    serviceConfig.IronSourceConfig.ios.bannerID = line[1];
                    serviceConfig.IronSourceConfig.ios.rewardID = line[1];
                }
                if (line[0] == "Iron Source Android ID")
                {
                    if(!string.IsNullOrEmpty(line[1])) AddLog($"Iron Source Android ID: {line[1]}");
                    serviceConfig.IronSourceConfig.android.appID = line[1];
                    serviceConfig.IronSourceConfig.android.interstitialID = line[1];
                    serviceConfig.IronSourceConfig.android.bannerID = line[1];
                    serviceConfig.IronSourceConfig.android.rewardID = line[1];
                }
#endif
            });


#if PluginFacebook
            EditorUtility.SetDirty(serviceConfig);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
#endif
        }

        private void AddLogError(string log)
        {
            AddLog($"--- ERROR: {log}");
        }

        private void AddLog(object log)
        {
            AddLog(log.ToString());
        }
        private void AddLog(string log)
        {
            if (m_SetSdkLog.Length != 0) m_SetSdkLog += "\n";
            m_SetSdkLog += $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {log}";
            
        }

        private static void AddDefineSymbol(PluginDefineSymbol defineSymbol)
        {
            ScriptingDefineSymbolEditor.AddDefineSymbol(defineSymbol.ToString());
        }
    }
}
