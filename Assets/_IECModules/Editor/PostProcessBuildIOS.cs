using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif
using System.IO;
using System.Collections.Generic;
using System;
using System.Linq;
using BestHTTP;
using SimpleJSON;

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
        var podProjPath = path + "/Pods/Pods.xcodeproj/project.pbxproj";
        
        //CheckAddSmaatoSKAdNetworkID(path);
        //CheckAddInmobiSKAdNetworkID(path);
        EditGoogleInfoPlistAddTargetMembership();
        CopyFastlaneConfig();
        EditPBX();
        SigningPod();
        EditCloudSaveCapability();

        void EditCloudSaveCapability()
        {
            if(!Directory.Exists(Application.dataPath + "/Extensions/CloudOnce")) return;
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
            pbxProject.SetBuildProperty(pbxProject.ProjectGuid(), "ENABLE_BITCODE", "NO");
            var mainTargetGuid = pbxProject.GetUnityMainTargetGuid();
            pbxProject.SetBuildProperty(mainTargetGuid, "ENABLE_BITCODE", "NO");
            pbxProject.WriteToFile(projectPath);
        }

        void SigningPod()
        {
            var appFilePath = path + "/fastlane/Appfile";
            var appFileContent = File.ReadAllText(appFilePath);
            var teamId = appFileContent.Split(new[] {"team_id"}, StringSplitOptions.None)[2]
                .GetBetweenString("(\"", "\")",false);
            Debug.Log("teamId:" + teamId);
            var podPbx = new PBXProject();
            podPbx.ReadFromFile(podProjPath);
            var id = podPbx.TargetGuidByName("FBSDKCoreKit-FacebookSDKStrings");
            Debug.Log(id);
            podPbx.SetTeamId(id, teamId);
            podPbx.WriteToFile(podProjPath);
        }

        void CopyFastlaneConfig()
        {
            var fastlaneAppFilePath = (Application.dataPath + "/_IEC/Editor/fastlane/Appfile");
            var fastlaneModuleFolderPath = (Application.dataPath + "/_IECModules/Editor/fastlane");
            if (File.Exists(fastlaneAppFilePath))
            {
                Debug.Log("Fastlane config folder found");
                FileUtil.ReplaceDirectory(fastlaneModuleFolderPath, Path.Combine(path, "fastlane"));
                FileUtil.ReplaceFile(fastlaneAppFilePath, Path.Combine(path, "fastlane/Appfile"));
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
    public static void CheckAddSmaatoSKAdNetworkID(string path = "")
    {
#if PluginApplovin
        if(!File.Exists(Application.dataPath + "/MaxSdk/Mediation/Smaato/Editor/Dependencies.xml")) return;
        const string smaatoGuideUrl = "https://developers.smaato.com/publishers/nextgen-sdk-ios-integration/#skadnetwork-setup-ios";
        new HTTPRequest(new Uri(smaatoGuideUrl), (originalRequest, response) =>
        {
            var smaatoSKAdNetworkItems = response.DataAsText.GetBetweenString("SKAdNetworkItems", "&lt;/array&gt;")
                .Replace("&lt;", "<").Replace("&gt;", ">")
                .Split(new[] {"<string>"}, StringSplitOptions.None)
                .ToList();
            smaatoSKAdNetworkItems.RemoveAt(0);
            smaatoSKAdNetworkItems = smaatoSKAdNetworkItems.Select(x => x.Substring(0, 10)).ToList();

            AddMissingSKAdNetworkID(smaatoSKAdNetworkItems, path);
        }).Send();

#endif
    }
    public static void CheckAddInmobiSKAdNetworkID(string path = "")
    {
#if PluginApplovin
        if(!File.Exists(Application.dataPath + "/MaxSdk/Mediation/InMobi/Editor/Dependencies.xml")) return;
        const string inmobiSkAdNetworkJsonUrl = "https://www.inmobi.com/skadnetworkids.json";
        new HTTPRequest(new Uri(inmobiSkAdNetworkJsonUrl), (originalRequest, response) =>
        {
            var inmobiSKAdNetworkJsonItems = JSON.Parse(response.DataAsText)["skadnetwork_ids"].AsArray;
            var inmobiSKAdNetworkItems = new List<string>();
            for (var i = 0; i < inmobiSKAdNetworkJsonItems.Count; i++)
            {
                inmobiSKAdNetworkItems.Add(inmobiSKAdNetworkJsonItems[i]["skadnetwork_id"].ToString().Substring(2, 10));
            }
                
            AddMissingSKAdNetworkID(inmobiSKAdNetworkItems, path);
        }).Send();

#endif
    }

    public static void AddMissingSKAdNetworkID(List<string> missingSkAdNetworkItems, string path = "")
    {
        var plistPath = Path.Combine(path, "Info.plist");
        var plist = new PlistDocument();
        plist.ReadFromFile(plistPath);

        var rootDict = plist.root;
        var plistSKAdNetworkItemsArray = rootDict.values["SKAdNetworkItems"].AsArray();

        var missingSKAdId = missingSkAdNetworkItems.Where(smaatoId =>
                !plistSKAdNetworkItemsArray.values.Any(x => x.AsDict().values["SKAdNetworkIdentifier"].AsString().Contains(smaatoId)))
            .ToList();
            
        if(missingSKAdId.Count == 0) return;
        Debug.LogWarning($"{missingSKAdId.Count} SKAdnetworkID Missing: {missingSKAdId.JsonSerialize()}");
            
        missingSKAdId.ForEach(missingId => plistSKAdNetworkItemsArray.AddDict().SetString("SKAdNetworkIdentifier", missingId));
        File.WriteAllText(plistPath, plist.WriteToString());
    }
#endif
}