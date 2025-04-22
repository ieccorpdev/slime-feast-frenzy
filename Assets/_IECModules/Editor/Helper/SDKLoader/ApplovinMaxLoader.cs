using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using SimpleJSON;
using UnityEngine;

namespace _IECModules
{
    public class ApplovinMaxLoader : BaseSdkLoader
    {
        public class PrefixVersion
        {
            public string android;
            public string iOS;
        }
        protected override string SdkName => "applovinMax";
        public override string BaseUrl => "https://artifacts.applovin.com/unity/com/applovin/applovin-sdk/AppLovin-MAX-Unity-Plugin-{0}.unitypackage";
        protected override string[] ListVersionDefault => new[]
        {
            "5.4.2-Android-11.4.2-iOS-11.4.1",
            "5.2.1-Android-11.2.1-iOS-11.2.0",
            "5.2.0-Android-11.2.0-iOS-11.2.0",
        };

        private List<PrefixVersion> listPrefixVersion = new List<PrefixVersion>()
        {
            new PrefixVersion
            {
                android = "Android SDK v",
                iOS = "iOS SDK v"
            },
            new PrefixVersion
            {
                android = "Android SDK [v",
                iOS = "iOS SDK [v"
            }
        };

        protected override void LoadVersionFromServer()
        {
            PlayerPrefs.DeleteKey(VersionCacheKey);
            var url = "https://api.github.com/repos/AppLovin/AppLovin-MAX-Unity-Plugin/releases";
            var content = UnityWebRequestDownloadHelper.DownloadTextFromUrl(url);
            // var json = JSON.Parse(content).AsArray;
            // var versions = new List<string>();
            // for (int i = 0; i < json.Count; i++)
            // {
            //     var node = json[i];
            //     var body = node["body"].ToString();
            //
            //     versions.AddRange(from prefix in listPrefixVersion
            //         where body.Contains(prefix.android) && body.Contains(prefix.iOS)
            //         let unityVersion = node["name"].ToString().Replace("\"", "")
            //         let androidVersion = string.Join(".", body.GetBetweenString(prefix.android, string.Empty, false)
            //             .Split(' ')[0]
            //             .Split(']')[0]
            //             .Split('.')
            //             .ToList()
            //             .GetRange(0, 3))
            //         let iOSVersion = string.Join(".", body.GetBetweenString(prefix.iOS, string.Empty, false)
            //             .Split(' ')[0]
            //             .Split(']')[0]
            //             .Split('.')
            //             .ToList()
            //             .GetRange(0, 3))
            //         select $"{unityVersion}-Android-{androidVersion}-iOS-{iOSVersion}");

                // if (body.Contains("Android SDK v") && body.Contains("iOS SDK v"))
                // {
                //     var unityVersion = node["name"].ToString().Replace("\"", "");
                //     var androidVersion = string.Join(".", body.GetBetweenString("Android SDK v", string.Empty, false)
                //         .Split(' ')[0].Split(']')[0].Split('.').ToList().GetRange(0, 3));
                //     var iOSVersion = string.Join(".", body.GetBetweenString("iOS SDK v", string.Empty, false)
                //         .Split(' ')[0].Split(']')[0].Split('.').ToList().GetRange(0, 3));
                //     versions.Add($"{unityVersion}-Android-{androidVersion}-iOS-{iOSVersion}");
                // }
                
                // if (body.Contains("Android SDK [v") && body.Contains("iOS SDK [v"))
                // {
                //     var unityVersion = node["name"].ToString().Replace("\"", "");
                //     var androidVersion = string.Join(".", body.GetBetweenString("Android SDK v", string.Empty, false)
                //         .Split(' ')[0].Split(']')[0].Split('.').ToList().GetRange(0, 3));
                //     var iOSVersion = string.Join(".", body.GetBetweenString("iOS SDK v", string.Empty, false)
                //         .Split(' ')[0].Split(']')[0].Split('.').ToList().GetRange(0, 3));
                //     versions.Add($"{unityVersion}-Android-{androidVersion}-iOS-{iOSVersion}");
                // }
            //}
            //PlayerPrefs.SetString(VersionCacheKey, JsonConvert.SerializeObject(versions));
        }
    }
}