using System.Collections.Generic;
using System.IO;
using _IECModules;
using NUnit.Framework;
using SimpleJSON;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class PreProcessBuild : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;

    private static readonly List<string> ListPermissionRemove = new List<string>()
    {
        "WRITE_EXTERNAL_STORAGE"
    };
    public void OnPreprocessBuild(BuildReport report)
    {
        SetFirebaseVersion();
        RemovePermissions(ListPermissionRemove);
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

    private void RemovePermissions(List<string> permissions)
    {
        var manifestPath = "Assets/Plugins/Android/AndroidManifest.xml";
        if (!File.Exists(manifestPath))
        {
            return;
        }

        var manifestContent = File.ReadAllText(manifestPath);
        var manifestTag = manifestContent.GetBetweenString("<manifest", "</manifest>",false);
        // Debug.LogError(manifestTag);
        var manifestTagReplace = manifestTag;
        for (int i = 0; i < permissions.Count; i++)
        {
            var permission = permissions[i];
            var permissionRemove =
                $"<uses-permission android:name=\"android.permission.{permission}\" tools:node=\"remove\"/>";
            if (!manifestTagReplace.Contains(permissionRemove))
            {
                manifestTagReplace += $"{permissionRemove}\n";
            }
        }
        manifestContent = manifestContent.Replace(manifestTag, manifestTagReplace);
        File.WriteAllText(manifestPath,manifestContent);
    }
}