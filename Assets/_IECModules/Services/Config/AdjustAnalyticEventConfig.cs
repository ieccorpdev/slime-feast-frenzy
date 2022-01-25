using System.Collections;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
[CreateAssetMenu(fileName = "AdjustAnalyticEventConfig", menuName = "Services/AdjustAnalyticEventConfig")]
public class AdjustAnalyticEventConfig : ScriptableObject
{
    [Header("Config")]
    public string adjustEventConfigUrl;
    // public string gameNameAndroid;
    // public string gameNameIos;

    [Header("Data")]
    public List<Param> listData = new List<Param>();
    
    [Serializable]
    public class Param
    {
        public string gameEvent;
        public PlatformParam android;
        public PlatformParam iOS;
    }
    
    [Serializable]
    public class PlatformParam
    {
        public string adjustEventId;
        public bool isPushEventFirebase;
        public bool isAdjustRev;
    }

    [NaughtyAttributes.Button]
    public void LoadData()
    {
        listData.Clear();
        var data = DownloadCsvFromAuthUrl(adjustEventConfigUrl);
        if (data == null || data.Count == 0) return;
        var androidIndex = data[0].IndexOf("Adjust Android");
        var iosIndex = data[0].IndexOf("Adjust iOS");
        var firebaseIndexAndroid = data[0].IndexOf("Firebase Android");
        var firebaseIndexIOS = data[0].IndexOf("Firebase iOS");
        var adjustRevAndroid = data[0].IndexOf("Adjust in app revenue Android");
        var adjustRevIOS = data[0].IndexOf("Adjust in app revenue iOS");
        for (int i = 1; i < data.Count; i++)
        {
            var eventMap = data[i];
            var gameEvent = eventMap[0].Trim();
            var adjustEventIdAndroid = GetValue(androidIndex, eventMap);
            var adjustEventIdIOS = GetValue(iosIndex, eventMap);
            var isFirebaseAndroid = GetValue(firebaseIndexAndroid, eventMap) == "TRUE";
            var isFirebaseIOS = GetValue(firebaseIndexIOS, eventMap) == "TRUE";
            var isAdjustRevAndroid = GetValue(adjustRevAndroid, eventMap)== "TRUE";
            var isAdjustRevIOS = GetValue(adjustRevIOS, eventMap) == "TRUE";
            if (string.IsNullOrEmpty(gameEvent))
            {
                continue;
            }
            var param = new Param
            {
                gameEvent = gameEvent, 
                android = new PlatformParam
                {
                    adjustEventId = adjustEventIdAndroid,
                    isPushEventFirebase = isFirebaseAndroid,
                    isAdjustRev = isAdjustRevAndroid
                },
                iOS = new PlatformParam
                {
                    adjustEventId = adjustEventIdIOS,
                    isPushEventFirebase = isFirebaseIOS,
                    isAdjustRev = isAdjustRevIOS
                    
                }
            };
            listData.Add(param);
        }
    }

    private string GetValue(int index, List<string> listValue)
    {
        return (index >= 0 && index < listValue.Count) ? listValue[index].Trim() : string.Empty;
    }

    public Dictionary<string, string> GetDictEvent()
    {
        var dict = new Dictionary<string, string>();
        foreach (var data in listData)
        {
#if UNITY_ANDROID
            var adjustEvent = data.android.adjustEventId;
#else
            var adjustEvent = data.iOS.adjustEventId;
#endif
            if(string.IsNullOrEmpty(data.gameEvent) || string.IsNullOrEmpty(adjustEvent)) continue;
            dict[data.gameEvent] = adjustEvent;
        }
        return dict;
    }

    private List<List<string>> DownloadCsvFromAuthUrl(string url)
    {
        var jsoner = new IEC.JsonDotNetModule();
        var uwr = UnityEditorHelper.GetWebRequestCsvFromAuthUrl(url);
        uwr.SendWebRequest();
        while (!uwr.isDone)
        {
            //Debug.Log(uwr.downloadProgress);
        }

        if (uwr.isNetworkError)
        {
            Debug.LogError("Download Error");
            Debug.LogError($"Downloading: {url}");
        }

        Debug.Log("Data downloaded:\n" + uwr.downloadHandler.text);

        return jsoner.FromJson<List<List<string>>>(uwr.downloadHandler.text);
    }
}
