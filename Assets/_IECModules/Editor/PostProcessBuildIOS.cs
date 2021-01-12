using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif
using System.IO;
using System.Collections.Generic;

public class PostProcessBuildIOS : MonoBehaviour
{
#if UNITY_IOS
    [PostProcessBuild(int.MaxValue)]
    static void OnPostprocessBuild(BuildTarget buildTarget, string path)
    {
        // Read plist
        var plistPath = Path.Combine(path, "Info.plist");
        var plist = new PlistDocument();
        plist.ReadFromFile(plistPath);

        // Update value
        PlistElementDict rootDict = plist.root;
        rootDict.SetBoolean("ITSAppUsesNonExemptEncryption", false);
        rootDict.SetString("NSCalendarsUsageDescription", "advertisement");
        rootDict.SetString("NSCameraUsageDescription", "camera");
        rootDict.SetString("NSPhotoLibraryUsageDescription", "advertisement");
        rootDict.SetString("NSLocationWhenInUseUsageDescription", "advertisement");
        rootDict.values.Remove("UIApplicationExitsOnSuspend");

        var appTransportDict = rootDict.CreateDict("NSAppTransportSecurity");
        appTransportDict.SetBoolean("NSAllowsArbitraryLoads", true);
        appTransportDict.SetBoolean("NSAllowsArbitraryLoadsInWebContent", false);
        appTransportDict.values.Remove("NSAllowsArbitraryLoadsInWebContent");

        // Write plist
        File.WriteAllText(plistPath, plist.WriteToString());

        var projectPath = path + "/Unity-iPhone.xcodeproj/project.pbxproj";

        EditPBX();
        // EditCapability();

        void EditCapability()
        {
            PBXProject pbxProject = new PBXProject();
            pbxProject.ReadFromFile(projectPath);
            var mainTargetGuid = pbxProject.GetUnityMainTargetGuid();
            var entitlementsFileName = pbxProject.GetBuildPropertyForAnyConfig(mainTargetGuid, "CODE_SIGN_ENTITLEMENTS") ?? Application.productName + ".entitlements";
            ProjectCapabilityManager projectCapabilityManager = new ProjectCapabilityManager(projectPath,
              entitlementsFileName, "Unity-iPhone", mainTargetGuid);
            projectCapabilityManager.AddiCloud(false, true, true, false, new string[] { $"iCloud.{Application.identifier}.cloud.save" });
            projectCapabilityManager.WriteToFile();

            var entitleRead = File.ReadAllText(path + "/" + entitlementsFileName);
            entitleRead = entitleRead.Replace("<string>iCloud.$(CFBundleIdentifier)</string>", "");
            Debug.LogWarning(entitleRead);
            File.WriteAllText(path + "/" + entitlementsFileName, entitleRead);
        }

        void EditPBX()
        {
            PBXProject pbxProject = new PBXProject();
            pbxProject.ReadFromFile(projectPath);
            var mainTargetGuid = pbxProject.GetUnityMainTargetGuid();
            pbxProject.SetBuildProperty(mainTargetGuid, "ENABLE_BITCODE", "NO");
            pbxProject.WriteToFile(projectPath);
        }
    }
#endif
}