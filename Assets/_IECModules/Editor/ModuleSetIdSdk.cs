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

        public const string VERSION = "1.2.0";

        private const string m_GoogleSheetUrlDefault =
            "https://docs.google.com/spreadsheets/d/1L7XrEic88trSrhg3drmlA5X5YkbP5ZMxRx3u1gUWcYg/edit#gid=927018210";

        private string m_GoogleSheetUrl = m_GoogleSheetUrlDefault;

        private bool m_ShowDebugButton = false;

        private int m_PluginApplovinVersionSelect;
        private int m_PluginFirebaseVersionSelect;
        private int m_PluginEDMVersionSelect;
        private int m_PluginFirebaseCoreSelect;

        private GUIStyle m_LabelStyle;

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
            m_LabelStyle = new GUIStyle(GUI.skin.label) { fontSize = 18, fontStyle = FontStyle.Bold };
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
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            GUILayout.Label("Install SDK", m_LabelStyle);
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();



            CreateGuiButton("Facebook SDK (latest)", () =>
            {
                ImportUnityPackageHelper.DownloadPackage(SdkHelper.FACEBOOK_SDK_CURRENT_VERSION_URL);
                ScriptingDefineSymbolEditor.AddDefineSymbol(PluginDefineSymbol.PluginFacebook);
            });

            GUILayout.Space(20);


            var applovinSdkLoaded = SdkHelper.LoadApplovinVersionFromCache();
            CreateGuiButton($"Applovin SDK {(applovinSdkLoaded ? "Load Done" : $"Loading {loadingDotText}")}", () =>
            {
                ImportUnityPackageHelper.DownloadPackage(SdkHelper.ApplovinSdkUrlByVersion(SdkHelper.ListApplovinVersion[m_PluginApplovinVersionSelect]));
                ScriptingDefineSymbolEditor.AddDefineSymbol(PluginDefineSymbol.PluginApplovin);
            });

            m_PluginApplovinVersionSelect = EditorGUILayout.Popup("", m_PluginApplovinVersionSelect,
                SdkHelper.ListApplovinVersion,
                GUILayout.Width(220), GUILayout.Height(dropdownSize.y));

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();

            CreateGuiButton($"Kochava SDK {SdkHelper.KOCHAVA_CURRENT_VERSION}", () =>
            {
                ImportUnityPackageHelper.DownloadPackage(SdkHelper.KochavaSdkUrlByVersion);
                ScriptingDefineSymbolEditor.AddDefineSymbol(PluginDefineSymbol.PluginKochava);
            });

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();

            #region External Dependency Manager
            //GUILayout.TextArea("External Dependency Manager");

            var edmPkg = SdkHelper.DictUnityPackage[SdkHelper.EXTERNAL_DEPENDENCY_PKG];
            var edmSdkLoaded = edmPkg.LoadPKGVersionFromCache();
            CreateGuiButton($"External Dependency Manager {(edmSdkLoaded ? "Load Done" : $"Loading {loadingDotText}")}", () =>
            {
                if (!edmSdkLoaded)
                {
                    return;
                }
                DeleteObsoleteResolver();
                AddRegistries(GoogleRegistry);
                edmPkg.Install();
            });

            edmPkg.versionSelected = EditorGUILayout.Popup("", edmPkg.versionSelected,
               edmPkg.listVersion,
               GUILayout.Width(220), GUILayout.Height(dropdownSize.y));

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();

            #endregion

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
               GUILayout.Width(220), GUILayout.Height(dropdownSize.y));

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
               GUILayout.Width(220), GUILayout.Height(dropdownSize.y));

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();

            #endregion


            CreateGuiButton("Update Main Template and Android Resolve", ProjectResolveHelper.UpdateMainTemplateAndResolve, buttonSize.x * 2);

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            GUILayout.Label("Update Project Checklist", m_LabelStyle);
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Update from Google Docs", GUILayout.Width(buttonSize.x), GUILayout.Height(buttonSize.y)))
            {
                OnClickSetSDKId();
            }
            m_GoogleSheetUrl = EditorGUILayout.TextField(m_GoogleSheetUrl, GUILayout.Width(EditorGUIUtility.currentViewWidth - 250));
            EditorGUILayout.EndHorizontal();

            LogEditorHelper.EditorDrawLog();
        }

        private void CreateGuiButton(string text, Action onClickButton) => CreateGuiButton(text, onClickButton, buttonSize.x);
        private void CreateGuiButton(string text, Action onClickButton, float buttonWidth)
        {
            if (!GUILayout.Button(text, GUILayout.Width(buttonWidth), GUILayout.Height(buttonSize.y))) return;
            LogEditorHelper.ClearLog();
            onClickButton?.Invoke();
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

        private void DeleteObsoleteResolver()
        {
            if (FileUtil.DeleteFileOrDirectory("Assets/ExternalDependencyManager"))
            {
                LogEditorHelper.AddLog("Delete EDM");
            }
            if (FileUtil.DeleteFileOrDirectory("Assets/PlayServicesResolver"))
            {
                LogEditorHelper.AddLog("Delete Play Service Resolver");
            }
            AssetDatabase.Refresh();
        }

    }

    public class RegistryManifest
    {
        public string name;
        public string url;
        public List<string> scopes;
    }
}

