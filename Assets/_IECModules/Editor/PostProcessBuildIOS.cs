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
            "zmvfpc5aq8.skadnetwork",
            "488r3q3dtq.skadnetwork",
            "6964rsfnh4.skadnetwork",
            "6xzpu9s2p8.skadnetwork",
            "238da6jt44.skadnetwork",
            "252b5q8x7y.skadnetwork",
            "4mn522wn87.skadnetwork",
            "4w7y6s5ca2.skadnetwork",
            "8r8llnkz5a.skadnetwork",
            "a8cz6cu7e5.skadnetwork",
            "cwn433xbcr.skadnetwork",
            "dbu4b84rxf.skadnetwork",
            "dkc879ngq3.skadnetwork",
            "dzg6xy7pwj.skadnetwork",
            "f7s53z58qe.skadnetwork",
            "g6gcrrvk4p.skadnetwork",
            "gta8lk7p23.skadnetwork",
            "hdw39hrw9y.skadnetwork",
            "krvm3zuq6h.skadnetwork",
            "lr83yxwka7.skadnetwork",
            "mqn7fxpca7.skadnetwork",
            "s69wq72ugq.skadnetwork",
            "v79kvwwj4g.skadnetwork",
            "x5l83yy675.skadnetwork",
            "x8jxxk4ff5.skadnetwork",
            "x8uqf25wch.skadnetwork",
            "xga6mpmplv.skadnetwork",
            "y45688jllp.skadnetwork",
            "z24wtl6j62.skadnetwork",
            "55644vm79v.skadnetwork",
            "577p5t736z.skadnetwork",
            "6rd35atwn8.skadnetwork",
            "6yxyv74ff7.skadnetwork",
            "7bxrt786m8.skadnetwork",
            "7fbxrn65az.skadnetwork",
            "8r8lInkz5a.skadnetwork",
            "9g2aggbj52.skadnetwork",
            "ce8ybjwass.skadnetwork",
            "dt3cjx1a9i.skadnetwork",
            "fz2k2k5tej.skadnetwork",
            "jk2fsx2rgz.skadnetwork",
            "k6y4y55b64.skadnetwork",
            "In5gz23vtd.skadnetwork",
            "Ir83yxwka7.skadnetwork",
            "mj797d8u6f.skadnetwork",
            "nu4557a4je.skadnetwork",
            "qwpu75vrh2.skadnetwork",
            "r8lj5b58b5.skadnetwork",
            "t6d3zquu66.skadnetwork",
            "tmhh9296z4.skadnetwork",
            "tvvz7th9br.skadnetwork",
            "w7jznl3r6g.skadnetwork",
            "x5183yy675.skadnetwork",
            "xga6nmpmplv.skadnetwork",
            "y45688jillp.skadnetwork",
            "z959bm4gru.skadnetwork",
            "55y65gfgn7.skadnetwork",
            "ns5j362hk7.skadnetwork",
            "6xzzpu9s2p8.skadnetwork",
            "g6gcrvvk4p.skadnetwork",
            "w7jzn13r6g.skadnetwork",
            "dbu4bb84rxf.skadnetwork",
            "5f5u5tfb26.skadnetwork",
            "2q884k2j68.skadnetwork",
            "2rq3zucswp.skadnetwork",
            "2tdux391x8.skadnetwork",
            "33r6p7g8nc.skadnetwork",
            "3cgn6rq224.skadnetwork",
            "5ghnmfs3dh.skadnetwork",
            "5mv394q32t.skadnetwork",
            "627r9wr2y5.skadnetwork",
            "633vhxswh4.skadnetwork",
            "67369282zy.skadnetwork",
            "6qx585k4p6.skadnetwork",
            "79w64w269u.skadnetwork",
            "7k3cvf297u.skadnetwork",
            "7tnzynbdc7.skadnetwork",
            "866k9ut3g3.skadnetwork",
            "88k8774x49.skadnetwork",
            "899vrgt9g8.skadnetwork",
            "8qiegk9qfv.skadnetwork",
            "8w3np9182g.skadnetwork",
            "9wsyqb3ku7.skadnetwork",
            "au67k4efj4.skadnetwork",
            "axh5283zss.skadnetwork",
            "b55w3d8y8z.skadnetwork",
            "c7g47wypnu.skadnetwork",
            "cad8qz2s3j.skadnetwork",
            "d7g9azk84q.skadnetwork",
            "dd3a75yxkv.skadnetwork",
            "dmv22haz9p.skadnetwork",
            "dn942472g5.skadnetwork",
            "dr774724x4.skadnetwork",
            "dticjx1a9i.skadnetwork",
            "eqhxz8m8av.skadnetwork",
            "fkak3gfpt6.skadnetwork",
            "g69uk9uh2b.skadnetwork",
            "gfat3222tu.skadnetwork",
            "h5jmij969g5.skadnetwork",
            "h8vml93bkz.skadnetwork",
            "jb7bn6koa5.skadnetwork",
            "l6nv3x923s.skadnetwork",
            "l93v5h6a4m.skadnetwork",
            "nfqy3847ph.skadnetwork",
            "nrt9jy4kw9.skadnetwork",
            "pd25vrrwzn.skadnetwork",
            "qlbq5gtkt8.skadnetwork",
            "r26jly69rpl.skadnetwork",
            "s69wq72ugg.skadnetwork",
            "sczv5946wb.skadnetwork",
            "t3b3f7n3x8.skadnetwork",
            "t7ky8fmwkd.skadnetwork",
            "uzqba5354d.skadnetwork",
            "v7896pgt74.skadnetwork",
            "vc83br9sjg.skadnetwork",
            "w28pnjg2k4.skadnetwork",
            "x2jnk7ly8j.skadnetwork",
            "x5854y7y24.skadnetwork",
            "xmn954pzmp.skadnetwork",
            "xx9sdjej2w.skadnetwork",
            "y755zyxw56.skadnetwork",
            "z5b3gh5ugf.skadnetwork",
            "zh3b7bxvad.skadnetwork"
        };

        AddMissingSKAdNetworkID(listMissing, path);
    }
#endif
}