using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace _IECModules
{
    public static class KeystoreHelper
    {
        public static void CreateKeyStore(string keystoreName)
        {
            var keystoreFileName = $"{keystoreName}.keystore";
            var jdkPath = EditorPrefs.GetString("JdkPath");
            
            var osName = "MacOS";
            if (Application.platform.ToString().Contains("Windows")) osName = "Windows";
            if (Application.platform.ToString().Contains("Linux")) osName = "Linux";
            
            if (string.IsNullOrEmpty(jdkPath) || EditorPrefs.GetInt("JdkUseEmbedded") == 1)
            {
                
                var listPathCheck = new List<string>
                {
                    $"{EditorApplication.applicationContentsPath}/PlaybackEngines/AndroidPlayer/Tools/OpenJDK/{osName}",
                    $"{EditorApplication.applicationContentsPath}/PlaybackEngines/AndroidPlayer/OpenJDK"
                };
                
                var directoryInfo = new FileInfo(EditorApplication.applicationPath).Directory;
                if (directoryInfo != null)
                {
                    var applicationPathFile = directoryInfo.ToString();
                    listPathCheck.Add($"{applicationPathFile}/PlaybackEngines/AndroidPlayer/OpenJDK");
                    listPathCheck.Add($"{applicationPathFile}/PlaybackEngines/AndroidPlayer/Tools/OpenJDK/{osName}");
                    listPathCheck.Add($"{applicationPathFile}/PlaybackEngines/AndroidPlayer/Tools/OpenJDK");
                }

                foreach (var path in listPathCheck.Where(Directory.Exists))
                {
                    jdkPath = path; 
                    break;
                }
                
                if (!Directory.Exists(jdkPath))
                {
                    var logError = "Key Store Error: Cannot find keytool in following folder: " +
                                   $"\nJDKPath: {jdkPath}";
                    for (var i = 0; i < listPathCheck.Count; i++)
                    {
                        logError += $"\nJDKPath {i + 1}: {listPathCheck[i]}";
                    }
                    LogEditorHelper.AddLogError(logError);
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
                    LogEditorHelper.AddLog($"Keystore Created: {keystoreFileName}");
                }
                else if (standardOutput.Contains($"Key pair not generated, alias <{keystoreName}> already exists"))
                {
                    LogEditorHelper.AddLogWarning($"Keystore Already Exists: {keystoreFileName}");
                }
                else
                {
                    LogEditorHelper.AddLogError($"Key Store Creation Error: \nStandardOutput: {standardOutput}\nStandardError: {standardError}");   
                }
            }
        }

    }
}