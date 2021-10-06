using System;
using System.Collections.Generic;
using System.Linq;
using BestHTTP;
using Newtonsoft.Json;
using UnityEngine;

namespace _IECModules
{
    public class BaseSdkLoader
    {
        protected virtual string SdkName => "";
        
        public int versionSelect;
        public virtual string SdkSelectVersionName => ListSDKVersion[versionSelect];
        public virtual string SdkSelectVersionUrl => string.Format(BaseUrl, SdkSelectVersionName);
        
        public virtual string BaseUrl => "";
        protected virtual string[] ListVersionDefault => new[] {"4.29.0"};
        
        private string[] ListVersionFromServer;

        public string[] ListSDKVersion =>
            ListVersionFromServer == null ||
            ListVersionFromServer.Length <= ListVersionDefault.Length
                ? ListVersionDefault
                : ListVersionFromServer;

        protected string VersionCacheKey => $"{SdkName}_version_cache";

        
        protected virtual void LoadVersionFromServer()
        {
        }

        public void Init()
        {
            LoadVersionFromServer();
        }

        public bool LoadVersionFromCache()
        {
            if (ListVersionFromServer != null) return true;
            var data = PlayerPrefs.GetString(VersionCacheKey);
            if (string.IsNullOrEmpty(data)) return false;
            ListVersionFromServer = JsonConvert.DeserializeObject<string[]>(data);
            return true;
        }
    }
}