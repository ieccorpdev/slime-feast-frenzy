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
            GetWindow(typeof(ModuleSetIdSdk), false, "Set ID SDK");
        }


        private const string PROJECT_CHECKLIST_URL = "project_checklist_url";
        private const string LOG_SET_ID_SDK = "log_set_id_sdk";
        
        private string m_GoogleSheetUrl =
            "https://docs.google.com/spreadsheets/d/1L7XrEic88trSrhg3drmlA5X5YkbP5ZMxRx3u1gUWcYg/edit#gid=927018210";

        private List<LogData> m_SetSdkLog = new List<LogData>();
        
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

        public enum ChecklistItemName
        {
            ProductName,
            BundleID,
            KeyStoreName,
            KanboardSwimlane,
            DiscordChannel,
            FirebaseProject,
            UnityCloudProject,
            UnityCloudBuildKanboardWebhooks,
            UnityCloudBuildDiscordWebhooks,
            GitlabProject,
            GitlabIntegration,
            UnityCloudBuildTargetConfig,
            TrackingEventList,
            FacebookID,
            FirebaseConfigFileIOS,
            FirebaseConfigFileAndroid,
            KochavaIDIOS,
            KochavaIDAndroid,
            AppodealIDIOS,
            AppodealIDAndroid,
            AdmobIDIOS,
            AdmobIDAndroid,
            MarketingChecklist,
            IAPChecklist,
            IAPIOSList,
            IAPAndroidList,
            AdPosition,
            ApplovinIOSID,
            ApplovinAndroidID,
            ApplovinIOSInterID,
            ApplovinIOSBannerID,
            ApplovinIOSRewardID,
            ApplovinAndroidInterID,
            ApplovinAndroidBannerID,
            ApplovinAndroidRewardID,
            IronSourceIOSID,
            IronSourceAndroidID
        }

        private static Dictionary<string, ChecklistItemName> _checklistKey = new Dictionary<string, ChecklistItemName>
        {
            {"Tên game", ChecklistItemName.ProductName},
            {"BundleID", ChecklistItemName.BundleID},
            {"Keystore Name", ChecklistItemName.KeyStoreName},
            {"Kanboard Swimlane", ChecklistItemName.KanboardSwimlane},
            {"Discord Channel", ChecklistItemName.DiscordChannel},
            {"Firebase Project", ChecklistItemName.FirebaseProject},
            {"Unity Cloud Project", ChecklistItemName.UnityCloudProject},
            {"Unity Cloud Build Kanboard Webhooks", ChecklistItemName.UnityCloudBuildKanboardWebhooks},
            {"Unity Cloud Build Discord Webhooks", ChecklistItemName.UnityCloudBuildDiscordWebhooks},
            {"Gitlab project", ChecklistItemName.GitlabProject},
            {"Gitlab integration", ChecklistItemName.GitlabIntegration},
            {"Unity Cloud Build Target Config", ChecklistItemName.UnityCloudBuildTargetConfig},
            {"Tracking Event List", ChecklistItemName.TrackingEventList},
            {"Facebook ID", ChecklistItemName.FacebookID},
            {"Firebase Config File IOS", ChecklistItemName.FirebaseConfigFileIOS},
            {"Firebase Config File Android", ChecklistItemName.FirebaseConfigFileAndroid},
            {"Kochava ID IOS", ChecklistItemName.KochavaIDIOS},
            {"Kochava ID Android", ChecklistItemName.KochavaIDAndroid},
            {"Appodeal ID IOS", ChecklistItemName.AppodealIDIOS},
            {"Appodeal ID Android", ChecklistItemName.AppodealIDAndroid},
            {"Admob ID IOS", ChecklistItemName.AdmobIDIOS},
            {"Admob ID Android", ChecklistItemName.AdmobIDAndroid},
            {"Marketing Checklist", ChecklistItemName.MarketingChecklist},
            {"IAP Checklist", ChecklistItemName.IAPChecklist},
            {"IAP IOS list", ChecklistItemName.IAPIOSList},
            {"IAP Android list", ChecklistItemName.IAPAndroidList},
            {"Vị trí đặt quảng cáo", ChecklistItemName.AdPosition},
            {"Applovin IOS ID", ChecklistItemName.ApplovinIOSID},
            {"Applovin Android ID", ChecklistItemName.ApplovinAndroidID},
            {"Applovin IOS Inter ID", ChecklistItemName.ApplovinIOSInterID},
            {"Applovin IOS Banner ID", ChecklistItemName.ApplovinIOSBannerID},
            {"Applovin IOS Reward ID", ChecklistItemName.ApplovinIOSRewardID},
            {"Applovin Android Inter ID", ChecklistItemName.ApplovinAndroidInterID},
            {"Applovin Android Banner ID", ChecklistItemName.ApplovinAndroidBannerID},
            {"Applovin Android Reward ID", ChecklistItemName.ApplovinAndroidRewardID},
            {"Iron Source IOS ID", ChecklistItemName.IronSourceIOSID},
            {"Iron Source Android ID", ChecklistItemName.IronSourceAndroidID},
        };

        public static Dictionary<ChecklistItemName, string> dictionaryCheckListValue =
            new Dictionary<ChecklistItemName, string>();

        private void Awake()
        {
            var savedUrl = PlayerPrefs.GetString(PROJECT_CHECKLIST_URL); 
            if(!string.IsNullOrEmpty(savedUrl)) m_GoogleSheetUrl = savedUrl;
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
                    CreateKeyStore("thanhdeptrai");
                });
                
                GUILayout.Space(20);
                CreateGuiButton("Test Add Define PluginFacebook", () =>
                {
                    AddDefineSymbol(PluginDefineSymbol.PluginFacebook);
                }, 220);
                
                GUILayout.Space(20);
                CreateGuiButton("Test Remove Define PluginFacebook", () =>
                {
                    RemoveDefineSymbol(PluginDefineSymbol.PluginFacebook);
                }, 220);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            GUILayout.Label("Install SDK", m_LabelStyle);
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();

            CreateGuiButton("Facebook SDK (latest)", () =>
            {
                DownloadPackage(PluginUrl.FACEBOOK_SDK_CURRENT_VERSION_URL);
                AddDefineSymbol(PluginDefineSymbol.PluginFacebook);
            });

            GUILayout.Space(20);
            CreateGuiButton("Applovin SDK", () =>
            {
                DownloadPackage(PluginUrl.ApplovinSdkUrlByVersion(PluginUrl.ListApplovinVersion[m_PluginApplovinVersionSelect]));
                AddDefineSymbol(PluginDefineSymbol.PluginApplovin);
            });
            
            m_PluginApplovinVersionSelect = EditorGUILayout.Popup("", m_PluginApplovinVersionSelect, 
                PluginUrl.ListApplovinVersion, 
                GUILayout.Width(dropdownSize.x), GUILayout.Height(dropdownSize.y));
            
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();

            CreateGuiButton($"Kochava SDK {PluginUrl.KOCHAVA_CURRENT_VERSION}", () =>
            {
                DownloadPackage(PluginUrl.KochavaSdkUrlByVersion);
                AddDefineSymbol(PluginDefineSymbol.PluginKochava);
            });
            
            GUILayout.Space(20);
            CreateGuiButton($"Firebase SDK", () =>
            {
                DownloadPackage(PluginUrl.FirebaseSdkUrlByVersion(PluginUrl.ListFirebaseVersion[m_PluginFirebaseVersionSelect]));
                AddDefineSymbol(PluginDefineSymbol.PluginFirebase);
                AddDefineSymbol(PluginDefineSymbol.PluginFirebaseAnalytic);
            });
            
            m_PluginFirebaseVersionSelect = EditorGUILayout.Popup("", m_PluginFirebaseVersionSelect, 
                PluginUrl.ListFirebaseVersion, GUILayout.Width(dropdownSize.x), GUILayout.Height(dropdownSize.y));
            
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();

            CreateGuiButton("Update Main Template and Android Resolve", OnClickUpdateMainTemplateAndResolve, buttonSize.x * 2);
            
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

            if (m_SetSdkLog.Count == 0)
            {
                var savedLogData = PlayerPrefs.GetString(LOG_SET_ID_SDK);
                if(!string.IsNullOrEmpty(savedLogData)) m_SetSdkLog = JsonConvert.DeserializeObject<List<LogData>>(savedLogData);   
            }
            m_SetSdkLog.ForEach(logData =>
            {
                var guiStyle = GUI.skin.textArea;
                guiStyle.normal.textColor = guiStyle.focused.textColor = Color.black;
                if(logData.logType == LogType.Warning) guiStyle.normal.textColor = guiStyle.focused.textColor = new Color(1f, 0.5f, 0f);
                if(logData.logType == LogType.Error) guiStyle.normal.textColor = guiStyle.focused.textColor = Color.red;
                EditorGUILayout.TextArea(logData.log, guiStyle);
            });
        }

        private void CreateGuiButton(string text, Action onClickButton) => CreateGuiButton(text, onClickButton, buttonSize.x);
        private void CreateGuiButton(string text, Action onClickButton, float buttonWidth)
        {
            if (!GUILayout.Button(text, GUILayout.Width(buttonWidth), GUILayout.Height(buttonSize.y))) return;
            ClearLog();
            onClickButton?.Invoke();
        }

        private void CreateKeyStore(string keystoreName)
        {
            var keystoreFileName = $"{keystoreName}.keystore";
            var jdkPath = EditorPrefs.GetString("JdkPath");
            
            var osName = "MacOS";
            if (Application.platform.ToString().Contains("Windows")) osName = "Windows";
            if (Application.platform.ToString().Contains("Linux")) osName = "Linux";
            
            if (string.IsNullOrEmpty(jdkPath) || EditorPrefs.GetInt("JdkUseEmbedded") == 1)
            {
                var jdkPath2019_2 = $"{EditorApplication.applicationContentsPath}/PlaybackEngines/AndroidPlayer/Tools/OpenJDK/{osName}";
                var jdkPath2019_3_Or_Newer = $"{EditorApplication.applicationContentsPath}/PlaybackEngines/AndroidPlayer/OpenJDK";

                if (Directory.Exists(jdkPath2019_2)) jdkPath = jdkPath2019_2;
                else if (Directory.Exists(jdkPath2019_3_Or_Newer)) jdkPath = jdkPath2019_3_Or_Newer;
                else if (!Directory.Exists(jdkPath))
                {
                    AddLogError($"Key Store Error: Cannot find keytool in following folder: " +
                                $"\nJDKPath: {jdkPath}" +
                                $"\nJDKPath Unity 2019.2: {jdkPath2019_2}" + 
                                $"\nJDKPath Unity 2019.3 or Newer: {jdkPath2019_3_Or_Newer}");
                    return;
                }
            }
            var keyToolPath = $"{jdkPath.Replace(" ", "\" \"")}/bin/keytool";
            var processInfo = new ProcessStartInfo
            {
                FileName= @"cmd.exe",
                Arguments = $"/C {keyToolPath}",
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                RedirectStandardError = true
            };

            if (osName != "Windows")
            {
                processInfo.FileName = $"{keyToolPath}";
                processInfo.Arguments = "";
            }

            processInfo.Arguments += " -genkey -noprompt -v "
                + "-dname \"CN=, OU=, O=, L=, S=, C=\" "
                + $"-keystore {keystoreFileName} "
                + $"-storepass {keystoreName} "
                + $"-alias {keystoreName} "
                + $"-keypass {keystoreName} "
                + "-keyalg RSA -keysize 2048 -validity 10000";
                
            using(var process = new Process()) {
                process.StartInfo = processInfo;
                process.Start();

                // Add this: wait until process does its work
                process.WaitForExit();

                // and only then read the result
                var standardOutput = process.StandardOutput.ReadToEnd(); 
                var standardError = process.StandardError.ReadToEnd();
                Debug.Log($"StandardOutput: {standardOutput}");
                Debug.Log($"StandardError: {standardError}");
                
                if (standardError.Contains($"[Storing {keystoreFileName}]"))
                {
                    AddLog($"Keystore Created: {keystoreFileName}");
                }
                else if (standardOutput.Contains($"Key pair not generated, alias <{keystoreName}> already exists"))
                {
                    AddLogWarning($"Keystore Already Exists: {keystoreFileName}");
                }
                else
                {
                    AddLogError($"Key Store Creation Error: \nStandardOutput: {standardOutput}\nStandardError: {standardError}");   
                }
            }
        }

        private void OnClickUpdateMainTemplateAndResolve()
        {
            EnableJetifier();
            UpdateMultidexInMainTemplateAndAndroidManifest();
            GooglePlayServicesForceResolve();
        }

        private void UpdateMultidexInMainTemplateAndAndroidManifest()
        {
            var mainTemplateDirectory = "Assets/Plugins/Android/";
            var mainTemplateFileName = $"{mainTemplateDirectory}mainTemplate.gradle";
            
            var androidManifestDirectory = "Assets/Plugins/Android/";
            var androidManifestFileName = $"{androidManifestDirectory}AndroidManifest.xml";
            
            if (File.Exists(mainTemplateFileName))
            {
                AddLogWarning("mainTemplate.gradle already exists");
            }
            else
            {
                Directory.CreateDirectory(mainTemplateDirectory);
                FileUtil.CopyFileOrDirectory("Assets/_IECModules/Template/template.mainTemplate.gradle", mainTemplateFileName);
                AddLog("mainTemplate.gradle created");
            }
            
            if (File.Exists(androidManifestFileName))
            {
                AddLogWarning("AndroidManifest.xml already exists");
            }
            else
            {
                Directory.CreateDirectory(androidManifestDirectory);
                FileUtil.CopyFileOrDirectory("Assets/_IECModules/Template/template.AndroidManifest.xml", androidManifestFileName);
                AddLog("AndroidManifest.xml created");
            }
            
            var mainTemplateFileContent = File.ReadAllText(mainTemplateFileName);
            var androidManifestFileContent = File.ReadAllText(androidManifestFileName);
            var multiDexAlreadyEnabled = true;
            
            if (!mainTemplateFileContent.Contains("multiDexEnabled true"))
            {
                multiDexAlreadyEnabled = false;
                mainTemplateFileContent = mainTemplateFileContent.Replace(
                    "        versionName '**VERSIONNAME**'" , 
                    "        versionName '**VERSIONNAME**'\n        multiDexEnabled true");
                File.WriteAllText(mainTemplateFileName, mainTemplateFileContent);
                AddLog("mainTemplate.gradle Updated: multiDexEnabled true");
            }
            
            if (!mainTemplateFileContent.Contains("implementation 'androidx.multidex:multidex:2.0.1'"))
            {
                multiDexAlreadyEnabled = false;
                mainTemplateFileContent = mainTemplateFileContent.Replace(
                    "\ndependencies {" , 
                    "\ndependencies {\n	implementation 'androidx.multidex:multidex:2.0.1'");
                File.WriteAllText(mainTemplateFileName, mainTemplateFileContent);
                AddLog("mainTemplate.gradle Updated: implementation 'androidx.multidex:multidex:2.0.1'");
            }

            if (!androidManifestFileContent.Contains("android:name=\"androidx.multidex.MultiDexApplication\""))
            {
                multiDexAlreadyEnabled = false;
                var applicationTag = androidManifestFileContent.GetBetweenString("<application", ">");
                var applicationTagReplaced = applicationTag.Replace(
                    ">" , 
                    " android:name=\"androidx.multidex.MultiDexApplication\">");
                androidManifestFileContent = androidManifestFileContent.Replace(applicationTag, applicationTagReplaced);
                File.WriteAllText(androidManifestFileName, androidManifestFileContent);
                AddLog("AndroidManifest.xml Updated: implementation 'androidx.multidex:multidex:2.0.1'");
            }
            
            if (multiDexAlreadyEnabled)
            {
                AddLog("MultiDex already enabled in mainTemplate.gradle and AndroidManifest.xml");
            }
        }

        private void EnableJetifier()
        {
            const string gvhProjectSettingFile = "ProjectSettings/GvhProjectSettings.xml";
            if (!File.Exists(gvhProjectSettingFile))
            {
                AddLogError($"{gvhProjectSettingFile} Not Found!");
                return;
            }

            var gvhFileContent = File.ReadAllText(gvhProjectSettingFile);
            if (gvhFileContent.Contains("<projectSetting name=\"GooglePlayServices.UseJetifier\" value=\"True\" />"))
            {
                AddLog("Jetifier already enabled!");
                return;
            }
            if (gvhFileContent.Contains("<projectSetting name=\"GooglePlayServices.UseJetifier\" value=\"False\" />"))
            {
                gvhFileContent = gvhFileContent.Replace(
                    "<projectSetting name=\"GooglePlayServices.UseJetifier\" value=\"False\" />",
                    "<projectSetting name=\"GooglePlayServices.UseJetifier\" value=\"True\" />");
                File.WriteAllText(gvhProjectSettingFile, gvhFileContent);
                AddLog("Jetifier switched from FALSE to TRUE!");
                return;
            }
            
            gvhFileContent = gvhFileContent.Replace(
                "\n</projectSettings>",
                "  <projectSetting name=\"GooglePlayServices.UseJetifier\" value=\"True\" />\n</projectSettings>");
            File.WriteAllText(gvhProjectSettingFile, gvhFileContent);
            AddLog("Jetifier added to True!");
        }
        
        private void GooglePlayServicesForceResolve()
        {
            AddLog("Android Play Service Start Force Resolve");
            
            var googlePlayServicesNameSpace = "GooglePlayServices";
            var googlePlayServicesClassName = "PlayServicesResolver";
            var googlePlayServicesMethodForceResolve = "MenuForceResolve";

            var googlePlayServicesClassType = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                from type in assembly.GetTypes()
                where type.Namespace == googlePlayServicesNameSpace && type.Name == googlePlayServicesClassName
                select type).FirstOrDefault();
            
            if (googlePlayServicesClassType == null)
            {
                AddLogError($"{googlePlayServicesNameSpace}.{googlePlayServicesClassName} NOT Defined.");
                return;
            }
            var googlePlayServicesMethodForceResolveMethod = googlePlayServicesClassType.GetMethod(googlePlayServicesMethodForceResolve);
            if (googlePlayServicesMethodForceResolveMethod == null)
            {
                AddLogError($"Method {googlePlayServicesMethodForceResolve} NOT Exist!");
                return;
            }
            googlePlayServicesMethodForceResolveMethod.Invoke(null, null);
        }
            
        private void OnClickSetSDKId()
        {
            ClearLog();
            
            PlayerPrefs.SetString(PROJECT_CHECKLIST_URL, m_GoogleSheetUrl);

            var checklistData = DownloadTextFromUrl(GoogleSheetToCSV.ParseGoogleSheetUrl(m_GoogleSheetUrl));

            ApplySdkSetting(GoogleSheetToCSV.ParseCSV(checklistData, false));
        }

        private void DownloadPackage(string url, Action onComplete  = null)
        {
            AddLog($"Start Download SDK: {url}");

            HTTPManager.Setup();
            var httpRequest = new HTTPRequest(new Uri(url), (originalRequest, response) =>
            {
                EditorUtility.ClearProgressBar();
                onComplete?.Invoke();
                
                AddLog($"Download SDK Complete: {BytesToString(response.Data.Length)}");
                // using (var fs = File.Create("Assets/facebook.zip")) 
                // {
                //     fs.Write(response.Data, 0, response.Data.Length);
                // }
                
                // var byteArray = File.ReadAllBytes("Assets/facebook.zip");
                // AddLog($"Assets/facebook.zip: {byteArray.Length} bytes");

                if (url.EndsWith(".unitypackage"))
                {
                    ImportPackage(response.Data, url);
                }
                else
                {
                    ZipUtils.GetZipFileFromByteArray(response.Data, zipFile =>
                    {
                        foreach (ZipEntry entry in zipFile)
                        {
                            if(entry.IsDirectory) continue;
                            if(!entry.Name.EndsWith(".unitypackage")) continue;
                            AddLog(entry.Name);
                            var zippedStream = zipFile.GetInputStream(entry);
                            var fileByteArray = ZipUtils.ReadFully(zippedStream);
                            
                            var filePath = $"{GetTempFolderPath()}{entry.Name.Split('/').Last()}";
                        
                            AddLog($"Start Unzip To: {filePath}");

                            ImportPackage(fileByteArray, entry.Name);
                        
                            break;
                        }

                    });
                }
            }).Send();
            httpRequest.OnProgress += (request, downloaded, length) =>
            {
                EditorUtility.DisplayProgressBar("Downloading", $"{BytesToString(downloaded)}/{BytesToString(length)}", (float) downloaded/ length);
            }; 
        }

        public static string BytesToString(long byteCount)
        {
            string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB
            if (byteCount == 0)
                return "0" + suf[0];
            var bytes = Math.Abs(byteCount);
            var place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            var num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return (Math.Sign(byteCount) * num) + suf[place];
        }

        private void ImportPackage(byte[] packageByteArray, string packageName)
        {
            var fileName = GetFileNameFromURL(packageName);
            var filePath = $"{GetTempFolderPath()}{fileName}";
            using (var fs = File.Create(filePath)) 
            {
                fs.Write(packageByteArray, 0, packageByteArray.Length);
            }
                        
            AddLog($"Waiting Import Package");

            AssetDatabase.importPackageStarted += ImportPackageStart;
            AssetDatabase.importPackageCompleted += ImportPackageComplete;
            AssetDatabase.ImportPackage(filePath, false);

        }

        private string GetFileNameFromURL(string url)
        {
            var fileName = url.Split('/').Last();
            fileName = fileName.Split(new[] { "%2F" }, StringSplitOptions.None).Last();
            fileName = fileName.Split(new[] { "=" }, StringSplitOptions.None).Last();
            return fileName;
        }

        private string GetTempFolderPath()
        {
            var consoleLogFile = new FileInfo(Application.consoleLogPath);
            var tempFolderPath = $"{consoleLogFile.Directory}/Temp/";
            Directory.CreateDirectory(tempFolderPath); 
            return tempFolderPath;
        }

        private void ImportPackageComplete(string packageName)
        {
            AddLog($"Import Package Done: {packageName}");
            AssetDatabase.importPackageCompleted -= ImportPackageComplete;
        }
        private void ImportPackageStart(string packageName)
        {
            AddLog($"Import Package Start: {packageName}");
            AssetDatabase.importPackageStarted -= ImportPackageStart;
        }
        private byte[] DownloadDataFromUrl(string url)
        {
            var uwr = DownloadFromUrl(url);
            var data = uwr.downloadHandler.data;
            uwr.Dispose();
            return data;
        }
        private string DownloadTextFromUrl(string url)
        {
            var uwr = DownloadFromUrl(url);
            var text = uwr.downloadHandler.text;
            uwr.Dispose();
            return text;
        }
        private UnityWebRequest DownloadFromUrl(string url)
        {
            var uwr = UnityWebRequest.Get(url);
            
            // uwr.SetRequestHeader("accept","text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9" ); 
            // // uwr.SetRequestHeader("accept-encoding", "gzip, deflate, br");
            // uwr.SetRequestHeader("accept-language", "ru-RU,ru;q=0.9,en-US;q=0.8,en;q=0.7");

            uwr.SendWebRequest();
            while (!uwr.isDone)
            {
                // Debug.Log(uwr.downloadProgress);
            }

            if (uwr.isNetworkError)
            {
                Debug.LogError("Download Error");
                AddLogError($"Downloading: {url}");
            }
            return uwr;
        }
        

        private void ApplySdkSetting(List<List<string>> sdkData)
        {

            dictionaryCheckListValue = sdkData.Where(checklistKey => _checklistKey.ContainsKey(checklistKey[0]))
                .ToDictionary(checklistKey => _checklistKey[checklistKey[0]], checklistKey => checklistKey[1]);


#if PluginFacebook
            ServicesConfig serviceConfig;
            try
            {
                var assets = AssetDatabase.FindAssets("t:ServicesConfig");
                var serviceConfigPath = AssetDatabase.GUIDToAssetPath(assets[0]);
                serviceConfig = AssetDatabase.LoadAssetAtPath<ServicesConfig>(serviceConfigPath);
                AddLog("Service Config Load Success: " + serviceConfigPath);
            }
            catch (Exception e)
            {
                AddLog("Error Loading Service Config: " + e.Message + "\n" + e.StackTrace);
                throw;
            }
#else
            AddLogError("PluginFacebook NOT Defined!!!!!");
#endif

            foreach (var keyValuePair in dictionaryCheckListValue)
            {
                var checklistItemName = keyValuePair.Key;
                var checklistItemValue = keyValuePair.Value;
                switch (checklistItemName)
                {
                    case ChecklistItemName.ProductName:
                        var productName = checklistItemValue;
                        AddLog($"Product Name: {productName}");
                        PlayerSettings.productName = productName;
                        break;
                    case ChecklistItemName.BundleID:
                        var bundleId = checklistItemValue;
                        AddLog($"Bundle ID: {bundleId}");
                        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Standalone, bundleId);
                        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, bundleId);
                        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, bundleId);

                        if (bundleId.ToLower().Contains(".gma.")) PlayerSettings.companyName = "GMA";
                        else if (bundleId.ToLower().Contains(".iec.")) PlayerSettings.companyName = "IEC";
                        else if (bundleId.ToLower().Contains(".dta.")) PlayerSettings.companyName = "DTA";
                        else
                        {
                            AddLogError("Unknown Company Name");
                        }
                        break;
                    case ChecklistItemName.KeyStoreName:
                        if(!string.IsNullOrEmpty(checklistItemValue)) CreateKeyStore(checklistItemValue);
                        break;
                    case ChecklistItemName.KanboardSwimlane:
                        break;
                    case ChecklistItemName.DiscordChannel:
                        break;
                    case ChecklistItemName.FirebaseProject:
                        break;
                    case ChecklistItemName.UnityCloudProject:
                        break;
                    case ChecklistItemName.UnityCloudBuildKanboardWebhooks:
                        break;
                    case ChecklistItemName.UnityCloudBuildDiscordWebhooks:
                        break;
                    case ChecklistItemName.GitlabProject:
                        break;
                    case ChecklistItemName.GitlabIntegration:
                        break;
                    case ChecklistItemName.UnityCloudBuildTargetConfig:
                        break;
                    case ChecklistItemName.TrackingEventList:
                        break;
#if PluginFacebook
                    case ChecklistItemName.FacebookID:
                        var facebookId = checklistItemValue;
                        AddLog($"Facebook ID: {facebookId}");
                        if (Facebook.Unity.Settings.FacebookSettings.NullableInstance == null)
                        {
                            var instance = CreateInstance<Facebook.Unity.Settings.FacebookSettings>();
                            var path1 = Path.Combine(Application.dataPath, "FacebookSDK/SDK/Resources");
                            if (!Directory.Exists(path1))
                                Directory.CreateDirectory(path1);
                            var path2 = Path.Combine(Path.Combine("Assets", "FacebookSDK/SDK/Resources"), "FacebookSettings.asset");
                            AssetDatabase.CreateAsset(instance, path2);
                        }
                        Selection.activeObject = Facebook.Unity.Settings.FacebookSettings.Instance;
                        Facebook.Unity.Settings.FacebookSettings.AppIds = new List<string> {facebookId};
                        Facebook.Unity.Settings.FacebookSettings.AppLabels = new List<string> {PlayerSettings.productName};
                        EditorUtility.SetDirty(Facebook.Unity.Settings.FacebookSettings.Instance);
                        Facebook.Unity.Editor.ManifestMod.GenerateManifest();
                        break;
                    case ChecklistItemName.FirebaseConfigFileIOS:
                        break;
                    case ChecklistItemName.FirebaseConfigFileAndroid:
                        break;
                    case ChecklistItemName.KochavaIDIOS:
                        if(!string.IsNullOrEmpty(checklistItemValue)) AddLog($"Kochava ID IOS: {checklistItemValue}");
                        serviceConfig.kochavaConfig.iosID = checklistItemValue;
                        break;
                    case ChecklistItemName.KochavaIDAndroid:
                        if(!string.IsNullOrEmpty(checklistItemValue)) AddLog($"Kochava ID Android: {checklistItemValue}");
                        serviceConfig.kochavaConfig.androidID = checklistItemValue;
                        break;
                    case ChecklistItemName.AppodealIDIOS:
                        if(!string.IsNullOrEmpty(checklistItemValue)) AddLog($"Appodeal ID IOS: {checklistItemValue}");
                        serviceConfig.AppodealConfig.android.appID = checklistItemValue;
                        break;
                    case ChecklistItemName.AppodealIDAndroid:
                        if(!string.IsNullOrEmpty(checklistItemValue)) AddLog($"Appodeal ID Android: {checklistItemValue}");
                        serviceConfig.AppodealConfig.android.appID = checklistItemValue;
                        break;
                    case ChecklistItemName.AdmobIDIOS:
                        break;
                    case ChecklistItemName.AdmobIDAndroid:
                        break;
                    case ChecklistItemName.MarketingChecklist:
                        break;
                    case ChecklistItemName.IAPChecklist:
                        break;
                    case ChecklistItemName.IAPIOSList:
                        break;
                    case ChecklistItemName.IAPAndroidList:
                        break;
                    case ChecklistItemName.AdPosition:
                        break;
                    case ChecklistItemName.ApplovinIOSID:
                        if(!string.IsNullOrEmpty(checklistItemValue)) AddLog($"Applovin IOS ID: {checklistItemValue}");
                        serviceConfig.ApplovinConfig.ios.appID = checklistItemValue;
                        break;
                    case ChecklistItemName.ApplovinAndroidID:
                        if(!string.IsNullOrEmpty(checklistItemValue)) AddLog($"Applovin Android ID: {checklistItemValue}");
                        serviceConfig.ApplovinConfig.android.appID = checklistItemValue;
                        break;
                    case ChecklistItemName.ApplovinIOSInterID:
                        if(!string.IsNullOrEmpty(checklistItemValue)) AddLog($"Applovin IOS Inter ID: {checklistItemValue}");
                        serviceConfig.ApplovinConfig.ios.interstitialID = checklistItemValue;
                        break;
                    case ChecklistItemName.ApplovinIOSBannerID:
                        if(!string.IsNullOrEmpty(checklistItemValue)) AddLog($"Applovin IOS Banner ID: {checklistItemValue}");
                        serviceConfig.ApplovinConfig.ios.bannerID = checklistItemValue;
                        break;
                    case ChecklistItemName.ApplovinIOSRewardID:
                        if(!string.IsNullOrEmpty(checklistItemValue)) AddLog($"Applovin IOS Reward ID: {checklistItemValue}");
                        serviceConfig.ApplovinConfig.ios.rewardID = checklistItemValue;
                        break;
                    case ChecklistItemName.ApplovinAndroidInterID:
                        if(!string.IsNullOrEmpty(checklistItemValue)) AddLog($"Applovin Android Inter ID: {checklistItemValue}");
                        serviceConfig.ApplovinConfig.android.interstitialID = checklistItemValue;
                        break;
                    case ChecklistItemName.ApplovinAndroidBannerID:
                        if(!string.IsNullOrEmpty(checklistItemValue)) AddLog($"Applovin Android Banner ID: {checklistItemValue}");
                        serviceConfig.ApplovinConfig.android.bannerID = checklistItemValue;
                        break;
                    case ChecklistItemName.ApplovinAndroidRewardID:
                        if(!string.IsNullOrEmpty(checklistItemValue)) AddLog($"Applovin Android Reward ID: {checklistItemValue}");
                        serviceConfig.ApplovinConfig.android.rewardID = checklistItemValue;
                        break;
                    case ChecklistItemName.IronSourceIOSID:
                        if(!string.IsNullOrEmpty(checklistItemValue)) AddLog($"Iron Source IOS ID: {checklistItemValue}");
                        serviceConfig.IronSourceConfig.ios.appID = checklistItemValue;
                        serviceConfig.IronSourceConfig.ios.interstitialID = checklistItemValue;
                        serviceConfig.IronSourceConfig.ios.bannerID = checklistItemValue;
                        serviceConfig.IronSourceConfig.ios.rewardID = checklistItemValue;
                        break;
                    case ChecklistItemName.IronSourceAndroidID:
                        if(!string.IsNullOrEmpty(checklistItemValue)) AddLog($"Iron Source Android ID: {checklistItemValue}");
                        serviceConfig.IronSourceConfig.android.appID = checklistItemValue;
                        serviceConfig.IronSourceConfig.android.interstitialID = checklistItemValue;
                        serviceConfig.IronSourceConfig.android.bannerID = checklistItemValue;
                        serviceConfig.IronSourceConfig.android.rewardID = checklistItemValue;
                        break;
#endif
                }
            }
            
#if PluginFacebook
            EditorUtility.SetDirty(serviceConfig);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
#endif
        }

        private void AddLogError(string log)
        {
            AddLog($"Error: {log}", LogType.Error);
        }
        private void AddLogWarning(string log)
        {
            AddLog($"Warning: {log}", LogType.Warning);
        }
        private void AddLog(object log)
        {
            AddLog(log.ToString());
        }
        private void AddLog(string log, LogType logType = LogType.Debug)
        {
            var logStr = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {log}";
            if (m_SetSdkLog.Count > 0 && m_SetSdkLog.Last().logType == LogType.Debug && logType == LogType.Debug)
            {
                m_SetSdkLog.Last().log += $"\n{logStr}";
            }
            else {
                m_SetSdkLog.Add(new LogData
                {
                    log = logStr,
                    logType = logType
                });
            }
            SaveLogToPlayerPref();
        }

        private void ClearLog()
        {
            m_SetSdkLog?.Clear();
            SaveLogToPlayerPref();
        }

        private void SaveLogToPlayerPref()
        {
            PlayerPrefs.SetString(LOG_SET_ID_SDK, JsonConvert.SerializeObject(m_SetSdkLog));
        }

        private static void AddDefineSymbol(PluginDefineSymbol defineSymbol)
        {
            ScriptingDefineSymbolEditor.AddDefineSymbol(defineSymbol.ToString());
        }
        private static void RemoveDefineSymbol(PluginDefineSymbol defineSymbol)
        {
            ScriptingDefineSymbolEditor.RemoveDefineSymbol(defineSymbol.ToString());
        }
    }

    public class LogData
    {
        public string log;
        public LogType logType = LogType.Debug;
    }

    public enum LogType
    {
        Debug,
        Warning,
        Error
    }
}

