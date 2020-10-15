using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace _IECModules
{
    public static class ProjectResolveHelper
    {
        public static List<string> listObsoleteResolver = new List<string>
        {
            "Assets/ExternalDependencyManager",
            "Assets/PlayServicesResolver"
        };
        private static GUIStyle m_WarningTextStyle;
        private static string m_WarningString;
        
        public static List<string> listTemplatePathCheck = new List<string>
        {
            $"{EditorApplication.applicationContentsPath}/PlaybackEngines/AndroidPlayer/Tools/GradleTemplates",
            $"{EditorApplication.applicationContentsPath}/PlaybackEngines/AndroidPlayer/GradleTemplates"
        };

        private static string m_TemplateFolderPath; 
        public static string TemplateFolderPath => m_TemplateFolderPath ?? (m_TemplateFolderPath 
                                                       = listTemplatePathCheck.FirstOrDefault(Directory.Exists)); 


        public static void EditorOnGUI()
        {
            if (m_WarningTextStyle == null)
            {
                m_WarningTextStyle = new GUIStyle(GUI.skin.textArea);
                m_WarningTextStyle.normal.textColor = m_WarningTextStyle.focused.textColor = new Color(1f, 0.5f, 0f);   
            }
            
            m_WarningString = "";
            listObsoleteResolver.ForEach(path =>
            {
                if (Directory.Exists(path)) m_WarningString += $"\n{path} Exist! Please Install External Dependency Manager Above!";

            });
            if(!string.IsNullOrEmpty(m_WarningString)) EditorGUILayout.TextArea($"WARNING:{m_WarningString}", m_WarningTextStyle);
        }
        
        public static void ResolveProject() 
        {
            //EnableJetifier();
            RemoveUnityAdsPackage();
            EnableJetifierNew();
            UpdateMainTemplateAndAndroidManifest();
            GooglePlayServicesForceResolve();
            UpdateJetifierInMainTemplate();
            UpdateApplovinQualityService();
        }

        public static void DeleteObsoleteResolver()
        {
            listObsoleteResolver.ForEach(path =>
            {
                if (FileUtil.DeleteFileOrDirectory(path))
                {
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
            AppLovinSettings.Instance.QualityServiceEnabled = false;
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
                var androidManifestFileName = $"{manifestAndMainTemplateDirectory}AndroidManifest.xml";

                var networkSecurityConfigDirectory = "Assets/Plugins/Android/res/xml/";
                var networkSecurityConfigFileName = $"{networkSecurityConfigDirectory}network_security_config.xml";

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

                if (File.Exists(networkSecurityConfigFileName))
                {
                    LogEditorHelper.AddLogWarning("network_security_config.xml already exists");
                }
                else
                {
                    Directory.CreateDirectory(networkSecurityConfigDirectory);
                    FileUtil.CopyFileOrDirectory("Assets/_IECModules/Template/template.network_security_config.xml", networkSecurityConfigFileName);
                    LogEditorHelper.AddLog("network_security_config created");
                }

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
                var androidManifestFileContent = File.ReadAllText(androidManifestFileName);
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

                if (multiDexAlreadyEnabled)
                {
                    LogEditorHelper.AddLog("MultiDex already enabled in mainTemplate.gradle and AndroidManifest.xml");
                }

                var androidManifestNetworkSecurityConfigString = "android:networkSecurityConfig=\"@xml/network_security_config\"";
                if (!androidManifestFileContent.Contains(androidManifestNetworkSecurityConfigString))
                {
                    var applicationTag = androidManifestFileContent.GetBetweenString("<application", ">");
                    var applicationTagReplaced = applicationTag.Replace(
                        ">",
                        $" {androidManifestNetworkSecurityConfigString}>");
                    androidManifestFileContent = androidManifestFileContent.Replace(applicationTag, applicationTagReplaced);
                    File.WriteAllText(androidManifestFileName, androidManifestFileContent);
                    LogEditorHelper.AddLog($"AndroidManifest.xml Updated: {androidManifestNetworkSecurityConfigString}");
                }
                else
                {
                    LogEditorHelper.AddLog("network_security_config already enabled in AndroidManifest.xml");
                }
            }
            catch (Exception e)
            {
                LogEditorHelper.AddLogError($"UpdateMainTemplateAndAndroidManifest\n{e.Message}\n{e.StackTrace}");
            }
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

        private static void GooglePlayServicesForceResolve()
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

    }
}