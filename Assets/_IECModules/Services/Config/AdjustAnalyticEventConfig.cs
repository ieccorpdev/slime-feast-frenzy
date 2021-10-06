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
    public string gameName;

    [Header("Data")]
    public List<Param> listData = new List<Param>();

    [Serializable]
    public class Param
    {
        public string gameEvent;
        public string adjustEventId;
    }

    [NaughtyAttributes.Button]
    public void LoadData()
    {
        listData.Clear();
        var data = DownloadCsvFromAuthUrl(adjustEventConfigUrl);
        if (data == null || data.Count == 0) return;
        var gameIndex = data[0].IndexOf(gameName);
        for (int i = 1; i < data.Count; i++)
        {
            var eventMap = data[i];
            var gameEvent = eventMap[0].Trim();
            var adjustEventId = eventMap[gameIndex].Trim();
            if (string.IsNullOrEmpty(gameEvent) || string.IsNullOrEmpty(adjustEventId))
            {
                continue;
            }
            var param = new Param { gameEvent = gameEvent, adjustEventId = adjustEventId };
            listData.Add(param);
        }
    }

    public Dictionary<string, string> GetDictEvent()
    {
        var dict = new Dictionary<string, string>();
        for (int i = 0; i < listData.Count; i++)
        {
            var data = listData[i];
            dict[data.gameEvent] = data.adjustEventId;
        }
        return dict;
    }

    public List<List<string>> DownloadCsvFromAuthUrl(string url)
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
