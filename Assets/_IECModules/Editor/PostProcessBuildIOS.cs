using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
#if UNITY_IOS
using _IECModules;
using IEC;
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


        PBXProject pbxProject = new PBXProject();
        pbxProject.ReadFromFile(projectPath);

        //CheckAddSmaatoSKAdNetworkID(path);
        //CheckAddInmobiSKAdNetworkID(path);
#if PluginMaxAmazonNetwork
        CheckMissingSKAdId(path);
#endif
        EditGoogleInfoPlistAddTargetMembership();
        CopyFastlaneConfig();
        EditPBX();
        SigningPod();
        EditCapability();

        void EditCapability()
        {
            if (!Directory.Exists(Application.dataPath + "/Extensions/CloudOnce")) return;
            var mainTargetGuid = pbxProject.GetUnityMainTargetGuid();
            var entitlementsFileName =
                pbxProject.GetBuildPropertyForAnyConfig(mainTargetGuid, "CODE_SIGN_ENTITLEMENTS") ??
                Application.productName + ".entitlements";
            ProjectCapabilityManager projectCapabilityManager = new ProjectCapabilityManager(projectPath,
                entitlementsFileName, "Unity-iPhone", mainTargetGuid);
            projectCapabilityManager.AddiCloud(true, true, true, false,
                new string[] { $"iCloud.{Application.identifier}.cloud.save" });
            projectCapabilityManager.WriteToFile();

            var entitleRead = File.ReadAllText(path + "/" + entitlementsFileName);
            entitleRead = entitleRead.Replace("<string>iCloud.$(CFBundleIdentifier)</string>", "");

            ProjectChecklistHelper.LoadServicesConfig();
            if (ProjectChecklistHelper.servicesConfig != null &&
                !string.IsNullOrWhiteSpace(ProjectChecklistHelper.servicesConfig.teamIdCloudSave))
            {
                Debug.LogError("Change Team ID");
                entitleRead = entitleRead.Replace(
                    "<string>$(TeamIdentifierPrefix)$(CFBundleIdentifier)</string>",
                    $"<string>{ProjectChecklistHelper.servicesConfig.teamIdCloudSave}.$(CFBundleIdentifier)</string>");
            }

            var entitlements = new PlistDocument();
            entitlements.ReadFromString(entitleRead);
            var entitlementsDict = entitlements.root;

#if PluginGameCenter
            entitlementsDict.SetBoolean("com.apple.developer.game-center", true);
#endif
            Debug.LogWarning(entitleRead);
            File.WriteAllText(path + "/" + entitlementsFileName, entitlements.WriteToString());
        }

        void EditPBX()
        {
            pbxProject.SetBuildProperty(pbxProject.ProjectGuid(), "ENABLE_BITCODE", "NO");
            var mainTargetGuid = pbxProject.GetUnityMainTargetGuid();
            pbxProject.SetBuildProperty(mainTargetGuid, "ENABLE_BITCODE", "NO");

            //Fix upload ipa xcode 14
            pbxProject.SetBuildProperty(pbxProject.GetUnityFrameworkTargetGuid(),
                "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "NO");
            //Fix upload ipa xcode 15
            pbxProject.SetBuildProperty(pbxProject.GetUnityFrameworkTargetGuid(), "MARKETING_VERSION",
                PlayerSettings.bundleVersion);
            // Add AdServices framework
            pbxProject.AddFrameworkToProject(mainTargetGuid, "AdServices.framework", true );
            //
            pbxProject.WriteToFile(projectPath);
        }

        //Fix build xcode 14
        void SigningPod()
        {
            var appFilePath = path + "/fastlane/Appfile";
            var appFileContent = File.ReadAllText(appFilePath);
            var teamId = appFileContent.Split(new[] { "team_id" }, StringSplitOptions.None)[2]
                .GetBetweenString("(\"", "\")", false);
            Debug.Log("teamId:" + teamId);
            var podPbx = new PBXProject();
            podPbx.ReadFromFile(podProjPath);
            var id = podPbx.TargetGuidByName("FBSDKCoreKit-FacebookSDKStrings");
            Debug.Log(id);
            podPbx.SetTeamId(id, teamId);
            podPbx.WriteToFile(podProjPath);
        }
        //

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
                var stringPBXResourcesBuildPhase =
                    pbxProjectStr.GetBetweenString("/* Begin PBXResourcesBuildPhase section */", "}");
                if (!stringPBXResourcesBuildPhase.Contains("GoogleService-Info.plist in Resources"))
                {
                    var googlePlistFileGuid = pbxProject.FindFileGuidByProjectPath("GoogleService-Info.plist");

                    var randomGenerateGuid = Guid.NewGuid().ToString().Replace("-", "").ToUpper().Substring(0, 24);

                    var newStringPBXResourcesBuildPhase = stringPBXResourcesBuildPhase.Replace(
                        "/* Data in Resources */,",
                        $"/* Data in Resources */,\n\t\t\t\t{randomGenerateGuid} /* GoogleService-Info.plist in Resources */,");

                    pbxProjectStr =
                        pbxProjectStr.Replace(stringPBXResourcesBuildPhase, newStringPBXResourcesBuildPhase);
                    pbxProjectStr = pbxProjectStr.Replace(" /* Il2CppGenericClassTable.c */; };",
                        $" /* Il2CppGenericClassTable.c */; }};\n\t\t{randomGenerateGuid} /* GoogleService-Info.plist in Resources */ = {{isa = PBXBuildFile; fileRef = {googlePlistFileGuid} /* GoogleService-Info.plist */; }};");
                    File.WriteAllText(projectPath, pbxProjectStr);

                    Debug.Log("GoogleService-Info.plist Target Memeber ship enabled");
                }
            }
        }
    }
    
    public static void AddMissingSKAdNetworkID(List<string> missingSkAdNetworkItems, string path = "")
    {
        var plistPath = Path.Combine(path, "Info.plist");
        var plist = new PlistDocument();
        plist.ReadFromFile(plistPath);

        var rootDict = plist.root;
        var plistSKAdNetworkItemsArray = rootDict.values["SKAdNetworkItems"].AsArray();

        var missingSKAdId = missingSkAdNetworkItems.Where(smaatoId =>
                !plistSKAdNetworkItemsArray.values.Any(x =>
                    x.AsDict().values["SKAdNetworkIdentifier"].AsString().Contains(smaatoId)))
            .ToList();

        if (missingSKAdId.Count == 0) return;
        Debug.LogWarning($"{missingSKAdId.Count} SKAdnetworkID Missing: {missingSKAdId.JsonSerialize()}");

        missingSKAdId.ForEach(missingId =>
            plistSKAdNetworkItemsArray.AddDict().SetString("SKAdNetworkIdentifier", missingId));
        File.WriteAllText(plistPath, plist.WriteToString());
    }

    private static void CheckMissingSKAdId(string path)
    {
        var listMissing = new List<string>()
        {
            "bvpn9ufa9b.skadnetwork",
            "gvmwg8q7h5.skadnetwork",
            "hjevpa356n.skadnetwork",
            "pu4na253f3.skadnetwork",
            "yrqqpx2mcb.skadnetwork",
            "z4gj7hsk7h.skadnetwork",
            "su67r6k2v3.skadnetwork",
            "h65wbv5k3f.skadnetwork",
            "24t9a8vw3c.skadnetwork",
            "24zw6aqk47.skadnetwork",
            "275upjj5gd.skadnetwork",
            "294l99pt4k.skadnetwork",
            "32z4fx6l9h.skadnetwork",
            "3l6bd9hu43.skadnetwork",
            "424m5254lk.skadnetwork",
            "44jx6755aq.skadnetwork",
            "44n7hlldy6.skadnetwork",
            "4883q3dtq.skadnetwork",
            "523jb4fst2.skadnetwork",
            "52fl2v3hgk.skadnetwork",
            "54nzkqm89y.skadnetwork",
            "5l3tpt7t6e.skadnetwork",
            "5lm9lj6jb7.skadnetwork",
            "5tjdwbrq8w.skadnetwork",
            "6964rstnh4.skadnetwork",
            "6g9af3uyq4.skadnetwork",
            "6p4ks3rnbw.skadnetwork",
            "6v7lgmsu45.skadnetwork",
            "6xzpug9s2p8.skadnetwork",
            "737z793b9f.skadnetwork",
            "74b6s63p6l.skadnetwork",
            "7953jerfzd.skadnetwork",
            "79pbpufp6p.skadnetwork",
            "7fmhfwg9en.skadnetwork",
            "7rz58n8ntl.skadnetwork",
            "84993kbrcf.skadnetwork",
            "89z7zv988g.skadnetwork",
            "8m87ys6875.skadnetwork",
            "97r2b46745.skadnetwork",
            "9b89h5y424.skadnetwork",
            "9nlqeag3gk.skadnetwork",
            "9rd848q2bz.skadnetwork",
            "9vvzujtq5s.skadnetwork",
            "9yg77x724h.skadnetwork",
            "a7xqa6mtl2.skadnetwork",
            "av6w8kgt66.skadnetwork",
            "b9bk5wbcq9.skadnetwork",
            "bxvub5ada5.skadnetwork",
            "c3frkrj4fj.skadnetwork",
            "cg4yq2srnc.skadnetwork",
            "cj5566h2ga.skadnetwork",
            "cs644xg564.skadnetwork",
            "ejvt5qm6ak.skadnetwork",
            "f73kdq92p3.skadnetwork",
            "feyaarzu9v.skadnetwork",
            "g28c52eehv.skadnetwork",
            "g2y4y55b64.skadnetwork",
            "ggvn48r87g.skadnetwork",
            "glqzh8vgby.skadnetwork",
            "hb56zgv37p.skadnetwork",
            "k674qkevps.skadnetwork",
            "kbmxgpxpgc.skadnetwork",
            "m297p6643m.skadnetwork", 
            "m5mvw97r93.skadnetwork",
            "m8dbw4sv7c.skadnetwork",
            "mls7yz5dvl.skadnetwork",
            "mp6xlyr22a.skadnetwork",
            "mtkv5xtk9e.skadnetwork",
            "n66cz3y3bx.skadnetwork",
            "n9x2a789qt.skadnetwork",
            "nzq8sh4pbs.skadnetwork",
            "prcb7njmu6.skadnetwork",
            "pwdxu55a5a.skadnetwork",
            "qqp299437r.skadnetwork",
            "qu637u8glc.skadnetwork",
            "r45fhb6rf7.skadnetwork",
            "rvh3l7un93.skadnetwork",
            "rx5hdcabgc.skadnetwork",
            "tl55sbb4fm.skadnetwork",
            "u679fj5vs4.skadnetwork",
            "vcra2ehyfk.skadnetwork",
            "vhf287vqwu.skadnetwork",
            "w9q455wk68.skadnetwork",
            "wg4vff78zm.skadnetwork",
            "x44k69ngh6.skadnetwork",
            "xy9t38ct57.skadnetwork",
            "zmvfpc5aq8.skadnetwork" 
        };

        AddMissingSKAdNetworkID(listMissing, path);
    }
#endif
}