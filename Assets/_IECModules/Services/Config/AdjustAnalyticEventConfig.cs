using System.Collections;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using IEC;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "AdjustAnalyticEventConfig", menuName = "Services/AdjustAnalyticEventConfig")]
public class AdjustAnalyticEventConfig : ScriptableObject
{
    [Header("Config")]
    public PlatformConfigUrl configUrl;

    [Header("Data")]
    public List<Param> listData = new List<Param>();

    [Serializable]
    public class Param
    {
        public string gameEvent;
        public PlatformParam param;

        public PlatformParam PlatformParam => param;
    }

    [Serializable]
    public class PlatformParam
    {
        public string adjustEventId;
        public bool isPushEventFirebase;
        public bool isAdjustRev;
        public bool isUnique;
    }

    [Serializable]
    public class PlatformConfigUrl
    {
        public string defaultEventUrl;
        [Header("Marketing config")]
        public string appVersion;
        public string marketingEventUrl;
    }

    [Button]
    public void LoadData()
    {
        listData.Clear();
        var dataDefault = DownloadCsvFromAuthUrl(configUrl.defaultEventUrl);

        var dataMkt = DownloadCsvFromAuthUrl(configUrl.marketingEventUrl);

        UpdateData(dataDefault, true);

        UpdateData(dataMkt, false);

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
        UnityEditor.AssetDatabase.SaveAssets();
#endif
    }

    void UpdateData(List<List<string>> sheetData, bool isDefault)
    {
        if (sheetData == null || sheetData.Count == 0) return;
        for (int i = 0; i < sheetData[0].Count; i++) {
            sheetData[0][i] = sheetData[0][i].Trim();
        }
        var adjustIndex = sheetData[0].IndexOf("Adjust");
        var firebaseIndex = sheetData[0].IndexOf("Firebase");
        var adjustRev = sheetData[0].IndexOf("Adjust in app revenue");
        for (int i = 1; i < sheetData.Count; i++)
        {
            var eventMap = sheetData[i];
            var gameEvent = eventMap[0].Trim();
            var adjustEventId = GetValue(adjustIndex, eventMap);
            var isFirebase = GetValue(firebaseIndex, eventMap) == "TRUE";
            var isAdjustRev = GetValue(adjustRev, eventMap) == "TRUE";
            if (string.IsNullOrEmpty(gameEvent)) continue;
            if (!listData.Exists(x => x.gameEvent == gameEvent))
            {
                listData.Add(new Param
                {
                    gameEvent = gameEvent
                });
            }
            var data = listData.Find(x => x.gameEvent == gameEvent);
            var param = new PlatformParam
            {
                adjustEventId = adjustEventId,
                isPushEventFirebase = isDefault || isFirebase,
                isAdjustRev = isAdjustRev,
                isUnique = data.gameEvent.Contains("unique")
            };
            data.param = param;
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
            var adjustEvent = data.PlatformParam.adjustEventId;
            if (string.IsNullOrEmpty(data.gameEvent) || string.IsNullOrEmpty(adjustEvent)) continue;
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

    public List<int> GetListPlayLevel()
    {
        return GetListValueEvent("play_level_");
    }

    public List<int> GetListPlayLevelFirstTime()
    {
        return GetListValueEvent("play_level_first_time_");
    }
    public List<int> GetListInter()
    {
        return GetListValueEvent("inter_","","rev_inter");
    }
    public Dictionary<RevAdType, List<int>> GetListMilestone()
    {
        var dict = new Dictionary<RevAdType, List<int>>
        {
            [RevAdType.INTER] = GetListValueEvent("rev_inter_"),
            [RevAdType.BANNER] = GetListValueEvent("rev_banner_"),
            [RevAdType.REWARD] = GetListValueEvent("rev_reward_"),
            [RevAdType.INTER_N_REWARD] = GetListValueEvent("rev_inter_n_reward_")
        };
       
       
       foreach (var key in dict.Keys)
       {
           var listMilestoneStr = string.Empty;
           var listMilestone = dict[key];
           listMilestoneStr = listMilestone.Aggregate(listMilestoneStr, (current, value) => current + $"{value},");
           if(!string.IsNullOrEmpty(listMilestoneStr)) listMilestoneStr = listMilestoneStr.Remove(listMilestoneStr.Length - 1);
           Debug.LogWarning($"{key.ToString()}:{listMilestoneStr}");
       }

       return dict;
    }

    public List<RevMilestone> GetListRevAllAdCentMilestone()
    {
        var list =  GetListValueEventWithEvent("rev_all_ad_", "_cent")
            .Select(x => new RevMilestone(x.eventName, x.value / 100f))
            .ToList();
        
        foreach (var revMilestone in list)
        {
            Debug.LogWarning($"{revMilestone.eventName}:{revMilestone.step}");
        }

        return list;
    }

    private bool TryGetValueEvent(string origin, string first, string end, out int result)
    {
        return int.TryParse(StringHelper.GetBetweenString(origin, first, end, false), out result);
    }

    private List<(int value, string eventName)> GetListValueEventWithEvent(string prefix, string end = "")
    {
        if (end == "") end = string.Empty;
        var listResult = new List<(int value, string eventName)>();
        foreach (var data in listData)
        {
            var eventName = data.gameEvent;
            if (eventName.Contains(prefix) && TryGetValueEvent(eventName, prefix, end, out var result))
            {
                listResult.Add((result, eventName));
            }
        }
        return listResult;
    }

    private List<int> GetListValueEvent(string prefix, string end = "",string ignore = "")
    {
        if (end == "") end = string.Empty;
        var listResult = new List<int>();
        foreach (var data in listData)
        {
            var eventName = data.gameEvent;
            if (ignore != "" && eventName.Contains(ignore))
            {
                continue;
            }
            if (eventName.Contains(prefix) && TryGetValueEvent(eventName, prefix, end, out var result))
            {
                listResult.Add(result);
            }
        }
        return listResult;
    }

    public Param GetEventParam(string eventName)
    {
        return listData.Find(x => x.gameEvent == eventName);
    }
}
