using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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
        public Button infoButton;
        public Button closeInfoButton;
        public GameObject infoGroup;

        [Header("Reward Group")] 
        public Button claimRewardButton;
        public GameObject claimRewardGroup;
        
        [Header("Info ui")]
        public List<Transform> listInfoUI = new();

        private Sequence showInfoSequence;
        
        [Space(10)]

        public EventType eventType;
        
        public TextMeshProUGUI eventTimerText;

        public GameEventPlayerDataUI playerDataUIPrefab;
        public List<GameEventPlayerDataUI> listPlayerDataUI = new();
        public List<EventPlayerData> listPlayerData = new ();
        
        protected EventRankController _eventRankController;
        protected GameEventConfig _eventConfig;

        protected bool IsEventMultipleStage;

        public bool IsInEvent { get; private set; }
        
        protected override void Awake()
        {
            base.Awake();
            
            SetEvent(infoButton, OnShowInfo);
            SetEvent(closeInfoButton, OnShowInfo);
            SetEvent(claimRewardButton, OnClaimReward);
            if (infoGroup != null)
            {
                infoGroup.SetActive(false);
            }

            if (claimRewardGroup != null)
            {
                claimRewardGroup.SetActive(false);
            }

            IsEventMultipleStage = false;
            
            Init();
        }

        public override void Hide()
        {
            base.Hide();
            EventController.RaiseEvent(EventList.GamePlay.UPDATE_GAME_EVENT_UI);
        }

        private void Start()
        {
            if(scroller != null)
                scroller.OnBeingDrag+=DisableRewardPopupMini;
        }

        protected virtual void OnShowInfo()
        {
            if (infoGroup != null)
            {
                infoGroup.SetActive(!infoGroup.activeInHierarchy);
            }

            if (infoGroup != null && infoGroup.activeInHierarchy)
            {
                DOTween.Kill(showInfoSequence);
                showInfoSequence = DOTween.Sequence();

                foreach (var ui in listInfoUI)
                {
                    ui.localScale = Vector3.zero;
                }

                closeInfoButton.transform.localScale = new Vector3(0, 1, 1);

                float delay = 0f;
                foreach (var ui in listInfoUI)
                {
                    showInfoSequence.Insert(delay, ui.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack));
                    delay += 0.1f;
                }

                showInfoSequence.Insert(delay, closeInfoButton.transform.DOScaleX(1, 0.2f).SetEase(Ease.OutBack));

                showInfoSequence.Play();
            }
        }

        protected virtual void Init()
        {
            _eventRankController = Controllers.Get<EventRankController>();
            _eventConfig = Config.AllGameEventConfig.GetGameEventConfig(eventType);
            
            InitScroller();
        }

        private void InitScroller()
        {
            if (scroller == null)
                return;
            
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
            if (scroller == null)
                return;
            
            scroller.RefreshActiveCellViews();
        }

        public void ReloadData()
        {
            if (scroller == null)
                return;
            
            scroller.ReloadData();
            JumpToPlayer();
        }
        
        private void JumpToPlayer()
        {
            if (scroller == null)
                return;
            
            InitScroller();
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
            var data = _eventRankController.GetEventSaveData(eventType);
            if (claimRewardGroup != null)
            {
                claimRewardGroup.SetActive(true);

                claimRewardButton.gameObject.SetActive(!data.claimedReward);
                
            }

            eventTimerText.text = "Finished";
        }

        protected virtual void OnClaimReward()
        {
            
            var data = _eventRankController.GetEventSaveData(eventType);

            // if (data.claimedReward)
            // {
            //     return;
            // }
            
            if (claimRewardGroup != null)
            {
                claimRewardGroup.SetActive(false);
            }
            data.claimedReward = true;
            int stage = 0;

            if (IsEventMultipleStage && !IsEndEvent())
            {
                if (IsEventStageCompleted())
                {
                    if (data.eventIndex < _eventConfig.listEventData.Count - 1)
                    {
                        stage = data.eventIndex + 1;
                    }
                    else
                    {
                        stage = -1;
                    }
                }
                else
                    stage = data.eventIndex;
            }

            var playerRank = data.listPlayerData.FindIndex(player => player.isPlayer) + 1; // index in list starts from 0
            var eventRewardData = _eventConfig.GetEventData(data.eventIndex).listEventRewardData.Find(data =>
                data.rankStartReward <= playerRank && (data.rankEndReward >= playerRank || data.rankEndReward == -1));

            if (eventRewardData.listReward.Count > 0)
            {
                Locator.Main.ClaimBonusRewardPanel.AddClaimRewardData(
                    new ShowClaimRewardData()
                    {
                        listDataReward = new List<Reward>(eventRewardData.listReward),
                    });

                Locator.Main.ClaimBonusRewardPanel.AddActionOnceOnClose(() =>
                {
                    if (stage != -1)
                    {
                        StartNewEventStage(stage);
                        CheckInitEventProgress();
                        UpdateUI();
                    }
                    else
                        Hide();
                });
                Locator.Main.ClaimBonusRewardPanel.ShowNext();
            }
            else
            {
                if (stage != -1)
                {
                    StartNewEventStage(stage);
                    CheckInitEventProgress();
                    UpdateUI();
                }
                else
                    Hide();
            }
        }

        protected virtual void ShowEventCompletedScreen()
        {
            
        }

        protected virtual void UpdateUI()
        {
            
        }

        public virtual bool IsEventCompleted()
        {
            return IsEndEvent();
        }

        public virtual bool IsEndEvent()
        {
            if (_eventRankController == null)
            {
                Init();
            }
            var data = _eventRankController.GetEventSaveData(eventType);
            return DateTime.Now >= data.timeEnd;
        }
        
        public virtual bool IsEndEventStage()
        {
            if (_eventRankController == null)
            {
                Init();
            }
            var data = _eventRankController.GetEventSaveData(eventType);
            return DateTime.Now >= data.timeEnd;
        }

        public virtual bool IsEventStageCompleted()
        {
            return true;
        }

        protected virtual void CheckInitEventProgress()
        {
            var data = _eventRankController.GetEventSaveData(eventType);
            
            if (data.listPlayerData.Count > 0 )
            {
                listPlayerData.Clear();
                listPlayerData = new List<EventPlayerData>(data.listPlayerData);
            }
            
            if (IsEndEvent() || (IsEventMultipleStage && IsEndEventStage()))
            {
                IsInEvent = false;

                if (data.listPlayerData.Count == 0) // First time in star chase
                {
                    OnShowInfo();
                    StartNewEventStage(0);
                }
                else
                {
                    CheckShowReward();
                }
            }
            // else
            // {
            //     listPlayerData.Clear();
            //     listPlayerData = new List<EventPlayerData>(data.listPlayerData);
            // }
        }

        public virtual void StartNewEventStage(int eventStageIndex)
        {
            IsInEvent = true;
            listPlayerData.Clear();
            
            var nameList =
                Config.AllGameEventConfig.GenerateAllNames(
                    _eventConfig.GetEventData(eventStageIndex).listEventBotData.Count, PlayerData.playerName);

            for (int i = 0; i < _eventConfig.GetEventData(eventStageIndex).listEventBotData.Count; i++)
            {
                var botData = _eventConfig.GetEventData(eventStageIndex).listEventBotData[i];
                var randomSpeedRange = Random.Range(-botData.speedRange * 100, botData.speedRange * 100);
                    
                listPlayerData.Add(
                    new EventPlayerData()
                    {
                        id = nameList[i],
                        avatarID = Config.playerAvatarConfig.GetRandomAvatarSprite().name,
                        avatarFrameID = Config.playerAvatarConfig.GetRandomAvatarFrameSprite().name,
                        progress = 0,
                        speed = botData.baseSpeed * (100 + randomSpeedRange) / 100, 
                        isPlayer = false,
                    });
            }
            
            //Insert player data to middle
            listPlayerData.Insert( listPlayerData.Count / 2,
                new EventPlayerData()
                {
                    id = PlayerData.playerName,
                    progress = 0,
                    speed = 0,
                    isPlayer = true,
                });

            for (int i = 0; i < listPlayerData.Count; i++)
            {
                listPlayerData[i].indexInUI = i;
            }
                
            _eventRankController
                .InitData(
                    eventType,
                    eventStageIndex,
                    DateTime.Now,
                    //DateTime.Now.AddHours(_eventConfig.eventDurationInHours),
                    DateTime.Now.AddDays(1).Date, //hard code to end of day - events last one day
                    new List<EventPlayerData>(listPlayerData)
                );
            
            CheckUpdateEventProgress();
        }

        protected virtual void CheckUpdateEventProgress()
        {
            // if (!IsInEvent)
            //     return;
            
            var data = _eventRankController.GetEventSaveData(eventType);
            var eventData = _eventConfig.GetEventData(data.eventIndex);
            
            

            var timeToCheck = DateTime.Now <= data.timeEnd ? DateTime.Now : data.timeEnd;

            if ((timeToCheck - data.lastUpdateTime).TotalHours >= _eventConfig.timeUpdateEventProgressInHours)
            {
                for (int i = 0; i < data.listPlayerData.Count; i++)
                {
                    var player = data.listPlayerData[i];

                    if (!player.isPlayer)
                    {
                        player.progress = Mathf.FloorToInt((float)(DateTime.Now - data.timeStart).TotalSeconds / player.speed);

                        if (eventData.eventTarget > 0 && player.progress > eventData.eventTarget)
                            player.progress = eventData.eventTarget;
                    }
                }
            }
        }

        private void UpdateTimer()
        {
            if (IsEndEvent())
            {
                if (IsInEvent)
                {
                    CheckUpdateEventProgress();
                    CheckInitEventProgress();
                }

                return;
            }
            
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
