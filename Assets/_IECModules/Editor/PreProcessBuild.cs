using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using _IECModules;
using SimpleJSON;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class PreProcessBuild : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;
    private const int ANDROID_TARGET_SDK = 33;

    public void OnPreprocessBuild(BuildReport report)
    {
        SetFirebaseVersion();
        SetPlatformVersion();
        UpdateVersionCodeFromEnvironment();
        SetTargetSdkAndroid();
        ProjectResolveHelper.UpdateApplovinQualityService();
        AddPermissionManifest();
        AddStartIOVersion();
    }

    private void SetTargetSdkAndroid()
    {
#if UNITY_ANDROID
        PlayerSettings.Android.targetSdkVersion = (AndroidSdkVersions)ANDROID_TARGET_SDK;
#endif
    }

    private void SetPlatformVersion()
    {
        ProjectChecklistHelper.LoadServicesConfig();
        if (ProjectChecklistHelper.servicesConfig.isVersionByPlatform)
        {
            var platform = EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS ? "ios" : "android";
            var platformVersion = ProjectChecklistHelper.servicesConfig.GetPlatformVersion(platform);
            PlayerSettings.bundleVersion = platformVersion.version;
        }
    }

    private void UpdateVersionCodeFromEnvironment()
    {
        var versionCodePrefixStr = Environment.GetEnvironmentVariable("VERSION_CODE_OFFSET");
        if(string.IsNullOrWhiteSpace(versionCodePrefixStr)) return;
        
        var versionCodePrefix = int.Parse(versionCodePrefixStr);
        var buildNumber = PlayerSettings.Android.bundleVersionCode;
        buildNumber += versionCodePrefix;
        
        Debug.LogWarning($"Setting build number (with offset) to: {buildNumber}");
        PlayerSettings.Android.bundleVersionCode = buildNumber;
        PlayerSettings.iOS.buildNumber = buildNumber.ToString();
    }

    private void SetFirebaseVersion()
    {
#if PluginFirebaseAnalytic
        ProjectChecklistHelper.LoadServicesConfig();
        var manifestJson = JSON.Parse(File.ReadAllText("Packages/manifest.json"));
        ProjectChecklistHelper.servicesConfig.firebaseSDKVersion = manifestJson["dependencies"]["com.google.firebase.analytics"].ToString();
#if PluginFirebaseRemote
        ProjectChecklistHelper.servicesConfig.firebaseRemoteSDKVersion = manifestJson["dependencies"]["com.google.firebase.remote-config"].ToString();
#endif
#endif
    }

    private void AddPermissionManifest()
    {
        var androidManifestPath = "Assets/Plugins/Android/AndroidManifest.xml";
        if(!File.Exists(androidManifestPath)) return;
        var androidManifestContent = File.ReadAllText("Assets/Plugins/Android/AndroidManifest.xml");
        if (!androidManifestContent.Contains("unityplayer.SkipPermissionsDialog"))
        {
            var applicationTag = androidManifestContent.GetBetweenString("<application", "</application>", false);
            var applicationTagReplace = applicationTag +"\t" + "<meta-data android:name=\"unityplayer.SkipPermissionsDialog\" android:value=\"true\"/>" + "\n\t";
            androidManifestContent = androidManifestContent.Replace(applicationTag, applicationTagReplace);

        }

        if (!androidManifestContent.Contains("android.permission.POST_NOTIFICATIONS"))
        {
            // <uses-permission android:name="android.permission.POST_NOTIFICATIONS"/>
            var permisionNoti = "\t" + "<uses-permission android:name=\"android.permission.POST_NOTIFICATIONS\" />" + "\n\t";
        
            var manifestTag = androidManifestContent.GetBetweenString("<manifest", "</manifest", false);
            var manifestTagReplace = manifestTag + permisionNoti;
            androidManifestContent =  androidManifestContent.Replace(manifestTag, manifestTagReplace); 
        }
       
        
        File.WriteAllText(androidManifestPath, androidManifestContent);
    }
    
    private void AddStartIOVersion()
    {
        // Path to the config asset
        string assetPath = "Assets/Resources/StartIOConfig.asset";

        // Check if the Resources folder exists, if not, create it
        if (!AssetDatabase.IsValidFolder("Assets/Resources"))
        {
            AssetDatabase.CreateFolder("Assets", "Resources");
        }

        // Load config
        StartIOConfig startIOConfig = AssetDatabase.LoadAssetAtPath<StartIOConfig>(assetPath);

        // If config doesn't exist, create a new one
        if (startIOConfig == null)
        {
            startIOConfig = ScriptableObject.CreateInstance<StartIOConfig>();
            AssetDatabase.CreateAsset(startIOConfig, assetPath);
            AssetDatabase.SaveAssets();
        }
        
        // Check if AdSdk script is exist in project
        var adSdkPath = "Assets/Plugins/API/AdSdk.cs";
        if (!File.Exists(adSdkPath))
        {
            startIOConfig.isHaveStartIO = false;
        }
        else
        {
            startIOConfig.isHaveStartIO = true;
            
            // Read all lines from the file
            string[] lines = File.ReadAllLines(adSdkPath);

            // Find the line that contains "WrapperVersion"
            string line = lines.FirstOrDefault(l => l.Contains("WrapperVersion"));

            if (line != null)
            {
                // Split the line by '=' and then by ';'
                string[] parts = line.Split(new[] { '=', ';' }, StringSplitOptions.RemoveEmptyEntries);

                // The second part should be the value of WrapperVersion, trimmed of any whitespace
                if (parts.Length >= 2)
                {
                    startIOConfig.startIOVersion = parts[1].Trim();
                }
            }
        }

        // Modify the ScriptableObject
        EditorUtility.SetDirty(startIOConfig);
        AssetDatabase.SaveAssets();
    }
}