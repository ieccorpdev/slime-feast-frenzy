using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

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

        private string m_GoogleSheetUrl =
            "https://docs.google.com/spreadsheets/d/1HhJ0oyLWfUawEx0MiMc07q7J7ViLn3a-IJIDML0EWpw/edit#gid=927018210";

        private string m_SetSdkLog = "";

        void OnGUI()
        {
            m_GoogleSheetUrl = EditorGUILayout.TextField("Google Sheet Info URL", m_GoogleSheetUrl);
            if (GUILayout.Button("Set SDK"))
            {
                var url = GoogleSheetToCSV.ParseGoogleSheetUrl(m_GoogleSheetUrl);
                var uwr = UnityWebRequest.Get(url);
                uwr.SendWebRequest();
                while (!uwr.isDone)
                {
                }

                if (uwr.isNetworkError)
                {
                    Debug.Log("Error");
                }

                ApplySdkSetting(GoogleSheetToCSV.ParseCSV(uwr.downloadHandler.text, false));
                uwr.Dispose();
            }

            if(m_SetSdkLog.Length > 0) m_SetSdkLog = EditorGUILayout.TextArea(m_SetSdkLog);
        }

        void ApplySdkSetting(List<List<string>> sdkData)
        {
            m_SetSdkLog = "";
            
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
            AddLog("ERROR: PluginFacebook NOT Defined!!!!!");
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
                    
                    if (bundleId.Contains("GMA")) PlayerSettings.companyName = "GMA";
                    else if (bundleId.Contains("IEC")) PlayerSettings.companyName = "IEC";
                    else if (bundleId.Contains("DTA")) PlayerSettings.companyName = "DTA";
                    else
                    {
                        AddLog($"ERROR: Unknown Company Name");
                    }
                }

                if (line[0] == "Facebook ID")
                {
                    AddLog($"Facebook ID: {line[1]}");
#if PluginFacebook
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

        void AddLog(string log)
        {
            if (m_SetSdkLog.Length != 0) m_SetSdkLog += "\n";
            m_SetSdkLog += log;
        }
    }
}