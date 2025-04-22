using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using SimpleJSON;
using UnityEngine;

namespace _IECModules
{
    public class FacebookSdkLoader : BaseSdkLoader
    {
        protected override string SdkName => "facebook";
        public override string BaseUrl => "https://lookaside.facebook.com/developers/resources/?id=facebook-unity-sdk-{0}.zip";
        
        public const string LatestUrl = "https://lookaside.facebook.com/developers/resources/?id=FacebookSDK-current.zip";
        protected override string[] ListVersionDefault => new[] {"9.2.0"};
        protected override void LoadVersionFromServer()
        {
            PlayerPrefs.DeleteKey(VersionCacheKey);
            // var httpRequest = new HTTPRequest(
            //     new Uri("https://developers.facebook.com/docs/unity/downloads"),
            //     (originalRequest, response) =>
            //     {
            //         var versions = response.DataAsText
            //             .Split(new[] { "https://lookaside.facebook.com/developers/resources/" }, StringSplitOptions.None)
            //             .ToList();
            //         versions.RemoveAt(0);
            //         versions = versions.Select(x => x.GetBetweenString("\">", "</a></p>", false)).ToList();
            //         PlayerPrefs.SetString(VersionCacheKey, JsonConvert.SerializeObject(versions));
            //     }).Send();
        }
    }
}