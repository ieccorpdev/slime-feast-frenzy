using System;
using System.Collections;
using System.Collections.Generic;
using EnhancedUI.EnhancedScroller;
using IEC.Helper;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace IEC
{
    public class PopupGameEvent : BasePopup, IEnhancedScrollerDelegate
    {
        public EnhancedScroller scroller;

        public EventType eventType;
        
        public TextMeshProUGUI eventTimerText;

        public GameEventPlayerDataUI playerDataUIPrefab;
        public List<GameEventPlayerDataUI> listPlayerDataUI = new();
        public List<EventPlayerData> listPlayerData = new ();
        
        protected EventRankController _eventRankController;
        protected GameEventConfig _eventConfig;

        protected bool IsEventMultipleStage;
        
        protected override void Awake()
        {
            base.Awake();
            Init();
        }

        private void Start()
        {
            scroller.OnBeingDrag+=DisableRewardPopupMini;
        }

        protected virtual void Init()
        {
            _eventRankController = Controllers.Get<EventRankController>();
            _eventConfig = Config.AllGameEventConfig.GetGameEventConfig(eventType);

            IsEventMultipleStage = false;
            
            
            
            if (scroller.Delegate == null)
            {
                scroller.Delegate = this;
            }
            else
            {
                LoadData();
            }
        }
        
        public void LoadData()
        {
            scroller.RefreshActiveCellViews();
        }

        public void ReloadData()
        {
            scroller.ReloadData();
            JumpToPlayer();
        }
        
        private void JumpToPlayer()
        {
            Init();
            int index = listPlayerData.FindIndex(data => data.isPlayer);
            scroller.JumpToDataIndex(index, 0.5f, 0, true, EnhancedScroller.TweenType.easeInOutQuart, 0.2f);
        }
        
         private void Update()
        {
            UpdateTimer();
        }

        public override void Show()
        {
            Init();
            
            if (PlayerData.level < _eventConfig.levelUnlock)
                return;

            base.Show();
            CheckInitEventProgress();
            CheckUpdateEventProgress();
            //UpdateUI();
            
            ReloadData();
        }
        
        public void DisableRewardPopupMini()
        {
            EventController.RaiseEvent(EventList.GamePlay.SHOW_REWARD_INFO_STARCHASE);
        }

        protected virtual void CheckShowReward()
        {
            
        }

        protected virtual void ShowEventCompletedScreen()
        {
            
        }

        public virtual bool IsEndEvent()
        {
            var data = _eventRankController.GetEventSaveData(eventType);
            return DateTime.Now >= data.timeEnd;
        }
        
        public virtual bool IsEndEventStage()
        {
            var data = _eventRankController.GetEventSaveData(eventType);
            return DateTime.Now >= data.timeEnd || (DateTime.Now - data.timeStart.Date).TotalDays > 0;
        }

        public virtual bool IsEventStageCompleted()
        {
            return true;
        }

        private void CheckInitEventProgress()
        {
            var data = _eventRankController.GetEventSaveData(eventType);
            
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
            else
            {
                listPlayerData.Clear();
                listPlayerData = new List<EventPlayerData>(data.listPlayerData);
            }
        }

        private void StartNewEventStage(int eventStageIndex)
        {
            listPlayerData.Clear();
            
            var nameList =
                Config.AllGameEventConfig.GenerateAllNames(
                    _eventConfig.GetEventData(eventStageIndex).listEventBotData.Count);

            for (int i = 0; i < _eventConfig.GetEventData(eventStageIndex).listEventBotData.Count; i++)
            {
                var botData = _eventConfig.GetEventData(eventStageIndex).listEventBotData[i];
                var randomSpeedRange = Random.Range(-botData.speedRange * 100, botData.speedRange * 100);
                    
                listPlayerData.Add(
                    new EventPlayerData()
                    {
                        id = nameList[i],
                        progress = 0,
                        speed = botData.baseSpeed * (100 + randomSpeedRange) / 100, 
                        isPlayer = false,
                    });
            }
            
            listPlayerData.Add(
                new EventPlayerData()
                {
                    id = PlayerData.playerName,
                    progress = 0,
                    speed = 0,
                    isPlayer = true,
                });
                
            _eventRankController
                .InitData(
                    eventType,
                    eventStageIndex,
                    DateTime.Now.Date,
                    //DateTime.Now.AddHours(_eventConfig.eventDurationInHours),
                    DateTime.Now.AddDays(1).Date, //hard code to end of day - events last one day
                    new List<EventPlayerData>(listPlayerData)
                );
        }

        private void CheckUpdateEventProgress()
        {
            var data = _eventRankController.GetEventSaveData(eventType);

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
            // var eventSaveData = _eventRankController.GetEventSaveData(eventType);
            // for (int i = 0; i < eventSaveData.listPlayerData.Count; i++)
            // {
            //     if (i >= listPlayerDataUI.Count)
            //     {
            //         listPlayerDataUI.Add(playerDataUIPrefab.Spawn(scroller.Container.transform));
            //     }
            //
            //     listPlayerDataUI[i].transform.localScale = Vector3.one;
            //     listPlayerDataUI[i].Setup(_eventConfig, eventSaveData.listPlayerData[i], i + 1);
            // }
        }

        private void UpdateTimer()
        {
            eventTimerText.text =
                DateTimeHelper.GetFormattedTimeHMS(
                    PlayerData.GetEventSaveData(eventType).timeEnd - DateTime.Now);
        }

        public int GetNumberOfCells(EnhancedScroller scroller)
        {
            return listPlayerData.Count;
        }

        public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
        {
            if (listPlayerData[dataIndex].isPlayer)
            {
                return 192;
            }
            return 162;
        }

        public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
        {
            //var data = listPlayerData[dataIndex];
            var eventSaveData = _eventRankController.GetEventSaveData(eventType);

            GameEventPlayerDataUI cellView = scroller.GetCellView(playerDataUIPrefab) as GameEventPlayerDataUI;
            
            cellView.Setup(_eventConfig, _eventConfig.GetEventData(eventSaveData.eventIndex),  eventSaveData.listPlayerData[dataIndex], dataIndex + 1);
            

            return cellView;
        }
    }
}
