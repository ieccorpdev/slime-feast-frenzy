using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
            switch (gameEvent)
            {
                case EventList.GamePlay.LEVEL_WIN:
                    // //Event Add Star
                    // AddPlayerEventProgress(
                    //     new AddEventProgressData()
                    //     {
                    //         eventType = EventType.StarChase,
                    //         id = PlayerData.playerName,
                    //         progressAmount = Locator.Game.ScreenLevelWin.StarWon,
                    //     });
                    //
                    // //Event Add Level
                    // AddPlayerEventProgress(
                    //     new AddEventProgressData()
                    //     {
                    //         eventType = EventType.SharkRace,
                    //         id = PlayerData.playerName,
                    //         progressAmount = 1,
                    //     });
                    break;
            }
        }

        public void InitData(EventType eventType, int eventIndex, DateTime timeStart, DateTime timeEnd, List<EventPlayerData> listEventPlayerData)
        {
            var eventData = PlayerData.GetEventSaveData(eventType);

            eventData.listPlayerData = new List<EventPlayerData>(listEventPlayerData);
            eventData.eventIndex = eventIndex;
            eventData.claimedReward = false;
            eventData.lastUpdateTime = DateTime.MinValue;
            eventData.timeStart = timeStart;
            eventData.timeEnd = timeEnd;
        }

        public void CheckAddPlayerProgress()
        {
            //Event Add Star
            AddPlayerEventProgress(
                Locator.Main.PopupStarChase,
                new AddEventProgressData()
                {
                    eventType = EventType.StarChase,
                    id = PlayerData.playerName,
                    progressAmount = Locator.Game.ScreenLevelWin.StarWon,
                });
                    
            //Event Add Level
            AddPlayerEventProgress(
                Locator.Main.PopupSharkRace,
                new AddEventProgressData()
                {
                    eventType = EventType.SharkRace,
                    id = PlayerData.playerName,
                    progressAmount = 1,
                });
            
            //Event Chomp Off
            AddPlayerEventProgress(
                Locator.Main.PopupChompOff,
                new AddEventProgressData()
                {
                    eventType = EventType.ChompOff,
                    id = PlayerData.playerName,
                    progressAmount = Config.AllGameEventConfig.chompOffProgressConfig.GetProgress(PlayerData.chompOffCurrentLevelLostCount),
                });
            PlayerData.ResetChompOffLevelLostCount();
        }

        public EventSaveData GetEventSaveData(EventType eventType)
        {
            var data = PlayerData.GetEventSaveData(eventType);
            
            data.listPlayerData =
                new List<EventPlayerData>(data.listPlayerData.OrderByDescending(playerData => playerData.progress));
            
            return data;
        }

        public void AddPlayerEventProgress(PopupGameEvent popupGameEvent, AddEventProgressData progressData)
        {
            if (PlayerData.level < Config.AllGameEventConfig.GetGameEventConfig(progressData.eventType).levelUnlock)
                return;
            
            var eventData = PlayerData.GetEventSaveData(progressData.eventType);
            var data = Config.AllGameEventConfig.GetGameEventConfig(progressData.eventType).GetEventData(eventData.eventIndex);

            if (DateTime.Now >= eventData.timeEnd)
                return;

            if (popupGameEvent.IsEndEventStage())
                return;
            

            var player = eventData.listPlayerData.Find(bot => bot.isPlayer);

            if (player != null)
            {
                player.progress += progressData.progressAmount;

                if (data.eventTarget > 0 && player.progress > data.eventTarget)
                {
                    player.progress = data.eventTarget;
                }
            }
        }
    }

    public enum EventType
    {
        StarChase,
        SharkRace,
        ChompOff
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
        public string avatarID;
        public string avatarFrameID;
        public int indexInUI; // For shark chase
        public bool isPlayer = false;
        public int progress;
        public float speed;
    }

    [System.Serializable]
    public class EventSaveData
    {
        public EventType eventType;

        //For event with different data after each win(shark race)
        public int eventIndex;
        
        public DateTime lastUpdateTime;
        public DateTime timeStart;
        public DateTime timeEnd;

        public bool claimedReward;

        public List<EventPlayerData> listPlayerData = new();
    }
}
