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

    public void OnPreprocessBuild(BuildReport report)
    {
        SetFirebaseVersion();
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