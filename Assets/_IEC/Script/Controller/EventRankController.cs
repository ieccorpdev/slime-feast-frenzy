using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public class EventRankController : _BaseBehaviour, IController, IEventListener, IEventAwake, IEventStart
    {
        public void LocateController()
        {
            Controllers.Add(this);
        }

        public bool IsActive()
        {
            return gameObject.activeSelf;
        }

        public void BehaviourStart()
        {
            
        }

        public void BehaviourAwake()
        {
            
        }

        public void OnEventRaise(string gameEvent, EventParam param)
        {
            
        }

        public void InitData(EventType eventType, DateTime timeStart, DateTime timeEnd, List<EventPlayerData> listEventPlayerData)
        {
            var eventData = PlayerData.GetEventSaveData(eventType);

            eventData.listPlayerData = new List<EventPlayerData>(listEventPlayerData);
            eventData.lastUpdateTime = DateTime.MinValue;
            eventData.timeStart = timeStart;
            eventData.timeEnd = timeEnd;
        }

        private void AddEventProgress(AddEventProgressData progressData)
        {
            var eventData = PlayerData.GetEventSaveData(progressData.eventType);

            var player = eventData.listPlayerData.Find(bot => bot.id.Equals(progressData.id));

            if (player != null)
            {
                player.progress += progressData.progressAmount;
            }
        }
    }

    public enum EventType
    {
        StarChase,
        SharkRace,
    }
    
    [System.Serializable]
    public class AddEventProgressData
    {
        public EventType eventType;
        public string id;
        public int progressAmount;
    }

    [System.Serializable]
    public class EventPlayerData
    {
        public string id;
        public bool isPlayer = false;
        public int progress;
    }

    [System.Serializable]
    public class EventSaveData
    {
        public EventType eventType;
        
        public DateTime lastUpdateTime;
        public DateTime timeStart;
        public DateTime timeEnd;

        public List<EventPlayerData> listPlayerData = new();
    }
}
