using System;
using System.Collections.Generic;
using System.Linq;
using BestHTTP;
using Newtonsoft.Json;
using SimpleJSON;
using UnityEngine;

namespace _IECModules
{
    public class OgurySdkLoader : BaseSdkLoader
    {
        protected override string SdkName => "ogury";
        public override string BaseUrl => "https://binaries.ogury.co/release/unity/{0}/OgurySdk-{0}.unitypackage";
        protected override string[] ListVersionDefault => new[] {"6.0.1"};
        protected override void LoadVersionFromServer()
        {
            
            PlayerPrefs.DeleteKey(VersionCacheKey);
            var url = "https://ogury-ltd.gitbook.io/release-notes/unity";
            var content = UnityWebRequestDownloadHelper.DownloadTextFromUrl(url);
            var versions = content.Split(new[] { "<h1 id=\"" }, StringSplitOptions.None)
                //.Select(x => x.Substring(0, 5))
                .ToList();
            versions.RemoveAt(0);
            versions = versions.Select(x => x.Split(new[] { " - " }, StringSplitOptions.None)[0]
                .Split(new[] { "\">" }, StringSplitOptions.None).Last()).ToList();
            //for(int i = 0; i < versions.Count; i++)
            //{
            //    var version = versions[i];
            //    //var versionCut = version.Split(new[] { "\" " }, StringSplitOptions.None)[0]
            //    //    .Split('-');
            //    //version = $"{versionCut[0]}.{versionCut[1]}.{versionCut[2]}";
            //    version = version.Split(new[] { " - " }, StringSplitOptions.None)[0];
            //    versions[i] = version.Split(new[] { "\">" }, StringSplitOptions.None).Last();
            //}
            PlayerPrefs.SetString(VersionCacheKey, JsonConvert.SerializeObject(versions));
        }
    }
}