using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IEC.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace _IECModules
{
    public static class ProjectResolveHelper
    {
        public static readonly List<string> listPermissionRemove = new List<string>()
        {
            "android.permission.WRITE_EXTERNAL_STORAGE"
        };

        public static readonly List<string> listPermissionNecessary = new List<string>()
        {
            "com.google.android.gms.permission.AD_ID"
        };
        
        public static List<string> listObsoleteResolver = new List<string>
        {
            "Assets/ExternalDependencyManager",
            "Assets/PlayServicesResolver"
        };

        public static List<string> listLib = new List<string>
        {
            "AdditionalLibrary",
            "MaxMediationGoogle",
            "MaxMediationGoogleAdManager"
        };
        
        private static GUIStyle m_WarningTextStyle;
        private static string m_WarningString;
        private static DirectoryInfo m_DirectoryInfo = new FileInfo(EditorApplication.applicationPath).Directory;
        public static List<string> listTemplatePathCheck = new List<string>
        {
            $"{EditorApplication.applicationContentsPath}/PlaybackEngines/AndroidPlayer/Tools/GradleTemplates",
            $"{EditorApplication.applicationContentsPath}/PlaybackEngines/AndroidPlayer/GradleTemplates",
            $"{m_DirectoryInfo}/PlaybackEngines/AndroidPlayer/Tools/GradleTemplates",
            $"{m_DirectoryInfo}/PlaybackEngines/AndroidPlayer/GradleTemplates",

        };

        private static string m_TemplateFolderPath;
        public static string TemplateFolderPath => m_TemplateFolderPath ?? (m_TemplateFolderPath
                                                       = listTemplatePathCheck.FirstOrDefault(Directory.Exists));

        public static void EditorOnGUI()
        {
            if (m_WarningTextStyle == null)
            {
                m_WarningTextStyle = new GUIStyle(GUI.skin.textArea);
                m_WarningTextStyle.normal.textColor = m_WarningTextStyle.focused.textColor = new Color(1f, 0f, 0f);
            }

            m_WarningString = "";
            listObsoleteResolver.ForEach(path =>
            {
                if (Directory.Exists(path)) m_WarningString += $"\n{path} Exist! Please Install External Dependency Manager Above!";

            });
            if (!string.IsNullOrEmpty(m_WarningString)) EditorGUILayout.TextArea($"WARNING:{m_WarningString}", m_WarningTextStyle);
        }

        public static void ResolveProject()
        {
            //EnableJetifier();
            RemoveUnityAdsPackage();
            EnableJetifierNew();
            UpdateMainTemplateAndAndroidManifest();
            // GooglePlayServicesForceResolve();
            UpdateJetifierInMainTemplate();
            ModifyPermissions(listPermissionRemove,PermissionAction.Remove);
            ModifyPermissions(listPermissionNecessary,PermissionAction.Add);
            GenerateGameObjectOguryCallback();
            DeleteObsoleteResolver();
            //UpdateApplovinQualityService();
        }

        private static void GenerateGameObjectOguryCallback()
        {
#if PluginOgury
            var oguryCallbacks = GameObject.FindObjectOfType<OgurySdk.OguryCallbacks>();
            if(oguryCallbacks != null) return;
            new GameObject("OguryCallbacks").AddComponent<OgurySdk.OguryCallbacks>();
#endif
        }

        public static void DeleteObsoleteResolver()
        {
            listObsoleteResolver.ForEach(path =>
            {
                if (FileUtil.DeleteFileOrDirectory(path))
                {
                    FileUtil.DeleteFileOrDirectory($"{path}.meta");
                    LogEditorHelper.AddLog($"DELETED: {path}");
                }
            });
            AssetDatabase.Refresh();
        }

        private static void RemoveUnityAdsPackage()
        {
            LogEditorHelper.AddLog($"Remove Package: com.unity.ads");
            PackageManagerHelper.Remove("com.unity.ads", null, true);
        }

        private static void UpdateApplovinQualityService()
        {
#if PluginApplovin
            LogEditorHelper.AddLog("AppLovin Disable Quality Service");
            AppLovinSettings.Instance.QualityServiceEnabled = true;
            EditorUtility.SetDirty(AppLovinSettings.Instance);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
#endif
        }

        /// <summary>Update <c>mainTemplate.gradle</c> and <c>AndroidManifest.xml</c> </summary>
        /// Update Multidex in mainTemplate.gradle and AndroidManifest.xml
        /// Create network_security_config.xml and update in AndroidManifest.xml

        private static void UpdateMainTemplateAndAndroidManifest()
        {
            try
            {
                var manifestAndMainTemplateDirectory = "Assets/Plugins/Android/";
                var mainTemplateFileName = $"{manifestAndMainTemplateDirectory}mainTemplate.gradle";
                var launcherTemplateFileName = $"{manifestAndMainTemplateDirectory}launcherTemplate.gradle";
                var baseTemplateFileName = $"{manifestAndMainTemplateDirectory}baseProjectTemplate.gradle";
                var androidManifestFileName = $"{manifestAndMainTemplateDirectory}AndroidManifest.xml";

                if (File.Exists(mainTemplateFileName))
                {
                    LogEditorHelper.AddLogWarning("mainTemplate.gradle already exists");
                }
                else
                {
                    Directory.CreateDirectory(manifestAndMainTemplateDirectory);
                    FileUtil.CopyFileOrDirectory($"{TemplateFolderPath}/mainTemplate.gradle", mainTemplateFileName);
                    LogEditorHelper.AddLog("mainTemplate.gradle created");
                }

                if (File.Exists(launcherTemplateFileName))
                {
                    LogEditorHelper.AddLogWarning("launcherTemplate.gradle already exists");
                }
                else
                {
                    Directory.CreateDirectory(manifestAndMainTemplateDirectory);
                    FileUtil.CopyFileOrDirectory($"{TemplateFolderPath}/launcherTemplate.gradle", launcherTemplateFileName);
                    LogEditorHelper.AddLog("launcherTemplate.gradle created");
                }

                if (File.Exists(baseTemplateFileName))
                {
                    LogEditorHelper.AddLogWarning("baseProjectTemplate.gradle already exists");
                }
                else
                {
                    Directory.CreateDirectory(manifestAndMainTemplateDirectory);
                    FileUtil.CopyFileOrDirectory($"{TemplateFolderPath}/baseProjectTemplate.gradle", baseTemplateFileName);
                    LogEditorHelper.AddLog("baseProjectTemplate.gradle created");
                }

                // if (File.Exists(networkSecurityConfigFileName))
                // {
                //     LogEditorHelper.AddLogWarning("network_security_config.xml already exists");
                // }
                // else
                // {
                //     Directory.CreateDirectory(networkSecurityConfigDirectory);
                //     FileUtil.CopyFileOrDirectory("Assets/_IECModules/Template/template.network_security_config.xml", networkSecurityConfigFileName);
                //     LogEditorHelper.AddLog("network_security_config created");
                // }
                
                CopyAndroidLibConfig();

                if (File.Exists(androidManifestFileName))
                {
                    LogEditorHelper.AddLogWarning("AndroidManifest.xml already exists");
                }
                else
                {
                    Directory.CreateDirectory(manifestAndMainTemplateDirectory);
                    FileUtil.CopyFileOrDirectory("Assets/_IECModules/Template/template.AndroidManifest.xml", androidManifestFileName);
                    LogEditorHelper.AddLog("AndroidManifest.xml created");
                }

                var mainTemplateFileContent = File.ReadAllText(mainTemplateFileName);
                var launcherTemplateFileContent = File.ReadAllText(launcherTemplateFileName);
                var baseTemplateFileContent = File.ReadAllText(baseTemplateFileName);
                var androidManifestFileContent = File.ReadAllText(androidManifestFileName);

                if (baseTemplateFileContent.Contains("com.android.tools.build:gradle:3.4.0"))
                {
                    baseTemplateFileContent = baseTemplateFileContent.Replace("com.android.tools.build:gradle:3.4.0",
                        "com.android.tools.build:gradle:3.4.3");
                    File.WriteAllText(baseTemplateFileName, baseTemplateFileContent);
                    LogEditorHelper.AddLog("baseProjectTemplate.gradle Updated: com.android.tools.build:gradle:3.4.3");
                }

                var multiDexAlreadyEnabled = true;

                if (!mainTemplateFileContent.Contains("multiDexEnabled true"))
                {
                    multiDexAlreadyEnabled = false;
                    mainTemplateFileContent = mainTemplateFileContent.Replace(
                        "        versionName '**VERSIONNAME**'",
                        "        versionName '**VERSIONNAME**'\n        multiDexEnabled true");
                    File.WriteAllText(mainTemplateFileName, mainTemplateFileContent);
                    LogEditorHelper.AddLog("mainTemplate.gradle Updated: multiDexEnabled true");
                }

                if (!mainTemplateFileContent.Contains("implementation 'androidx.multidex:multidex:2.0.1'"))
                {
                    multiDexAlreadyEnabled = false;
                    mainTemplateFileContent = mainTemplateFileContent.Replace(
                        "\ndependencies {",
                        "\ndependencies {\n	implementation 'androidx.multidex:multidex:2.0.1'");
                    File.WriteAllText(mainTemplateFileName, mainTemplateFileContent);
                    LogEditorHelper.AddLog("mainTemplate.gradle Updated: implementation 'androidx.multidex:multidex:2.0.1'");
                }

                if (!launcherTemplateFileContent.Contains("multiDexEnabled true"))
                {
                    multiDexAlreadyEnabled = false;
                    launcherTemplateFileContent = launcherTemplateFileContent.Replace(
                        "        versionName '**VERSIONNAME**'",
                        "        versionName '**VERSIONNAME**'\n        multiDexEnabled true");
                    File.WriteAllText(launcherTemplateFileName, launcherTemplateFileContent);
                    LogEditorHelper.AddLog("launcherTemplate.gradle Updated: multiDexEnabled true");
                }

                if (!launcherTemplateFileContent.Contains("implementation 'androidx.multidex:multidex:2.0.1'"))
                {
                    multiDexAlreadyEnabled = false;
                    launcherTemplateFileContent = launcherTemplateFileContent.Replace(
                        "\ndependencies {",
                        "\ndependencies {\n	implementation 'androidx.multidex:multidex:2.0.1'");
                    File.WriteAllText(launcherTemplateFileName, launcherTemplateFileContent);
                    LogEditorHelper.AddLog("launcherTemplate.gradle Updated: implementation 'androidx.multidex:multidex:2.0.1'");
                }

                var androidManifestMultidexString = "android:name=\"androidx.multidex.MultiDexApplication\"";
                if (!androidManifestFileContent.Contains(androidManifestMultidexString))
                {
                    multiDexAlreadyEnabled = false;
                    var applicationTag = androidManifestFileContent.GetBetweenString("<application", ">");
                    var applicationTagReplaced = applicationTag.Replace(
                        ">",
                        $" {androidManifestMultidexString}>");
                    androidManifestFileContent = androidManifestFileContent.Replace(applicationTag, applicationTagReplaced);
                    File.WriteAllText(androidManifestFileName, androidManifestFileContent);
                    LogEditorHelper.AddLog($"AndroidManifest.xml Updated: {androidManifestMultidexString}");
                }

                var xmlnsTool = "xmlns:tools=\"http://schemas.android.com/tools\"";
                if (!androidManifestFileContent.Contains(xmlnsTool))
                {
                    var manifestTag = androidManifestFileContent.GetBetweenString("<manifest", ">");
                    var manifestTagReplaced = manifestTag.Replace(">", $" {xmlnsTool}>");
                    androidManifestFileContent = androidManifestFileContent.Replace(manifestTag, manifestTagReplaced);
                    File.WriteAllText(androidManifestFileName, androidManifestFileContent);
                    LogEditorHelper.AddLog($"AndroidManifest.xml Updated: {xmlnsTool}");
                }

                if (multiDexAlreadyEnabled)
                {
                    LogEditorHelper.AddLog("MultiDex already enabled in mainTemplate.gradle and AndroidManifest.xml");
                }

                var androidDebuggableStr = "android:debuggable=\"true\"";
                if (androidManifestFileContent.Contains(androidDebuggableStr))
                {
                    androidManifestFileContent = androidManifestFileContent.Replace(androidDebuggableStr, androidDebuggableStr.Replace("true", "false"));
                    File.WriteAllText(androidManifestFileName, androidManifestFileContent);
                    LogEditorHelper.AddLog($"AndroidManifest.xml Updated: android:debuggable Set to False");
                }

                var androidManifestNetworkSecurityConfigString = "android:networkSecurityConfig=\"@xml/network_security_config\"";
                if (androidManifestFileContent.Contains(androidManifestNetworkSecurityConfigString))
                {
                    // var applicationTag = androidManifestFileContent.GetBetweenString("<application", ">");
                    // var applicationTagReplaced = applicationTag.Replace(
                    //     ">",
                    //     $" {androidManifestNetworkSecurityConfigString}>");
                    // androidManifestFileContent = androidManifestFileContent.Replace(applicationTag, applicationTagReplaced);
                    androidManifestFileContent = androidManifestFileContent.Replace(androidManifestNetworkSecurityConfigString, "");
                    File.WriteAllText(androidManifestFileName, androidManifestFileContent);
                    LogEditorHelper.AddLog($"AndroidManifest.xml Removed: {androidManifestNetworkSecurityConfigString}");
                }
                else
                {
                    // LogEditorHelper.AddLog("network_security_config already enabled in AndroidManifest.xml");
                }
            }
            catch (Exception e)
            {
                LogEditorHelper.AddLogError($"UpdateMainTemplateAndAndroidManifest\n{e.Message}\n{e.StackTrace}");
            }
        }

        private static void CopyLib(string libName)
        {
            string pluginPath, templatePath, libraryPath;

            switch (libName)
            {
                case "MaxMediationGoogle":
                    pluginPath = "Assets/MaxSdk/Mediation/Google";
                    break;
                case "MaxMediationGoogleAdManager":
                    pluginPath = "Assets/MaxSdk/Mediation/GoogleAdManager";
                    break;
                case "AdditionalLibrary":
                    pluginPath = "Assets/Plugin/Android";
                    break;
                default:
                    pluginPath = "";
                    break;
            }
            
            templatePath = $"Assets/_IECModules/Template/{libName}.androidlib.template";
            libraryPath = $"Assets/Plugins/Android/{libName}.androidlib";
            
            if(!Directory.Exists(pluginPath)) return;

            if (Directory.Exists(libraryPath))
            {
                Directory.Delete(libraryPath, true);
                if (File.Exists($"{libraryPath}.meta"))
                {
                    File.Delete($"{libraryPath}.meta");
                }
            }
            
            Directory.CreateDirectory(libraryPath);
            var allDirs = 
                Directory.GetDirectories(templatePath, "*", SearchOption.AllDirectories);
            foreach (var dir in allDirs)
            {
                var dirCreate = dir.Replace(templatePath, libraryPath);
                Directory.CreateDirectory(dirCreate);
            }
                
            var allFiles = Directory.GetFiles(templatePath, "*", SearchOption.AllDirectories);
            foreach (var file in allFiles)
            {
                var fileCreate = file.Replace(templatePath, libraryPath);
                File.Copy(file, fileCreate, true);
            }
        }

        private static void CopyAndroidLibConfig()
        {
            var networkSecurityConfigDirectory = "Assets/Plugins/Android/res/xml/";
            var networkSecurityConfigFileName = $"{networkSecurityConfigDirectory}network_security_config.xml";
            
            // #if UNITY_2021_3_OR_NEWER

            var templatePath = "Assets/_IECModules/Template/AdditionalLibrary.androidlib.template";
            var libraryPath = "Assets/Plugins/Android/AdditionalLibrary.androidlib";
            
            if (Directory.Exists("Assets/Plugins/Android/res"))
            {
                Directory.Delete("Assets/Plugins/Android/res", true);
                File.Delete("Assets/Plugins/Android/res.meta");
            }
            
            if (Directory.Exists("Assets/Plugins/Android/myLibrary.androidlib"))
            {
                Directory.Delete("Assets/Plugins/Android/myLibrary.androidlib", true);
                File.Delete("Assets/Plugins/Android/myLibrary.androidlib");
            }
            
            listLib.Foreach(CopyLib);
        }

        private static void UpdateJetifierInMainTemplate()
        {
            var jetifierMainTemplateStr = "// Android Resolver Repos Start\n" +
            "([rootProject] + (rootProject.subprojects as List)).each {" + "\n" +
                "    ext {" + "\n" +
                "        it.setProperty(\"android.useAndroidX\", true)" + "\n" +
                "        it.setProperty(\"android.enableJetifier\", true)" + "\n" +
                "    }" + "\n" +
                "}" + "\n";

            var mainTemplateDirectory = "Assets/Plugins/Android/";
            var mainTemplateFileName = $"{mainTemplateDirectory}mainTemplate.gradle";

            if (!File.Exists(mainTemplateFileName))
            {
                LogEditorHelper.AddLogError("mainTemplate.gradle NOT exists!!");
                return;
            }

            var mainTemplateFileContent = File.ReadAllText(mainTemplateFileName);
            if (mainTemplateFileContent.Contains("it.setProperty(\"android.useAndroidX\", true")
                && mainTemplateFileContent.Contains("it.setProperty(\"android.enableJetifier\", true"))
            {
                LogEditorHelper.AddLog("Jetifier (androidx) already enabled in mainTemplate.gradle");
                return;
            }

            if (mainTemplateFileContent.Contains("// Android Resolver Repos Start\n"))
            {
                mainTemplateFileContent = mainTemplateFileContent.Replace("// Android Resolver Repos Start\n", jetifierMainTemplateStr);
                File.WriteAllText(mainTemplateFileName, mainTemplateFileContent);
                LogEditorHelper.AddLog("Jetifier (androidx) added in mainTemplate.gradle");
            }
            else
            {
                LogEditorHelper.AddLogError("Jetifier (androidx) NOT added in mainTemplate.gradle");
            }
        }

        /// <summary>Obsolete in Unity 2019.4 LTS</summary>
        [Obsolete]
        private static void EnableJetifier()
        {
            const string gvhProjectSettingFile = "ProjectSettings/GvhProjectSettings.xml";
            if (!File.Exists(gvhProjectSettingFile))
            {
                LogEditorHelper.AddLogError($"{gvhProjectSettingFile} Not Found!");
                return;
            }

            var gvhFileContent = File.ReadAllText(gvhProjectSettingFile);
            if (gvhFileContent.Contains("<projectSetting name=\"GooglePlayServices.UseJetifier\" value=\"True\" />"))
            {
                LogEditorHelper.AddLog("Jetifier (androidx) already enabled!");
                return;
            }
            if (gvhFileContent.Contains("<projectSetting name=\"GooglePlayServices.UseJetifier\" value=\"False\" />"))
            {
                gvhFileContent = gvhFileContent.Replace(
                    "<projectSetting name=\"GooglePlayServices.UseJetifier\" value=\"False\" />",
                    "<projectSetting name=\"GooglePlayServices.UseJetifier\" value=\"True\" />");
                File.WriteAllText(gvhProjectSettingFile, gvhFileContent);
                LogEditorHelper.AddLog("Jetifier (androidx) switched from FALSE to TRUE!");
                return;
            }

            gvhFileContent = gvhFileContent.Replace(
                "\n</projectSettings>",
                "  <projectSetting name=\"GooglePlayServices.UseJetifier\" value=\"True\" />\n</projectSettings>");
            File.WriteAllText(gvhProjectSettingFile, gvhFileContent);
            LogEditorHelper.AddLog("Jetifier (androidx) added to True!");
        }

        private static void EnableJetifierNew()
        {
            //Debug.LogError("TemplateFolderPath:" + TemplateFolderPath);
            //foreach (var path in listTemplatePathCheck)
            //{
            //    Debug.LogError("link " + path + " exist:" + Directory.Exists(path));
            //}

            var gradleDirectory = "Assets/Plugins/Android";
            var gradlePropertiesFile = $"{gradleDirectory}/gradleTemplate.properties";
            if (!File.Exists(gradlePropertiesFile))
            {
                Directory.CreateDirectory(gradleDirectory);
                FileUtil.CopyFileOrDirectory($"{TemplateFolderPath}/gradleTemplate.properties", gradlePropertiesFile);
                LogEditorHelper.AddLog("gradle.properties created");
            }
            else
            {
                LogEditorHelper.AddLogWarning("gradle.properties already exist");
            }

            var gradlePropertiesFileContent = File.ReadAllText(gradlePropertiesFile);

            if (!gradlePropertiesFileContent.Contains("android.useAndroidX=true")
            || !gradlePropertiesFileContent.Contains("android.enableJetifier=true"))
            {
                gradlePropertiesFileContent += "\nandroid.useAndroidX=true";
                gradlePropertiesFileContent += "\nandroid.enableJetifier=true";
                File.WriteAllText(gradlePropertiesFile, gradlePropertiesFileContent);
                LogEditorHelper.AddLog("android.useAndroidX=true & android.enableJetifier=true Added in gradle.properties");
            }
        }

        public static void GooglePlayServicesForceResolve()
        {
            LogEditorHelper.AddLog("Android Play Service Start Force Resolve");

            var googlePlayServicesNameSpace = "GooglePlayServices";
            var googlePlayServicesClassName = "PlayServicesResolver";
            var googlePlayServicesMethodForceResolve = "MenuForceResolve";

            var googlePlayServicesClassType = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                               from type in assembly.GetTypes()
                                               where type.Namespace == googlePlayServicesNameSpace && type.Name == googlePlayServicesClassName
                                               select type).FirstOrDefault();

            if (googlePlayServicesClassType == null)
            {
                LogEditorHelper.AddLogError($"{googlePlayServicesNameSpace}.{googlePlayServicesClassName} NOT Defined.");
                return;
            }
            var googlePlayServicesMethodForceResolveMethod = googlePlayServicesClassType.GetMethod(googlePlayServicesMethodForceResolve);
            if (googlePlayServicesMethodForceResolveMethod == null)
            {
                LogEditorHelper.AddLogError($"Method {googlePlayServicesMethodForceResolve} NOT Exist!");
                return;
            }
            googlePlayServicesMethodForceResolveMethod.Invoke(null, null);
        }
        
        public enum PermissionAction
        {
            Add,
            Remove
        }
        private static void ModifyPermissions(List<string> permissions, PermissionAction action)
        {
            var manifestPath = "Assets/Plugins/Android/AndroidManifest.xml";
            if (!File.Exists(manifestPath))
            {
                return;
            }

            var manifestContent = File.ReadAllText(manifestPath);
            var manifestTag = manifestContent.GetBetweenString("<manifest", "</manifest>",false);
            // Debug.LogError(manifestTag);
            var manifestTagReplace = manifestTag;
            for (int i = 0; i < permissions.Count; i++)
            {
                var permission = permissions[i];
                var permissionAction =
                    $"<uses-permission android:name=\"{permission}\"";
                if(action == PermissionAction.Remove) permissionAction += " tools:node=\"remove\"";
                permissionAction += "/>";
                if (!manifestTagReplace.Contains(permission))
                {
                    manifestTagReplace += $"{permissionAction}\n";
                }
                else if(action == PermissionAction.Remove)
                {
                    LogEditorHelper.AddLogWarning($"Manifest has {permission} permission for some reasons, please check manifest file");
                }
            }
            manifestContent = manifestContent.Replace(manifestTag, manifestTagReplace);
            File.WriteAllText(manifestPath,manifestContent);
        }
    }
}