using Newtonsoft.Json;
using UnityEngine;

namespace _IECModules
{
    public class AudioMobSdkLoader : BaseSdkLoader
    {
        protected override string SdkName => "audioMob";
        public override string BaseUrl => "https://firebasestorage.googleapis.com/v0/b/audiomob-prod.appspot.com/o/AudioMob.v{0}.unitypackage?alt=media";
        protected override string[] ListVersionDefault => new[] { "7.2.0" };
        protected override void LoadVersionFromServer()
        {
            PlayerPrefs.DeleteKey(VersionCacheKey);
            var versions = ListVersionDefault; // currently can't get info unless signed in, so set to default
            PlayerPrefs.SetString(VersionCacheKey, JsonConvert.SerializeObject(versions));
        }

        public override bool LoadVersionFromCache()
        {
            return true;
        }
    }
}