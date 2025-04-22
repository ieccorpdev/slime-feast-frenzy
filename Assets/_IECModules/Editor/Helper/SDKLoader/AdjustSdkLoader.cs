using System;
using System.Collections.Generic;
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
            
        }
    }
}