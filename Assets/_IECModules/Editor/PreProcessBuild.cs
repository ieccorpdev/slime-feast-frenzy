using System;
using System.Collections.Generic;
using System.IO;
using _IECModules;
using SimpleJSON;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class PreProcessBuild : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;
    private const int ANDROID_TARGET_SDK = 31;

    public void OnPreprocessBuild(BuildReport report)
    {
        ProjectChecklistHelper.LoadServicesConfig();
        ModuleSdkManager.SetSDKId();
        ProjectResolveHelper.ResolveProject();
        SetFirebaseVersion();
        SetPlatformVersion();
        UpdateVersionCodeFromEnvironment();
        SetTargetSdkAndroid();
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
}