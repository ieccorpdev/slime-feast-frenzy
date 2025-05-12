using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using EnhancedUI.EnhancedScroller;
using IEC.Helper;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace IEC
{
    public class PopupStarChase : BasePopup
    {
        [SerializeField] EnhancedScroller scroller;

        public TextMeshProUGUI eventTimerText;

        public List<StarChasePlayerDataUI> listPlayerDataUI = new();
        
        private EventRankController _eventRankController;
        private GameEventConfig _eventConfig;

        protected override void Awake()
        {
            base.Awake();
            _eventRankController = Controllers.Get<EventRankController>();
            _eventConfig = Config.AllGameEventConfig.GetGameEventConfig(EventType.StarChase);
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

        private void CheckShowReward()
        {
            
        }

        private void CheckInitEventProgress()
        {
            var data = PlayerData.GetEventSaveData(EventType.StarChase);
            if (DateTime.Now >= data.timeEnd || (DateTime.Now - data.timeStart.Date).TotalDays > 0)
            {
                CheckShowReward();
                
                List<EventPlayerData> listPlayerData = new();

                int eventIndex = data.eventIndex < _eventConfig.listEventData.Count - 1 ? data.eventIndex + 1 : 0;
                
                var nameList =
                    Config.AllGameEventConfig.GenerateAllNames(_eventConfig.GetEventData(eventIndex).listEventBotData
                        .Count);

                for (int i = 0; i < _eventConfig.GetEventData(eventIndex).listEventBotData.Count; i++)
                {
                    var botData = _eventConfig.GetEventData(eventIndex).listEventBotData[i];
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
                        EventType.StarChase,
                        eventIndex,
                        DateTime.Now,
                        DateTime.Now.AddHours(_eventConfig.eventDurationInHours),
                        new List<EventPlayerData>(listPlayerData)
                        );
                
                listPlayerData.Clear();
            }
        }

        private void CheckUpdateEventProgress()
        {
            var data = PlayerData.GetEventSaveData(EventType.StarChase);

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
            var eventSaveData = _eventRankController.GetEventSaveData(EventType.StarChase);
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
                    PlayerData.GetEventSaveData(EventType.StarChase).timeEnd - DateTime.Now);
        }
    }
}
