using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif
using System.IO;
using System.Collections.Generic;
using System;

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

        EditGoogleInfoPlistAddTargetMembership();
        CopyFastlaneConfig();
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
            projectCapabilityManager.AddiCloud(true, true, true, false, new string[] { $"iCloud.{Application.identifier}.cloud.save" });
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

#if PluginByteDanceChinaIOS
            var debugRangerPath = path + "/Pods/Target Support Files/RangersAppLog/RangersAppLog.debug.xcconfig";
            var releaseRangerPath = path + "/Pods/Target Support Files/RangersAppLog/RangersAppLog.release.xcconfig";
            var debugRangerContent = File.ReadAllText(debugRangerPath);
            var releaseRangerContent = File.ReadAllText(releaseRangerPath);
            debugRangerContent += "ENABLE_BITCODE = NO\n";
            releaseRangerContent += "ENABLE_BITCODE = NO\n";
            File.WriteAllText(debugRangerPath, debugRangerContent);
            File.WriteAllText(releaseRangerPath, releaseRangerContent);

            pbxProject.AddFrameworkToProject(mainTargetGuid, "libz.tbd", false);
            pbxProject.AddFrameworkToProject(mainTargetGuid, "libsqlite3.tbd", false);
            pbxProject.AddFrameworkToProject(mainTargetGuid, "Foundation.framework", false);
            pbxProject.AddFrameworkToProject(mainTargetGuid, "UIKit.framework", false);
            pbxProject.AddFrameworkToProject(mainTargetGuid, "JavaScriptCore.framework", false);
            pbxProject.AddFrameworkToProject(mainTargetGuid, "WebKit.framework", false);
            pbxProject.AddFrameworkToProject(mainTargetGuid, "CoreFoundation.framework", false);
            pbxProject.AddFrameworkToProject(mainTargetGuid, "CoreTelephony.framework", false);
            pbxProject.AddFrameworkToProject(mainTargetGuid, "Security.framework", false);
            pbxProject.AddFrameworkToProject(mainTargetGuid, "SystemConfiguration.framework", false);
            pbxProject.AddFrameworkToProject(mainTargetGuid, "AdSupport.framework", false);
#endif
            pbxProject.WriteToFile(projectPath);
        }

        void CopyFastlaneConfig()
        {
            var fastlaneFolderPath = (Application.dataPath + "/_IEC/Editor/fastlane");
            if (Directory.Exists(fastlaneFolderPath))
            {
                Debug.Log("Fastlane config folder found");
                FileUtil.CopyFileOrDirectory(fastlaneFolderPath, Path.Combine(path, "fastlane"));
            }
        }

        void EditGoogleInfoPlistAddTargetMembership()
        {
            var googleServiceFiles = path + "/GoogleService-Info.plist";

            PBXProject pbxProject = new PBXProject();
            pbxProject.ReadFromFile(projectPath);

            if (File.Exists(googleServiceFiles))
            {
                var pbxProjectStr = File.ReadAllText(projectPath);

                // get first Resources only
                var stringPBXResourcesBuildPhase = pbxProjectStr.GetBetweenString("/* Begin PBXResourcesBuildPhase section */", "}");
                if (!stringPBXResourcesBuildPhase.Contains("GoogleService-Info.plist in Resources"))
                {
                    var googlePlistFileGuid = pbxProject.FindFileGuidByProjectPath("GoogleService-Info.plist");

                    var randomGenerateGuid = Guid.NewGuid().ToString().Replace("-", "").ToUpper().Substring(0, 24);

                    var newStringPBXResourcesBuildPhase = stringPBXResourcesBuildPhase.Replace("/* Data in Resources */,",
                        $"/* Data in Resources */,\n\t\t\t\t{randomGenerateGuid} /* GoogleService-Info.plist in Resources */,");

                    pbxProjectStr = pbxProjectStr.Replace(stringPBXResourcesBuildPhase, newStringPBXResourcesBuildPhase);
                    pbxProjectStr = pbxProjectStr.Replace(" /* Il2CppGenericClassTable.c */; };",
                        $" /* Il2CppGenericClassTable.c */; }};\n\t\t{randomGenerateGuid} /* GoogleService-Info.plist in Resources */ = {{isa = PBXBuildFile; fileRef = {googlePlistFileGuid} /* GoogleService-Info.plist */; }};");
                    File.WriteAllText(projectPath, pbxProjectStr);

                    Debug.Log("GoogleService-Info.plist Target Memeber ship enabled");
                }

            }
        }
    }
#endif
}