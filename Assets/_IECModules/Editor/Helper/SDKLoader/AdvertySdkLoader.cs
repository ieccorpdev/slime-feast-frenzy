using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BestHTTP;
using Newtonsoft.Json;
using SimpleJSON;

namespace _IECModules
{
    public class AdvertySdkLoader : BaseSdkLoader
    {
        protected override string SdkName => "adverty";
        public override string BaseUrl => "https://bitbucket.org/adverty/adverty-sdk/raw/master/AdvertySDK_{0}.unitypackage";
        protected override string[] ListVersionDefault => new[] { "4.1.7f1" };
        protected override void LoadVersionFromServer()
        {
            PlayerPrefs.DeleteKey(VersionCacheKey);
            var httpRequest = new HTTPRequest(new Uri("https://api.bitbucket.org/2.0/repositories/adverty/adverty-sdk/src"),
                (originalRequest, response) =>
                {
                    var data = JSONNode.Parse(response.DataAsText).AsObject;
                    var dataValues = data["values"];
                    var versions = new List<string>();
                    for (int i = 0; i < dataValues.Count; i++)
                    {
                        var pathValue = dataValues[i]["path"].Value;
                        if (!pathValue.Contains("Adverty")) continue;
                        versions.Add(Between(pathValue, "_", ".unitypackage"));
                    }
                    PlayerPrefs.SetString(VersionCacheKey, JsonConvert.SerializeObject(versions));
                }).Send();
        }

        string Between(string str, string firstString, string lastString)
        {
            int pos1 = str.IndexOf(firstString) + firstString.Length;
            int pos2 = str.Substring(pos1).IndexOf(lastString);
            return str.Substring(pos1, pos2);
        }
    }
}