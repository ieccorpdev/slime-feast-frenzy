using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace IEC.CloudSave
{
    [Serializable]
    public class CloudSaveData
    {
        public Dictionary<string, string> dictDatas = new Dictionary<string, string>(); // key = save secret, value = json data
        
        public static string ConvertDataToJson(CloudSaveData data)
        {
            return JsonConvert.SerializeObject(data);
        }

        public static CloudSaveData ConvertJsonToData(string data)
        {
            return JsonConvert.DeserializeObject<CloudSaveData>(data);
        }
    }

    [Serializable]
    public class CloudSaveInstallData // Save per install, dont save to cloud
    {
        public string userId;
        public bool isCloudSaveOn;
        public bool isOldUserFirstTime;
    }
}