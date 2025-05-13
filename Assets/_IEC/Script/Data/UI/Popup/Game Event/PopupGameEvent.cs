using System;
using System.Collections;
using System.Collections.Generic;
using EnhancedUI.EnhancedScroller;
using IEC.Helper;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace IEC
{
    public class PopupGameEvent : BasePopup
    {
        [SerializeField] EnhancedScroller scroller;

        public EventType eventType;
        
        public TextMeshProUGUI eventTimerText;

        public List<GameEventPlayerDataUI> listPlayerDataUI = new();
        
        protected EventRankController _eventRankController;
        protected GameEventConfig _eventConfig;

        protected bool IsEventMultipleStage;
        
        protected override void Awake()
        {
            base.Awake();
            _eventRankController = Controllers.Get<EventRankController>();
            _eventConfig = Config.AllGameEventConfig.GetGameEventConfig(eventType);

            IsEventMultipleStage = false;
        }
        
         private void Update()
        {
            UpdateTimer();
        }

        public override void Show()
        {
            if (PlayerData.level < _eventConfig.levelUnlock)
                return;

            base.Show();
            CheckInitEventProgress();
            CheckUpdateEventProgress();
            UpdateUI();
        }

        protected virtual void CheckShowReward()
        {
            
        }

        protected virtual void ShowEventCompletedScreen()
        {
            
        }

        public virtual bool IsEndEvent()
        {
            var data = PlayerData.GetEventSaveData(eventType);
            return DateTime.Now >= data.timeEnd || (DateTime.Now - data.timeStart.Date).TotalDays > 0;
        }
        
        public virtual bool IsEndEventStage()
        {
            var data = PlayerData.GetEventSaveData(eventType);
            return DateTime.Now >= data.timeEnd || (DateTime.Now - data.timeStart.Date).TotalDays > 0;
        }

        public virtual bool IsEventStageCompleted()
        {
            return true;
        }

        private void CheckInitEventProgress()
        {
            var data = PlayerData.GetEventSaveData(eventType);
            
            if (IsEndEvent())
            {
                CheckShowReward();
                
                StartNewEventStage(0);
            }
            else if (IsEventMultipleStage && IsEndEventStage())
            {
                if (data.eventIndex < _eventConfig.listEventData.Count - 1)
                {
                    CheckShowReward();
                
                    StartNewEventStage(data.eventIndex + 1);
                }
                else
                {
                    //When complete all stages of an event
                    ShowEventCompletedScreen();
                }
            }
        }

        private void StartNewEventStage(int eventStageIndex)
        {
            List<EventPlayerData> listPlayerData = new();
                
            var nameList =
                Config.AllGameEventConfig.GenerateAllNames(
                    _eventConfig.GetEventData(eventStageIndex).listEventBotData.Count);

            for (int i = 0; i < _eventConfig.GetEventData(eventStageIndex).listEventBotData.Count; i++)
            {
                var botData = _eventConfig.GetEventData(eventStageIndex).listEventBotData[i];
                var randomSpeedRange = Random.Range(-botData.speedRange, botData.speedRange);
                    
                listPlayerData.Add(
                    new EventPlayerData()
                    {
                        id = nameList[i],
                        progress = 0,
                        speed = botData.baseSpeed * (100 + randomSpeedRange) / 100, 
                    });
            }
                
            _eventRankController
                .InitData(
                    eventType,
                    eventStageIndex,
                    DateTime.Now.Date,
                    //DateTime.Now.AddHours(_eventConfig.eventDurationInHours),
                    DateTime.Now.AddDays(1).Date, //hard code to end of day - events last one day
                    new List<EventPlayerData>(listPlayerData)
                );
                
            listPlayerData.Clear();
        }

        private void CheckUpdateEventProgress()
        {
            var data = PlayerData.GetEventSaveData(eventType);

            if ((DateTime.Now - data.lastUpdateTime).TotalHours >= _eventConfig.timeUpdateEventProgressInHours)
            {
                for (int i = 0; i < data.listPlayerData.Count; i++)
                {
                    var player = data.listPlayerData[i];

                    if (!player.isPlayer)
                    {
                        player.progress = Mathf.FloorToInt((float)(DateTime.Now - data.timeStart).TotalSeconds / player.speed);
                    }
                }
            }
        }

        private void UpdateUI()
        {
            var eventSaveData = _eventRankController.GetEventSaveData(eventType);
            for (int i = 0; i < eventSaveData.listPlayerData.Count; i++)
            {
                if (i >= listPlayerDataUI.Count)
                {
                    listPlayerDataUI.Add(listPlayerDataUI[0].Spawn(scroller.transform));
                }
                
                listPlayerDataUI[i].Setup(eventSaveData.listPlayerData[i], i + 1);
            }
        }

        private void UpdateTimer()
        {
            eventTimerText.text =
                DateTimeHelper.GetFormattedTimeHMS(
                    PlayerData.GetEventSaveData(eventType).timeEnd - DateTime.Now);
        }
    }
}
