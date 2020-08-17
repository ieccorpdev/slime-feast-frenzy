using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using BestHTTP;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using ICSharpCode.SharpZipLib.BZip2;
using ICSharpCode.SharpZipLib.Zip;
using Newtonsoft.Json;
using Debug = UnityEngine.Debug;


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

        public const string VERSION = "1.0.0";
        
        private const string m_GoogleSheetUrlDefault =
            "https://docs.google.com/spreadsheets/d/1L7XrEic88trSrhg3drmlA5X5YkbP5ZMxRx3u1gUWcYg/edit#gid=927018210";

        private string m_GoogleSheetUrl = m_GoogleSheetUrlDefault;
        
        private bool m_ShowDebugButton = false;

        private int m_PluginApplovinVersionSelect;
        private int m_PluginFirebaseVersionSelect;

        private GUIStyle m_LabelStyle;
        

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
            SdkHelper.LoadApplovinVersionList();
        }

        private void InitGuiParams()
        {
            m_LabelStyle = new GUIStyle(GUI.skin.label) {fontSize = 18, fontStyle = FontStyle.Bold};
        }

        private static Vector2 buttonSize = new Vector2(170, 20);
        private static Vector2 dropdownSize = new Vector2(60, 20);
        
        private void OnGUI()
        {
            InitGuiParams();
            
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

            var loadingDotText = "";
            var loadingDotCount = DateTime.Now.Second % 3 + 1;
            for (var i = 0; i < loadingDotCount; i++) loadingDotText += ".";
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
            
            GUILayout.Space(20);
            CreateGuiButton($"Firebase SDK", () =>
            {
                ImportUnityPackageHelper.DownloadPackage(SdkHelper.FirebaseSdkUrlByVersion(SdkHelper.ListFirebaseVersion[m_PluginFirebaseVersionSelect]));
                ScriptingDefineSymbolEditor.AddDefineSymbol(PluginDefineSymbol.PluginFirebase);
                ScriptingDefineSymbolEditor.AddDefineSymbol(PluginDefineSymbol.PluginFirebaseAnalytic);
            });
            
            m_PluginFirebaseVersionSelect = EditorGUILayout.Popup("", m_PluginFirebaseVersionSelect, 
                SdkHelper.ListFirebaseVersion, GUILayout.Width(dropdownSize.x), GUILayout.Height(dropdownSize.y));
            
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();

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

    }

}

