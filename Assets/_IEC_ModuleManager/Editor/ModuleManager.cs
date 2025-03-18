using System;
using System.IO;
using _IECModules;
using UnityEditor;
using UnityEngine;

namespace IEC.ModuleManager
{
    public enum PluginDefineSymbol
    {
        PluginFacebook,
        PluginFirebase,
        PluginFirebaseAnalytic,
        PluginFirebaseRemote,
        PluginKochava,
        PluginAdjust,
        PluginApplovin,
        PluginIronSource,
        PluginUnityIAP,
        PluginFacebookAudienceNetwork,
        PluginInAppRating,
        PluginByteDanceChinaIOS,
        PluginNotification,
        PluginOgury,
        PluginLocalization,
        PluginMaxAmazonNetwork,
        PluginStartIONetwork,
    }
    
    public class ModuleManager : EditorWindow
    {
        private GUIStyle m_StyleHeader1;
        private GUIStyle m_StyleHeader2;
        private static Vector2 buttonSize = new Vector2(300, 20);
        private static Vector2 dropdownSize = new Vector2(60, 20);
        
        private const string m_GoogleSheetUrlDefault =
            "https://docs.google.com/spreadsheets/d/1L7XrEic88trSrhg3drmlA5X5YkbP5ZMxRx3u1gUWcYg/edit#gid=927018210";

        private string m_GoogleSheetUrl = m_GoogleSheetUrlDefault;
        
        [MenuItem("IEC/Module Manager")]
        public static void ShowWindow()
        {
            //Show existing window instance. If one doesn't exist, make one.
            GetWindow(typeof(ModuleManager), false, $"IEC Module Manager");
        }

        private static readonly GitModule IecSdk = new GitModule
        {
            id = "com.iec.plugin",
            jsonID = "sdk",
            isAddAsset = true,
            isAddPackage = false
        };
        
        private static readonly GitModule IecPackage = new GitModule
        {
            id = "iec-package",
            jsonID = "package",
            isAddAsset = false,
            isAddPackage = true
        };
        
        private void Awake()
        {
            IecSdk.LoadSdkVersions();
            IecPackage.LoadSdkVersions();
            
            ProjectChecklistHelper.LoadServicesConfig();
            if (ProjectChecklistHelper.servicesConfig != null &&
                !string.IsNullOrEmpty(ProjectChecklistHelper.servicesConfig.projectChecklistUrl))
                m_GoogleSheetUrl = ProjectChecklistHelper.servicesConfig.projectChecklistUrl;
        }

        private void OnGUI()
        {
            InitGuiParams();

            CreateGuiLabel("1. Asset SDK", m_StyleHeader1);
            CreateGuiLabel("(Support Adjust, AudienceNetwork, Facebook, Applovin Max, Amazon)", m_StyleHeader2);
            ShowGitModule(IecSdk);
            CreateGuiBreakLine();
            
            CreateGuiLabel("2. Package SDK", m_StyleHeader1);
            CreateGuiLabel("(Support EDM, Firebase, Notification, InAppRating, InAppPurchase, Localization)", m_StyleHeader2);
            ShowGitModule(IecPackage);
            CreateGuiBreakLine();
            
            CreateGuiLabel("3. Update Project Checklist", m_StyleHeader1);
            if (GUILayout.Button("Click to UPDATE", 
                    GUILayout.Width(buttonSize.x), 
                    GUILayout.Height(buttonSize.y)))
            {
                ProjectChecklistHelper.ApplySdkSetting(m_GoogleSheetUrl);
                ProjectSettingRemoveSplashScreen();
            }
            m_GoogleSheetUrl = EditorGUILayout.TextField(m_GoogleSheetUrl, GUILayout.Width(EditorGUIUtility.currentViewWidth - buttonSize.x * 1.1f));

            CreateGuiBreakLine();
            
            CreateGuiLabel("4. RESOLVE PROJECT!", m_StyleHeader1);
            CreateGuiButton("Click to RESOLVE", ProjectResolveHelper.ResolveProject, buttonSize.x * 1.3f);
            CreateGuiBreakLine();
        }

        void ShowGitModule(GitModule gitModule)
        {
            var loadingDotText = "";
            var loadingDotCount = DateTime.Now.Second % 3 + 1;
            for (var i = 0; i < loadingDotCount; i++) loadingDotText += ".";
            
            var isVersionsLoaded = gitModule.IsVersionsLoaded;
            if (!isVersionsLoaded)
            {
                gitModule.LoadSdkVersions();
            }
            // GUILayout.Label("Access Token:", GUILayout.Width(100f));
            // m_AccessToken = EditorGUILayout.TextField(m_AccessToken,GUILayout.Width(200f));
            GUILayout.Label("Current version: " + gitModule.CurrentVersion, GUILayout.Width(500f));
            CreateGuiBreakLine();
            GUILayout.Label("Commit ID: " + gitModule.CurrentCommit, GUILayout.Width(500f));
            CreateGuiBreakLine();
            GUILayout.Label("Commit Title: " + gitModule.CurrentCommitTitle, GUILayout.Width(500f));
            CreateGuiBreakLine();
            GUILayout.Label("Commit Date: " + gitModule.CurrentCommitDate, GUILayout.Width(500f));
            CreateGuiBreakLine();
            GUILayout.Label("More info can be found at Assets/package.json", GUILayout.Width(500f));
            CreateGuiBreakLine();
            CreateGuiButton(
                $"{(isVersionsLoaded ? "Click to INSTALL" : $"Loading {loadingDotText}")}", () =>
                {
                    if (!isVersionsLoaded)
                    {
                        return;
                    }
                    gitModule.DownloadModule(gitModule.ListVersion[gitModule.versionSelected]);
                });

            gitModule.versionSelected = EditorGUILayout.Popup("", gitModule.versionSelected,
                gitModule.ListVersion,
                GUILayout.Width(200), GUILayout.Height(dropdownSize.y));
        }
        
        private void ProjectSettingRemoveSplashScreen()
        {
            var projectSettingFileName = "ProjectSettings/ProjectSettings.asset";
            var projectSettingFileContent = File.ReadAllText(projectSettingFileName);
            projectSettingFileContent =
                projectSettingFileContent.Replace("m_ShowUnitySplashScreen: 1", "m_ShowUnitySplashScreen: 0");
            File.WriteAllText(projectSettingFileName, projectSettingFileContent);
        }
        
        private void InitGuiParams()
        {
            m_StyleHeader1 ??= new GUIStyle(GUI.skin.label) { fontSize = 18, fontStyle = FontStyle.Bold };
            m_StyleHeader2 ??= new GUIStyle(GUI.skin.label) { fontSize = 12, fontStyle = FontStyle.Bold };
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
        
        private void CreateGuiButton(string text, Action onClickButton) =>
            CreateGuiButton(text, onClickButton, buttonSize.x);

        private void CreateGuiButton(string text, Action onClickButton, float buttonWidth)
        {
            if (!GUILayout.Button(text, GUILayout.Width(buttonWidth), GUILayout.Height(buttonSize.y))) return;
            onClickButton?.Invoke();
        }
    }
}