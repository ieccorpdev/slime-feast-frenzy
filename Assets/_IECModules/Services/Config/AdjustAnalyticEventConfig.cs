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
        public string adjustAndroidEventId;
        public string adjustIosEventId;
        public bool isPushEventToFirebase;
    }

    [NaughtyAttributes.Button]
    public void LoadData()
    {
        listData.Clear();
        var data = DownloadCsvFromAuthUrl(adjustEventConfigUrl);
        if (data == null || data.Count == 0) return;
        var androidIndex = data[0].IndexOf("Adjust Android");
        var iosIndex = data[0].IndexOf("Adjust iOS");
        var firebaseIndex = data[0].IndexOf("Firebase");
        for (int i = 1; i < data.Count; i++)
        {
            var eventMap = data[i];
            var gameEvent = eventMap[0].Trim();
            var adjustAndroidEventId = (androidIndex >=0 && androidIndex < eventMap.Count) ? eventMap[androidIndex].Trim() : string.Empty;
            var adjustIosEventId = (iosIndex >= 0 && iosIndex < eventMap.Count) ? eventMap[iosIndex].Trim() : string.Empty;
            var isFirebase = (firebaseIndex >= 0 && firebaseIndex < eventMap.Count) && eventMap[firebaseIndex] == "TRUE"; 
            if (string.IsNullOrEmpty(gameEvent))
            {
                continue;
            }
            var param = new Param
            {
                gameEvent = gameEvent, 
                adjustAndroidEventId = adjustAndroidEventId, 
                adjustIosEventId = adjustIosEventId,
                isPushEventToFirebase = isFirebase
            };
            listData.Add(param);
        }
    }

    public Dictionary<string, string> GetDictEvent()
    {
        var dict = new Dictionary<string, string>();
        foreach (var data in listData)
        {
#if UNITY_ANDROID
            var adjustEvent = data.adjustAndroidEventId;
#else
            var adjustEvent = data.adjustIosEventId;
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
