using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SimpleJSON;
using UnityEngine;

namespace _IECModules
{
    public class KochavaSdkLoader : BaseSdkLoader
    {
        protected override string SdkName => "kochava";
        public override string BaseUrl => "https://github.com/Kochava/unity-kochavatracker-releases/releases/download/v{0}/KochavaTrackerUnity.unitypackage";
        protected override string[] ListVersionDefault => new[] {"3.0.0"};
        protected override void LoadVersionFromServer()
        {
            PlayerPrefs.DeleteKey(VersionCacheKey);
            // var httpRequest = new HTTPRequest(
            //     new Uri("https://api.github.com/repos/Kochava/unity-kochavatracker-releases/releases"),
            //     (originalRequest, response) =>
            //     {
            //         var data = JSONNode.Parse(response.DataAsText).AsArray;
            //         var versions = new List<string>();
            //         for (var i = 0; i < data.Count; i++)
            //         {
            //             versions.Add(data[i]["name"]);
            //         }
            //
            //         PlayerPrefs.SetString(VersionCacheKey, JsonConvert.SerializeObject(versions));
            //     }).Send();
        }
    }
}