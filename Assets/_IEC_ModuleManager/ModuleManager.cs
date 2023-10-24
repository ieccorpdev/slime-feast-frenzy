using System;
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
        PluginMaxAmazonNetwork
    }
    
    public class ModuleManager : EditorWindow
    {
        private GUIStyle m_StyleHeader1;
        private GUIStyle m_StyleHeader2;
        private static Vector2 buttonSize = new Vector2(300, 20);
        private static Vector2 dropdownSize = new Vector2(60, 20);
        
        
        [MenuItem("IEC/Module Manager")]
        public static void ShowWindow()
        {
            //Show existing window instance. If one doesn't exist, make one.
            GetWindow(typeof(ModuleManager), false, $"IEC Module Download");
        }

        private static readonly GitModule IECSdk = new GitModule
        {
            id = "com.iec.plugin"
        };
        private void Awake()
        {
            IECSdk.LoadSdkVersions();
        }

        private void OnGUI()
        {
            InitGuiParams();
            var loadingDotText = "";
            var loadingDotCount = DateTime.Now.Second % 3 + 1;
            for (var i = 0; i < loadingDotCount; i++) loadingDotText += ".";

            CreateGuiLabel("1. Sync SDK", m_StyleHeader1);
            // Debug.LogError(IECSdk);
            var iecPluginLoaded = IECSdk.LoadPKGVersionFromCache();
            // GUILayout.Label("Access Token:", GUILayout.Width(100f));
            // m_AccessToken = EditorGUILayout.TextField(m_AccessToken,GUILayout.Width(200f));
            CreateGuiBreakLine();
            CreateGuiButton(
                $"IEC Plugin {(iecPluginLoaded ? "Load Done" : $"Loading {loadingDotText}")}", () =>
                {
                    if (!iecPluginLoaded)
                    {
                        return;
                    }
                    IECSdk.DownloadModule(IECSdk.listVersion[IECSdk.versionSelected]);
                });

            IECSdk.versionSelected = EditorGUILayout.Popup("", IECSdk.versionSelected,
                IECSdk.listVersion,
                GUILayout.Width(100), GUILayout.Height(dropdownSize.y));
            CreateGuiBreakLine();
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