using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace IEC
{
    [CreateAssetMenu(menuName = "Game/GameEventLiveOpsConfig")]
    public class GameEventLiveOpsConfig : ScriptableObject
    {
        public List<EventLiveOpsByDay> listEventLiveOps = new();
        
        [Header("Load data from csv")]
        public string csv;

        private Dictionary<string, EventType> eventMap = new Dictionary<string, EventType>()
        {
            {"Undersea Voyage", EventType.UnderseaVoyage},
            {"Shark Race", EventType.SharkRace},
            {"Star Chase", EventType.StarChase},
            {"Chomp-off", EventType.ChompOff},
            {"Relic Hunt", EventType.RelicHunt},
        };


        [Button]
        public void LoadData()
        {
            var dataDefault = DownloadCsvFromAuthUrl(csv);
            
            if (dataDefault.Count == 0)
                return;
            
            listEventLiveOps.Clear();

            foreach (var row in dataDefault)
            {
                var evt = new EventLiveOpsByDay();
                foreach (var pair in eventMap)
                {
                    if (row.Find(data => data.Contains(pair.Key)) != null)
                    {
                        evt.listEvents.Add(pair.Value);
                    }
                }
                listEventLiveOps.Add(evt);
            }
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

        public int GetLatestDayStartEvent(EventType eventType, int currentDay)
        {
            if (currentDay > listEventLiveOps.Count - 1)
                currentDay %= (listEventLiveOps.Count);

            for (int i = currentDay; i >= 0; i--)
            {
                if (listEventLiveOps[i].listEvents.Contains(eventType))
                    return i;
            }

            return -1;
        }
        public DateTime GetNextTimeStartEvent(EventType eventType, int currentDay)
        {
            if (listEventLiveOps.Count == 0)
                return DateTime.MaxValue;

            if (currentDay > listEventLiveOps.Count - 1)
                currentDay %= listEventLiveOps.Count;

            int totalDays = listEventLiveOps.Count;
            for (int i = 1; i <= totalDays; i++)
            {
                int nextDay = (currentDay + i) % totalDays;
                if (listEventLiveOps[nextDay].listEvents.Contains(eventType))
                {
                    // Trả về thời điểm bắt đầu event tiếp theo
                    return DateTime.Now.Date.AddDays(i);
                }
            }
            return DateTime.MaxValue;
        }
        public DateTime GetTimeEndEvent(EventType eventType, int eventDurationInDays, int currentDay)
        {
            if (currentDay > listEventLiveOps.Count - 1)
                currentDay %= (listEventLiveOps.Count);
            
            int latestDayStartEvent = GetLatestDayStartEvent(eventType, currentDay);

            return (DateTime.Now.Date.AddDays(-(currentDay - latestDayStartEvent) + eventDurationInDays));
        }


    }

    [System.Serializable]
    public class EventLiveOpsByDay
    {
        public List<EventType> listEvents = new();
    }
}
