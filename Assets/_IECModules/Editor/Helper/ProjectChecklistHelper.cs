using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace _IECModules
{
    public static class ProjectChecklistHelper
    {
        public static ServicesConfig servicesConfig;

        public enum ChecklistItemName
        {
            ProductName,
            BundleID,
            KeyStoreName,
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
            {"Tên game", ChecklistItemName.ProductName},
            {"BundleID", ChecklistItemName.BundleID},
            {"Keystore Name", ChecklistItemName.KeyStoreName},
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

        public static string LoadServicesConfig()
        {
            try
            {
                var assets = AssetDatabase.FindAssets("t:ServicesConfig");
                var servicesConfigPath = AssetDatabase.GUIDToAssetPath(assets[0]);
                servicesConfig = AssetDatabase.LoadAssetAtPath<ServicesConfig>(servicesConfigPath);
                return servicesConfigPath;
            }
            catch (Exception e)
            {
                Debug.LogError($"{e.Message}\n{e.StackTrace}");
                return e.StackTrace;
            }
        }

        public static void ApplySdkSetting(string googleDocsUrl)
        {
            var checklistData = UnityWebRequestDownloadHelper.DownloadTextFromUrl(GoogleSheetToCSV.ParseGoogleSheetUrl(googleDocsUrl));
            var googleDocsData = GoogleSheetToCSV.ParseCSV(checklistData, false);
            
            var getServicesConfigResult = LoadServicesConfig();
            
            if (servicesConfig != null)
            {
                LogEditorHelper.AddLog("Service Config Load Success: " + getServicesConfigResult);
            }
            else
            {
                LogEditorHelper.AddLogError($"Error Loading Service Config!: \n{getServicesConfigResult}");
                return;
            }
            
            servicesConfig.projectChecklistUrl = googleDocsUrl;
            
            ApplySdkSetting(googleDocsData);
        }
        
        private static void ApplySdkSetting(List<List<string>> sdkData)
        {

            dictionaryCheckListValue = sdkData.Where(checklistKey => _checklistKey.ContainsKey(checklistKey[0]))
                .ToDictionary(checklistKey => _checklistKey[checklistKey[0]], checklistKey => checklistKey[1]);

#if !PluginFacebook
            LogEditorHelper.AddLogError("PluginFacebook NOT Defined!!!!!");
#endif
            PlayerSettings.Android.targetSdkVersion = (AndroidSdkVersions) 29;

            foreach (var keyValuePair in dictionaryCheckListValue)
            {
                var checklistItemName = keyValuePair.Key;
                var checklistItemValue = keyValuePair.Value;
                switch (checklistItemName)
                {
                    case ChecklistItemName.ProductName:
                        var productName = checklistItemValue;
                        LogEditorHelper.AddLog($"Product Name: {productName}");
                        PlayerSettings.productName = productName;
                        break;
                    case ChecklistItemName.BundleID:
                        var bundleId = checklistItemValue;
                        LogEditorHelper.AddLog($"Bundle ID: {bundleId}");
                        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Standalone, bundleId);
                        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, bundleId);
                        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, bundleId);

                        if (bundleId.ToLower().Contains(".gma.")) PlayerSettings.companyName = "GMA";
                        else if (bundleId.ToLower().Contains(".iec.")) PlayerSettings.companyName = "IEC";
                        else if (bundleId.ToLower().Contains(".dta.")) PlayerSettings.companyName = "DTA";
                        else
                        {
                            LogEditorHelper.AddLogError("Unknown Company Name");
                        }
                        break;
                    case ChecklistItemName.KeyStoreName:
                        if(!string.IsNullOrEmpty(checklistItemValue)) KeystoreHelper.CreateKeyStore(checklistItemValue);
                        break;
                    case ChecklistItemName.KanboardSwimlane:
                        break;
                    case ChecklistItemName.DiscordChannel:
                        break;
                    case ChecklistItemName.FirebaseProject:
                        break;
                    case ChecklistItemName.UnityCloudProject:
                        break;
                    case ChecklistItemName.UnityCloudBuildKanboardWebhooks:
                        break;
                    case ChecklistItemName.UnityCloudBuildDiscordWebhooks:
                        break;
                    case ChecklistItemName.GitlabProject:
                        break;
                    case ChecklistItemName.GitlabIntegration:
                        break;
                    case ChecklistItemName.UnityCloudBuildTargetConfig:
                        break;
                    case ChecklistItemName.TrackingEventList:
                        break;
#if PluginFacebook
                    case ChecklistItemName.FacebookID:
                        var facebookId = checklistItemValue;
                        LogEditorHelper.AddLog($"Facebook ID: {facebookId}");
                        if (Facebook.Unity.Settings.FacebookSettings.NullableInstance == null)
                        {
                            var instance = EditorWindow.CreateInstance<Facebook.Unity.Settings.FacebookSettings>();
                            var path1 = Path.Combine(Application.dataPath, "FacebookSDK/SDK/Resources");
                            if (!Directory.Exists(path1))
                                Directory.CreateDirectory(path1);
                            var path2 = Path.Combine(Path.Combine("Assets", "FacebookSDK/SDK/Resources"), "FacebookSettings.asset");
                            AssetDatabase.CreateAsset(instance, path2);
                        }
                        Selection.activeObject = Facebook.Unity.Settings.FacebookSettings.Instance;
                        Facebook.Unity.Settings.FacebookSettings.AppIds = new List<string> {facebookId};
                        Facebook.Unity.Settings.FacebookSettings.AppLabels = new List<string> {PlayerSettings.productName};
                        EditorUtility.SetDirty(Facebook.Unity.Settings.FacebookSettings.Instance);
                        Facebook.Unity.Editor.ManifestMod.GenerateManifest();
                        break;
                    case ChecklistItemName.FirebaseConfigFileIOS:
                        break;
                    case ChecklistItemName.FirebaseConfigFileAndroid:
                        break;
                    case ChecklistItemName.KochavaIDIOS:
                        if(!string.IsNullOrEmpty(checklistItemValue)) LogEditorHelper.AddLog($"Kochava ID IOS: {checklistItemValue}");
                        servicesConfig.kochavaConfig.iosID = checklistItemValue;
                        break;
                    case ChecklistItemName.KochavaIDAndroid:
                        if(!string.IsNullOrEmpty(checklistItemValue)) LogEditorHelper.AddLog($"Kochava ID Android: {checklistItemValue}");
                        servicesConfig.kochavaConfig.androidID = checklistItemValue;
                        break;
                    case ChecklistItemName.AppodealIDIOS:
                        if(!string.IsNullOrEmpty(checklistItemValue)) LogEditorHelper.AddLog($"Appodeal ID IOS: {checklistItemValue}");
                        servicesConfig.AppodealConfig.android.appID = checklistItemValue;
                        break;
                    case ChecklistItemName.AppodealIDAndroid:
                        if(!string.IsNullOrEmpty(checklistItemValue)) LogEditorHelper.AddLog($"Appodeal ID Android: {checklistItemValue}");
                        servicesConfig.AppodealConfig.android.appID = checklistItemValue;
                        break;
                    case ChecklistItemName.AdmobIDIOS:
                        break;
                    case ChecklistItemName.AdmobIDAndroid:
                        break;
                    case ChecklistItemName.MarketingChecklist:
                        break;
                    case ChecklistItemName.IAPChecklist:
                        break;
                    case ChecklistItemName.IAPIOSList:
                        break;
                    case ChecklistItemName.IAPAndroidList:
                        break;
                    case ChecklistItemName.AdPosition:
                        break;
                    case ChecklistItemName.ApplovinIOSID:
                        if(!string.IsNullOrEmpty(checklistItemValue)) LogEditorHelper.AddLog($"Applovin IOS ID: {checklistItemValue}");
                        servicesConfig.ApplovinConfig.ios.appID = checklistItemValue;
                        break;
                    case ChecklistItemName.ApplovinAndroidID:
                        if(!string.IsNullOrEmpty(checklistItemValue)) LogEditorHelper.AddLog($"Applovin Android ID: {checklistItemValue}");
                        servicesConfig.ApplovinConfig.android.appID = checklistItemValue;
                        break;
                    case ChecklistItemName.ApplovinIOSInterID:
                        if(!string.IsNullOrEmpty(checklistItemValue)) LogEditorHelper.AddLog($"Applovin IOS Inter ID: {checklistItemValue}");
                        servicesConfig.ApplovinConfig.ios.interstitialID = checklistItemValue;
                        break;
                    case ChecklistItemName.ApplovinIOSBannerID:
                        if(!string.IsNullOrEmpty(checklistItemValue)) LogEditorHelper.AddLog($"Applovin IOS Banner ID: {checklistItemValue}");
                        servicesConfig.ApplovinConfig.ios.bannerID = checklistItemValue;
                        break;
                    case ChecklistItemName.ApplovinIOSRewardID:
                        if(!string.IsNullOrEmpty(checklistItemValue)) LogEditorHelper.AddLog($"Applovin IOS Reward ID: {checklistItemValue}");
                        servicesConfig.ApplovinConfig.ios.rewardID = checklistItemValue;
                        break;
                    case ChecklistItemName.ApplovinAndroidInterID:
                        if(!string.IsNullOrEmpty(checklistItemValue)) LogEditorHelper.AddLog($"Applovin Android Inter ID: {checklistItemValue}");
                        servicesConfig.ApplovinConfig.android.interstitialID = checklistItemValue;
                        break;
                    case ChecklistItemName.ApplovinAndroidBannerID:
                        if(!string.IsNullOrEmpty(checklistItemValue)) LogEditorHelper.AddLog($"Applovin Android Banner ID: {checklistItemValue}");
                        servicesConfig.ApplovinConfig.android.bannerID = checklistItemValue;
                        break;
                    case ChecklistItemName.ApplovinAndroidRewardID:
                        if(!string.IsNullOrEmpty(checklistItemValue)) LogEditorHelper.AddLog($"Applovin Android Reward ID: {checklistItemValue}");
                        servicesConfig.ApplovinConfig.android.rewardID = checklistItemValue;
                        break;
                    case ChecklistItemName.IronSourceIOSID:
                        if(!string.IsNullOrEmpty(checklistItemValue)) LogEditorHelper.AddLog($"Iron Source IOS ID: {checklistItemValue}");
                        servicesConfig.IronSourceConfig.ios.appID = checklistItemValue;
                        servicesConfig.IronSourceConfig.ios.interstitialID = checklistItemValue;
                        servicesConfig.IronSourceConfig.ios.bannerID = checklistItemValue;
                        servicesConfig.IronSourceConfig.ios.rewardID = checklistItemValue;
                        break;
                    case ChecklistItemName.IronSourceAndroidID:
                        if(!string.IsNullOrEmpty(checklistItemValue)) LogEditorHelper.AddLog($"Iron Source Android ID: {checklistItemValue}");
                        servicesConfig.IronSourceConfig.android.appID = checklistItemValue;
                        servicesConfig.IronSourceConfig.android.interstitialID = checklistItemValue;
                        servicesConfig.IronSourceConfig.android.bannerID = checklistItemValue;
                        servicesConfig.IronSourceConfig.android.rewardID = checklistItemValue;
                        break;
#endif
                }
            }
            
            EditorUtility.SetDirty(servicesConfig);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}