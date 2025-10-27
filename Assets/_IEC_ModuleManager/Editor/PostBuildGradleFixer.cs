using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor.Android;

namespace IEC
{
    public class PostBuildGradleFixer : IPostGenerateGradleAndroidProject
    {
        //Use for hard code build 2022 
        public int callbackOrder => 999;

        public void OnPostGenerateGradleAndroidProject(string path)
        {
            string gradleFile = Path.Combine(path, "build.gradle");
            if (!File.Exists(gradleFile))
            {
                UnityEngine.Debug.Log("Can't find gradleFile, path: " + path);
                return;
            }

            string content = File.ReadAllText(gradleFile);
            if (!content.Contains("namespace"))
            {
                content = content.Replace("android {", "android {\n    namespace \"com.iecgames.colorcubejam\"");
                File.WriteAllText(gradleFile, content);
                UnityEngine.Debug.Log("✅ Added namespace to Gradle file: " + gradleFile);
            }
        }
    }
}
