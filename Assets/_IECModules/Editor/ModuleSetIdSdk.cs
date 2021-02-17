using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using SimpleJSON;


namespace _IECModules
{
    public class ModuleSetIdSdk : EditorWindow
    {
        [MenuItem("IEC/Set ID SDK")]
        public static void ShowWindow()
        {
            //Show existing window instance. If one doesn't exist, make one.
            GetWindow(typeof(ModuleSetIdSdk), false, $"Set ID SDK v{VERSION}");
        }

        public const string VERSION = "1.3.0";

        private const string m_GoogleSheetUrlDefault =
            "https://docs.google.com/spreadsheets/d/1L7XrEic88trSrhg3drmlA5X5YkbP5ZMxRx3u1gUWcYg/edit#gid=927018210";

        private string m_GoogleSheetUrl = m_GoogleSheetUrlDefault;

        private const string m_AbVersionDefault =
            "AB_20201029_v44";

        private string m_AbVersionString = m_AbVersionDefault;

        private bool m_ShowDebugButton = false;

        private int m_PluginApplovinVersionSelect;
        private int m_PluginFirebaseVersionSelect;
        private int m_PluginEDMVersionSelect;
        private int m_PluginFirebaseCoreSelect;

        private GUIStyle m_StyleHeader1;
        private GUIStyle m_StyleHeader2;

        private static RegistryManifest GoogleRegistry = new RegistryManifest
        {
            name = "Game Package Registry by Google",
            url = "https://unityregistry-pa.googleapis.com",
            scopes = new List<string>()
            {
                "com.google"
            }
        };

        public enum PluginDefineSymbol
        {
            PluginFacebook,
            PluginFirebase,
            PluginFirebaseAnalytic,
            PluginFirebaseRemote,
            PluginKochava,
            PluginApplovin,
            PluginIronSource,
            PluginUnityIAP,
            PluginFacebookAudienceNetwork,
            PluginInAppRating,
            PluginByteDanceChinaIOS
        }

        private void Awake()
        {
            LogEditorHelper.ClearLog();
            ProjectChecklistHelper.LoadServicesConfig();
            if (ProjectChecklistHelper.servicesConfig != null && !string.IsNullOrEmpty(ProjectChecklistHelper.servicesConfig.projectChecklistUrl))
                m_GoogleSheetUrl = ProjectChecklistHelper.servicesConfig.projectChecklistUrl;
            AddRegistries(GoogleRegistry);
            SdkHelper.LoadApplovinVersionList();
            var unityPackages = SdkHelper.DictUnityPackage.Values;
            foreach (var pkg in unityPackages)
            {
                pkg.LoadPKGVersionList();
            }
        }

        private void InitGuiParams()
        {
            if (m_StyleHeader1 == null) m_StyleHeader1 = new GUIStyle(GUI.skin.label) { fontSize = 18, fontStyle = FontStyle.Bold };
            if (m_StyleHeader2 == null) m_StyleHeader2 = new GUIStyle(GUI.skin.label) { fontSize = 12, fontStyle = FontStyle.Bold };
        }

        private static Vector2 buttonSize = new Vector2(300, 20);
        private static Vector2 dropdownSize = new Vector2(60, 20);

        private void OnGUI()
        {
            InitGuiParams();
            var loadingDotText = "";
            var loadingDotCount = DateTime.Now.Second % 3 + 1;
            for (var i = 0; i < loadingDotCount; i++) loadingDotText += ".";
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button($"Show Debug Buttons: {m_ShowDebugButton}", GUILayout.Width(buttonSize.x), GUILayout.Height(buttonSize.y)))
            {
                m_ShowDebugButton = !m_ShowDebugButton;
            }

            if (m_ShowDebugButton)
            {
                GUILayout.Space(20);
                CreateGuiButton("Test Create Keystore", () =>
                {
                    KeystoreHelper.CreateKeyStore("thanhdeptrai");
                });

                GUILayout.Space(20);
                CreateGuiButton("Test Add Define PluginFacebook", () =>
                {
                    ScriptingDefineSymbolEditor.AddDefineSymbol(PluginDefineSymbol.PluginFacebook);
                }, 220);

                GUILayout.Space(20);
                CreateGuiButton("Test Remove Define PluginFacebook", () =>
                {
                    ScriptingDefineSymbolEditor.RemoveDefineSymbol(PluginDefineSymbol.PluginFacebook);
                }, 220);
            }

            CreateGuiLabel("1. Install SDK", m_StyleHeader1);

            CreateGuiLabel("- SDK For Prototype", m_StyleHeader2);

            CreateGuiButton("Facebook SDK (latest)", () =>
            {
                ImportUnityPackageHelper.DownloadPackage(SdkHelper.FACEBOOK_SDK_CURRENT_VERSION_URL);
                ScriptingDefineSymbolEditor.AddDefineSymbol(PluginDefineSymbol.PluginFacebook);
            });

            GUILayout.Space(20);

            CreateGuiButton($"Kochava SDK {SdkHelper.KOCHAVA_CURRENT_VERSION}", () =>
            {
                ImportUnityPackageHelper.DownloadPackage(SdkHelper.KochavaSdkUrlByVersion);
                ScriptingDefineSymbolEditor.AddDefineSymbol(PluginDefineSymbol.PluginKochava);
            });

            CreateGuiLabel("- SDK For LTV", m_StyleHeader2);

            var applovinSdkLoaded = SdkHelper.LoadApplovinVersionFromCache();
            CreateGuiButton($"Applovin SDK {(applovinSdkLoaded ? "Load Done" : $"Loading {loadingDotText}")}", () =>
            {
                ImportUnityPackageHelper.DownloadPackage(SdkHelper.ApplovinSdkUrlByVersion(SdkHelper.ListApplovinVersion[m_PluginApplovinVersionSelect]));
                ScriptingDefineSymbolEditor.AddDefineSymbol(PluginDefineSymbol.PluginApplovin);
            });

            m_PluginApplovinVersionSelect = EditorGUILayout.Popup("", m_PluginApplovinVersionSelect,
                SdkHelper.ListApplovinVersion,
                GUILayout.Width(220), GUILayout.Height(dropdownSize.y));

            CreateGuiBreakLine();

            CreateGuiButton($"Facebook Audience Network v6.2.1", () =>
            {
                ImportUnityPackageHelper.DownloadPackage(SdkHelper.FacebookAudienceNetworkSdkUrlByVersion());
                ScriptingDefineSymbolEditor.AddDefineSymbol(PluginDefineSymbol.PluginFacebookAudienceNetwork);
            });

            CreateGuiBreakLine();

            #region Firebase Analytics

            var firebaseAnalyticPkg = SdkHelper.DictUnityPackage[SdkHelper.FIREBAE_ANALYTIC_PKG];
            var firebaseAnalyticLoaded = firebaseAnalyticPkg.LoadPKGVersionFromCache();
            CreateGuiButton($"Firebase Analytics {(firebaseAnalyticLoaded ? "Load Done" : $"Loading {loadingDotText}")}", () =>
            {
                if (!firebaseAnalyticLoaded)
                {
                    return;
                }
                AddRegistries(GoogleRegistry);
                firebaseAnalyticPkg.Install();
                ScriptingDefineSymbolEditor.AddDefineSymbol(PluginDefineSymbol.PluginFirebase);
                ScriptingDefineSymbolEditor.AddDefineSymbol(PluginDefineSymbol.PluginFirebaseAnalytic);
            });

            firebaseAnalyticPkg.versionSelected = EditorGUILayout.Popup("", firebaseAnalyticPkg.versionSelected,
               firebaseAnalyticPkg.listVersion,
               GUILayout.Width(100), GUILayout.Height(dropdownSize.y));

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();

            #endregion

            #region Firebase RemoteConfig

            var firebaseRemotePkg = SdkHelper.DictUnityPackage[SdkHelper.FIREBASE_REMOTE_CONFIG_PKG];
            var firebaseRemoteLoaded = firebaseRemotePkg.LoadPKGVersionFromCache();
            CreateGuiButton($"Firebase Remote Config {(firebaseRemoteLoaded ? "Load Done" : $"Loading {loadingDotText}")}", () =>
            {
                if (!firebaseRemoteLoaded)
                {
                    return;
                }
                AddRegistries(GoogleRegistry);
                firebaseRemotePkg.Install();
                ScriptingDefineSymbolEditor.AddDefineSymbol(PluginDefineSymbol.PluginFirebase);
                ScriptingDefineSymbolEditor.AddDefineSymbol(PluginDefineSymbol.PluginFirebaseRemote);
            });

            firebaseRemotePkg.versionSelected = EditorGUILayout.Popup("", firebaseRemotePkg.versionSelected,
               firebaseRemotePkg.listVersion,
               GUILayout.Width(100), GUILayout.Height(dropdownSize.y));

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();

            #endregion

            #region In App Rating

            var inAppRatingPkg = SdkHelper.DictUnityPackage[SdkHelper.IN_APP_RATING_PKG];
            var inAppRatingLoaded = inAppRatingPkg.LoadPKGVersionFromCache();
            CreateGuiButton($"In App Rating {(inAppRatingLoaded ? "Load Done" : $"Loading {loadingDotText}")}", () =>
            {
                if (!inAppRatingLoaded)
                {
                    return;
                }
                AddRegistries(GoogleRegistry);
                inAppRatingPkg.Install();
                ScriptingDefineSymbolEditor.AddDefineSymbol(PluginDefineSymbol.PluginInAppRating);
            });
            inAppRatingPkg.versionSelected = EditorGUILayout.Popup("", inAppRatingPkg.versionSelected,
               inAppRatingPkg.listVersion,
               GUILayout.Width(100), GUILayout.Height(dropdownSize.y));

            CreateGuiBreakLine();

            #endregion

            #region ByteDance China IOS
            CreateGuiLabel("ByteDance China IOS", m_StyleHeader1);
            CreateGuiButton($"ByteDance SDK", () =>
            {
                ScriptingDefineSymbolEditor.AddDefineSymbol(PluginDefineSymbol.PluginByteDanceChinaIOS);
                CopyByteDanceSDKChinaIOS();
                LogEditorHelper.AddLog("Copy ByteDance SDK done!");
            });
            CreateGuiBreakLine();
            #endregion

            CreateGuiLabel("2. Project Resolver", m_StyleHeader1);

            #region External Dependency Manager

            var edmPkg = SdkHelper.DictUnityPackage[SdkHelper.EXTERNAL_DEPENDENCY_PKG];
            var edmSdkLoaded = edmPkg.LoadPKGVersionFromCache();
            CreateGuiButton($"External Dependency Manager {(edmSdkLoaded ? "Load Done" : $"Loading {loadingDotText}")}", () =>
            {
                if (!edmSdkLoaded) return;
                ProjectResolveHelper.DeleteObsoleteResolver();
                AddRegistries(GoogleRegistry);
                edmPkg.Install();
            });

            edmPkg.versionSelected = EditorGUILayout.Popup("", edmPkg.versionSelected,
                edmPkg.listVersion,
                GUILayout.Width(100), GUILayout.Height(dropdownSize.y));

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();

            #endregion

            CreateGuiButton("RESOLVE PROJECT!", ProjectResolveHelper.ResolveProject, buttonSize.x * 1.3f);

            CreateGuiBreakLine();

            ProjectResolveHelper.EditorOnGUI();

            CreateGuiLabel("3. Update Project Checklist", m_StyleHeader1);

            if (GUILayout.Button("Update from Google Docs", GUILayout.Width(buttonSize.x), GUILayout.Height(buttonSize.y)))
            {
                OnClickSetSDKId();
            }
            m_GoogleSheetUrl = EditorGUILayout.TextField(m_GoogleSheetUrl, GUILayout.Width(EditorGUIUtility.currentViewWidth - 250));

            CreateGuiLabel("4. Update AB Test Version", m_StyleHeader1);

            if (GUILayout.Button("Update string directly", GUILayout.Width(buttonSize.x), GUILayout.Height(buttonSize.y)))
            {
                ProjectChecklistHelper.UpdateAbVersion(m_AbVersionString);
            }
            m_AbVersionString = EditorGUILayout.TextField(m_AbVersionString, GUILayout.Width(EditorGUIUtility.currentViewWidth - 250));

            CreateGuiBreakLine();
            if (GUILayout.Button("Update from Project Checklist", GUILayout.Width(buttonSize.x), GUILayout.Height(buttonSize.y)))
            {
                ProjectChecklistHelper.UpdateAbVersionFromChecklist(m_GoogleSheetUrl);
            }

            EditorGUILayout.EndHorizontal();

            LogEditorHelper.EditorOnGUI();
        }

        private void CreateGuiButton(string text, Action onClickButton) => CreateGuiButton(text, onClickButton, buttonSize.x);
        private void CreateGuiButton(string text, Action onClickButton, float buttonWidth)
        {
            if (!GUILayout.Button(text, GUILayout.Width(buttonWidth), GUILayout.Height(buttonSize.y))) return;
            LogEditorHelper.ClearLog();
            onClickButton?.Invoke();
        }
        private void CreateGuiLabel(string text, GUIStyle guiStyle)
        {
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            GUILayout.Label(text, guiStyle);
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
        }
        private void CreateGuiBreakLine()
        {
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
        }

        private void OnClickSetSDKId()
        {
            LogEditorHelper.ClearLog();

            ProjectChecklistHelper.ApplySdkSetting(m_GoogleSheetUrl);

            ProjectSettingRemoveSplashScreen();
        }

        private void ProjectSettingRemoveSplashScreen()
        {
            var projectSettingFileName = "ProjectSettings/ProjectSettings.asset";
            var projectSettingFileContent = File.ReadAllText(projectSettingFileName);
            projectSettingFileContent =
                projectSettingFileContent.Replace("m_ShowUnitySplashScreen: 1", "m_ShowUnitySplashScreen: 0");
            File.WriteAllText(projectSettingFileName, projectSettingFileContent);
        }

        private void AddRegistries(RegistryManifest registry)
        {
            var manifestFileName = "Packages/manifest.json";
            var manifestFileContent = File.ReadAllText(manifestFileName);

            var jsonNode = JSON.Parse(manifestFileContent);
            jsonNode["scopedRegistries"] = new JSONArray();
            var scopes = new JSONArray();
            foreach (var regScope in registry.scopes)
            {
                scopes.Add(new JSONString(regScope));
            }

            var item = new JSONObject
            {
                ["name"] = registry.name,
                ["url"] = registry.url,
                ["scopes"] = scopes
            };
            jsonNode["scopedRegistries"].Add(item);
            File.WriteAllText(manifestFileName, jsonNode.ToString(2));
        }

        private void CopyByteDanceSDKChinaIOS()
        {
            var sourcePath = Application.dataPath + "/_IECModules/Plugins/ByteDanceChinaIOS/";
            var desPath = Application.dataPath + "/ByteDanceChina";
            if (!Directory.Exists(desPath))
            {
                Directory.CreateDirectory(desPath);
            }
            if (!Directory.Exists(desPath + "/Editor"))
            {
                Directory.CreateDirectory(desPath + "/Editor");
            }
            File.Copy(sourcePath + "Dependencies.xml", desPath + "/Editor/Dependencies.xml", true);

            if (!Directory.Exists(desPath + "/Plugins/iOS"))
            {
                Directory.CreateDirectory(desPath + "/Plugins/iOS");
            }

            File.Copy(sourcePath + "ByteDanceChinaIOS.code", desPath + "/Plugins/iOS/ByteDanceChinaIOS.mm", true);
            File.Copy(sourcePath + "ByteDanceChinaIOS.bridge", desPath + "/Plugins/iOS/ByteDanceChinaIOS.h", true);
        }
    }

    public class RegistryManifest
    {
        public string name;
        public string url;
        public List<string> scopes;
    }
}

