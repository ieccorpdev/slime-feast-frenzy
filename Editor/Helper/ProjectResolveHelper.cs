using System;
using System.IO;
using System.Linq;
using UnityEditor;

namespace _IECModules
{
    public static class ProjectResolveHelper
    {
        
        public static void UpdateMainTemplateAndResolve()
        {
            EnableJetifier();
            UpdateMainTemplateAndAndroidManifest();
            GooglePlayServicesForceResolve();
            UpdateJetifierInMainTemplate();
        }

        /// <summary>Update <c>mainTemplate.gradle</c> and <c>AndroidManifest.xml</c> </summary>
        /// Update Multidex in mainTemplate.gradle and AndroidManifest.xml
        /// Create network_security_config.xml and update in AndroidManifest.xml

        private static void UpdateMainTemplateAndAndroidManifest()
        {
            var mainTemplateDirectory = "Assets/Plugins/Android/";
            var mainTemplateFileName = $"{mainTemplateDirectory}mainTemplate.gradle";
            
            var androidManifestDirectory = "Assets/Plugins/Android/";
            var androidManifestFileName = $"{androidManifestDirectory}AndroidManifest.xml";
            
            var networkSecurityConfigDirectory = "Assets/Plugins/Android/res/xml/";
            var networkSecurityConfigFileName = $"{networkSecurityConfigDirectory}network_security_config.xml";
            
            if (File.Exists(mainTemplateFileName))
            {
                LogEditorHelper.AddLogWarning("mainTemplate.gradle already exists");
            }
            else
            {
                Directory.CreateDirectory(mainTemplateDirectory);
                FileUtil.CopyFileOrDirectory("Assets/_IECModules/Template/template.mainTemplate.gradle", mainTemplateFileName);
                LogEditorHelper.AddLog("mainTemplate.gradle created");
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
                Directory.CreateDirectory(androidManifestDirectory);
                FileUtil.CopyFileOrDirectory("Assets/_IECModules/Template/template.AndroidManifest.xml", androidManifestFileName);
                LogEditorHelper.AddLog("AndroidManifest.xml created");
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
                LogEditorHelper.AddLog("mainTemplate.gradle Updated: multiDexEnabled true");
            }
            
            if (!mainTemplateFileContent.Contains("implementation 'androidx.multidex:multidex:2.0.1'"))
            {
                multiDexAlreadyEnabled = false;
                mainTemplateFileContent = mainTemplateFileContent.Replace(
                    "\ndependencies {" , 
                    "\ndependencies {\n	implementation 'androidx.multidex:multidex:2.0.1'");
                File.WriteAllText(mainTemplateFileName, mainTemplateFileContent);
                LogEditorHelper.AddLog("mainTemplate.gradle Updated: implementation 'androidx.multidex:multidex:2.0.1'");
            }

            var androidManifestMultidexString = "android:name=\"androidx.multidex.MultiDexApplication\"";
            if (!androidManifestFileContent.Contains(androidManifestMultidexString))
            {
                multiDexAlreadyEnabled = false;
                var applicationTag = androidManifestFileContent.GetBetweenString("<application", ">");
                var applicationTagReplaced = applicationTag.Replace(
                    ">" , 
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
                    ">" , 
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

            mainTemplateFileContent = mainTemplateFileContent.Replace("// Android Resolver Repos Start\n", jetifierMainTemplateStr);
            File.WriteAllText(mainTemplateFileName, mainTemplateFileContent);
            LogEditorHelper.AddLog("Jetifier (androidx) added in mainTemplate.gradle");
        }

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