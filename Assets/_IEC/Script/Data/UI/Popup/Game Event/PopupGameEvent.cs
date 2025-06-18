using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using EnhancedUI.EnhancedScroller;
using IEC.Helper;
using Sirenix.OdinInspector;
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

        [Header("Reward Group")] public Button claimRewardButton;
        public TextMeshProUGUI claimRewardButtonText;
        public GameObject claimRewardGroup;

        [Header("Info ui")] public List<Transform> listInfoUI = new();

        private Sequence showInfoSequence;

        [Space(10)] public EventType eventType;

        public TextMeshProUGUI eventTimerText;

        public GameEventPlayerDataUI playerDataUIPrefab;
        public List<GameEventPlayerDataUI> listPlayerDataUI = new();
        public List<EventPlayerData> listPlayerData = new();

        protected EventRankController _eventRankController;
        protected GameEventConfig _eventConfig;

        protected bool IsEventMultipleStage;

        public bool IsInEvent { get; private set; }

        private Action _actionOnceOnClaimReward;

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
            EventController.RaiseEvent(EventList.GamePlay.SHOW_REWARD_INFO_GAME_EVENT);

            if (_actionOnceOnClaimReward != null)
            {
                _actionOnceOnClaimReward?.Invoke();
            }
        }

        public virtual void Start()
        {
            if (scroller != null)
                scroller.OnBeingDrag += DisableRewardPopupMini;
        }

        protected virtual void OnShowInfo()
        {
            EventController.RaiseEvent(EventList.GamePlay.SHOW_REWARD_INFO_GAME_EVENT);

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

                float delay = 0.1f;
                for (var index = 0; index < listInfoUI.Count; index++)
                {
                    var ui = listInfoUI[index];

                    showInfoSequence.Insert(delay, ui.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack));
                    delay += 0.1f;
                }

                showInfoSequence.Insert(delay, closeInfoButton.transform.DOScaleX(1, 0.2f).SetEase(Ease.OutBack));
                var scaleMulti = 1.1f;
                var duration = 0.6f;
                closeInfoButton.transform.GetChild(0)?.DOScale(Vector3.one * scaleMulti, duration)
                    .SetLoops(-1, LoopType.Yoyo);
                showInfoSequence.Play();
            }
        }

        protected virtual void ShowStartPopup()
        {
        }

        public virtual bool CheckShowStartPopup()
        {
            if (_eventRankController == null)
            {
                Init();
            }

            var data = _eventRankController.GetEventSaveData(eventType);
            return (IsEndEventStage() && data.claimedReward) || data == null || data.listPlayerData.Count == 0;
        }

        protected virtual void Init()
        {
            _eventRankController = Controllers.Get<EventRankController>();
            _eventConfig = Config.AllGameEventConfig.GetGameEventConfig(eventType);

            InitScroller();
        }

        public void ResetEvent()
        {
            StartNewEventStage(0);
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

        public virtual void ReloadData()
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

            // if (CheckShowStartPopup())
            // {
            //     ShowStartPopup();
            // }
            // else
            {
                InitEvent();
            }
        }

        public virtual void InitEvent()
        {
            CheckUpdateEventProgress();
            CheckInitEventProgress();
            ReloadData();
        }

        public void DisableRewardPopupMini()
        {
            EventController.RaiseEvent(EventList.GamePlay.SHOW_REWARD_INFO_GAME_EVENT);
        }

        protected virtual void CheckShowReward()
        {
            var data = _eventRankController.GetEventSaveData(eventType);
            var playerRank =
                data.listPlayerData.FindIndex(player => player.isPlayer) + 1; // index in list starts from 0
            var eventRewardData = _eventConfig.GetEventData(data.eventIndex).listEventRewardData.Find(data =>
                data.rankStartReward <= playerRank && (data.rankEndReward >= playerRank || data.rankEndReward == -1));
            if (claimRewardGroup != null)
            {
                claimRewardGroup.SetActive(true);

                claimRewardButton.gameObject.SetActive(!data.claimedReward);

                if (claimRewardButtonText != null)
                {
                    if (eventRewardData == null || eventRewardData.listReward.Count == 0)
                    {
                        claimRewardButtonText.text = "OK!";
                    }
                    else
                    {
                        claimRewardButtonText.text = "Claim";
                    }
                }
            }

            eventTimerText.text = "Finished";
        }

        public void SetActionOnceOnClaimReward(Action action)
        {
            _actionOnceOnClaimReward = action;
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

            bool eventAvailable = Locator.Main.ScreenMain.trophyTab.IsEventCanShow(eventType);

            if (!eventAvailable)
            {
                stage = -1;
            }

            var player = data.listPlayerData.Find(player => player.isPlayer);
            var playerRank =
                data.listPlayerData.FindIndex(player => player.isPlayer) + 1; // index in list starts from 0
            var eventRewardData = _eventConfig.GetEventData(data.eventIndex).listEventRewardData.Find(data =>
                data.rankStartReward <= playerRank && (data.rankEndReward >= playerRank || data.rankEndReward == -1));

            var titleData = _eventConfig.GetRewardTitleData(playerRank);

            if (player.progress == 0)
            {
                titleData = _eventConfig.GetRewardTitleDataNoProgress(playerRank);
            }

            if (stage == -1)
                titleData = _eventConfig.GetWinAllEventStagesTitleData(playerRank);

            if (eventRewardData != null && eventRewardData.listReward.Count > 0)
            {
                string subtitle = String.Format(titleData.claimRewardSubtitleText,
                    ClaimBonusRewardPanel.GetOrdinal(playerRank));
                Locator.Main.ClaimBonusRewardPanel.SetupTempTitle(titleData.claimRewardTitleSprite, subtitle);
                Locator.Main.ClaimBonusRewardPanel.AddClaimRewardData(
                    new ShowClaimRewardData()
                    {
                        listDataReward = new List<Reward>(eventRewardData.listReward),
                    });

                if (stage != -1)
                {
                    var actionClaimedReward = _actionOnceOnClaimReward;
                    Locator.Main.ClaimBonusRewardPanel.SetActionOnceOnAnimRewardDone(() =>
                    {
                        if (actionClaimedReward == null)
                            ShowStartPopup();
                        actionClaimedReward?.Invoke();
                    });
                }

                Locator.Main.ClaimBonusRewardPanel.ShowNext();
            }
            else
            {
                // if (stage != -1)
                // {
                //     // StartNewEventStage(stage);
                //     // CheckInitEventProgress();
                //     // UpdateUI();
                //     ShowStartPopup();
                // }
                // else
                // Hide();
                _actionOnceOnClaimReward?.Invoke();
            }

            CheckAddTrophy(playerRank);
            _actionOnceOnClaimReward = null;
            Hide();
        }

        public void CheckAddTrophy(int playerRank)
        {
            if(!IsEventCompleted()) return;
            switch (eventType)
            {
                case EventType.SharkRace:
                    CollectTrophy(playerRank,TrophyType.SharkRace); 
                    break;
                case EventType.ChompOff:
                    CollectTrophy(playerRank,TrophyType.ChompOff); 
                    break;
                case EventType.StarChase:
                    CollectTrophy(playerRank,TrophyType.StarRace); 
                    break;
                case EventType.RelicHunt:
                    //Run with diffirent 
                    //CollectTrophy(playerRank,TrophyType.RelicHunt); 
                    break;
            }
        }

        private static void CollectTrophy(int playerRank,TrophyType trophyType)
        {
            if (playerRank <= 2)
            {
                PlayerData.trophyData.CollectTrophy(trophyType);
                Locator.Main.HomeScreenTab.CheckShowTrophyPopup();
                Locator.Main.HomeScreenTab.ShowNextPopup();
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

        public virtual DateTime GetTimeEndStage()
        {
            if (_eventRankController == null)
            {
                Init();
            }

            var data = _eventRankController.GetEventSaveData(eventType);
            return data.timeEnd;
        }

        public virtual bool IsHaveNextEventStage()
        {
            var data = _eventRankController.GetEventSaveData(eventType);

            return data.eventIndex < _eventConfig.listEventData.Count - 1;
        }

        public virtual bool IsEventStageCompleted()
        {
            return true;
        }

        public virtual int GetNextEventStageIndex()
        {
            if (_eventRankController == null)
            {
                Init();
            }

            var data = _eventRankController.GetEventSaveData(eventType);
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

            return stage;
        }

        protected virtual bool IsShowInfo()
        {
            var data = _eventRankController.GetEventSaveData(eventType);
            return data.listPlayerData.Count == 0;
        }

        protected virtual void CheckInitEventProgress()
        {
            if (_eventRankController == null)
            {
                Init();
            }

            var data = _eventRankController.GetEventSaveData(eventType);

            if (data.listPlayerData.Count > 0)
            {
                listPlayerData.Clear();
                listPlayerData = new List<EventPlayerData>(data.listPlayerData);
            }

            int stage = GetNextEventStageIndex();

            if (IsEndEvent() || (IsEventMultipleStage && IsEndEventStage()))
            {
                IsInEvent = false;

                if (IsShowInfo())
                {
                    OnShowInfo();
                    StartNewEventStage(0);
                }
                else
                {
                    if (!data.claimedReward)
                        CheckShowReward();
                    else
                        StartNewEventStage(stage);
                }
            }
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
            listPlayerData.Insert(listPlayerData.Count / 2,
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

            var data = _eventRankController.GetEventSaveData(eventType);

            var timeStart = DateTime.Now;
            //var timeEnd = eventStageIndex != 0 ? data.timeEnd : DateTime.Now.Date.AddHours(_eventConfig.eventDurationInHours);
            var timeEnd = eventStageIndex != 0
                ? data.timeEnd
                : Config.eventLiveOpsConfig.GetTimeEndEvent(
                    eventType,
                    Mathf.FloorToInt(_eventConfig.eventDurationInHours / 24),
                    PlayerData.GetDaysSinceDownload());

            _eventRankController
                .InitData(
                    eventType,
                    eventStageIndex,
                    timeStart,
                    //DateTime.Now.AddHours(_eventConfig.eventDurationInHours),
                    timeEnd,
                    new List<EventPlayerData>(listPlayerData)
                );

            CheckUpdateEventProgress();
        }

        public virtual void CheckUpdateEventProgress()
        {
            // if (!IsInEvent)
            //     return;

            if (_eventRankController == null)
            {
                Init();
            }

            var data = _eventRankController.GetEventSaveData(eventType);
            var eventData = _eventConfig.GetEventData(data.eventIndex);

            var timeToCheck = DateTime.Now <= GetTimeEndStage() ? DateTime.Now : GetTimeEndStage();

            if ((DateTime.Now - data.lastUpdateTime).TotalHours >= _eventConfig.timeUpdateEventProgressInHours ||
                IsEndEventStage())
            {
                data.lastUpdateTime = DateTime.Now;
                for (int i = 0; i < data.listPlayerData.Count; i++)
                {
                    var player = data.listPlayerData[i];

                    if (!player.isPlayer)
                    {
                        player.progress =
                            Mathf.FloorToInt((float)(timeToCheck - data.timeStart).TotalSeconds / player.speed);

                        if (eventData.eventTarget > 0 && player.progress > eventData.eventTarget)
                            player.progress = eventData.eventTarget;

                        if (player.progress < 0)
                            player.progress = 0;
                    }
                }
            }
        }

        protected virtual void UpdateTimer()
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

        [SerializeField] private float cellSize = 162;

        public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
        {
            if (listPlayerData[dataIndex].isPlayer)
            {
                return 192;
            }

            return cellSize;
        }

        public virtual EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
        {
            //var data = listPlayerData[dataIndex];
            var eventSaveData = _eventRankController.GetEventSaveData(eventType);

            GameEventPlayerDataUI cellView = scroller.GetCellView(playerDataUIPrefab) as GameEventPlayerDataUI;

            cellView.Setup(_eventConfig, _eventConfig.GetEventData(eventSaveData.eventIndex),
                eventSaveData.listPlayerData[dataIndex], dataIndex + 1);


            return cellView;
        }
    }
}