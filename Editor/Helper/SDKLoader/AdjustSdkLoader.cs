using System;
using System.Collections.Generic;
using BestHTTP;
using Newtonsoft.Json;
using SimpleJSON;
using UnityEngine;

namespace _IECModules
{
    public class AdjustSdkLoader : BaseSdkLoader
    {
        protected override string SdkName => "adjust";
        public override string BaseUrl => "https://github.com/adjust/unity_sdk/releases/download/v{0}/Adjust_v{0}.unitypackage";
        protected override string[] ListVersionDefault => new[] {"4.29.0"};
        protected override void LoadVersionFromServer()
        {
            PlayerPrefs.DeleteKey(VersionCacheKey);
            var httpRequest = new HTTPRequest(
                new Uri("https://api.github.com/repos/adjust/unity_sdk/releases"),
                (originalRequest, response) =>
                {
                    var data = JSONNode.Parse(response.DataAsText).AsArray;
                    var versions = new List<string>();
                    for (var i = 0; i < data.Count; i++)
                    {
                        versions.Add(data[i]["name"].Value.Split(' ')[1]);
                    }

                    PlayerPrefs.SetString(VersionCacheKey, JsonConvert.SerializeObject(versions));
                }).Send();
        }
    }
}