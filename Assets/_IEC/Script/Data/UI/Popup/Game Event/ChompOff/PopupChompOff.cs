using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using IEC.Helper;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class PopupChompOff : PopupGameEvent
    {
        public Button startEventButton;
        public Button playButton;
        public Button retryButton;
        public GameEventPlayerDataUI playerUI;
        public GameEventPlayerDataUI opponentUI;
        public ChompOffProgressBarUI progressBarUI;
        public List<ChompOffRewardUI> listRewardUI = new();

        [Header("Random Opponent Avatar")] 
        public Sprite randomAvatar;
        public string randomName = "???";
        public int randomAvatarCount = 20;
        public float timeStartAnim = 5f;

        private DateTime _currentTimeEndEventStage;
        private DateTime _currentTimeEndEvent;

        private Sequence _startEventSequence;
        private bool _eventStarted = false;

        [Button]
        public void CheckTweenTime()
        {
            var tweenTimes = TweenDurationDistributor.GetEasedDurations(randomAvatarCount, timeStartAnim);

            string timeString = "";

            foreach (var time in tweenTimes)
            {
                timeString += time + " ";
            }
            Debug.LogError(timeString);
        }
        
        protected override void Awake()
        {
            base.Awake();
            
            SetEvent(startEventButton, OnStartEventButtonClick);
            SetEvent(playButton, OnPlayButtonClick);
            SetEvent(retryButton, OnRetryButtonClick);

            IsEventMultipleStage = true;
        }

        private void OnStartEventButtonClick()
        {
            startEventButton.gameObject.SetActive(false);
            DOTween.Kill(_startEventSequence);

            _startEventSequence = DOTween.Sequence();
            float delay = 0f;
            
            var nameList =
                Config.AllGameEventConfig.GenerateAllNames(randomAvatarCount, PlayerData.playerName);

            var tweenTimes = TweenDurationDistributor.GetEasedDurations(randomAvatarCount, timeStartAnim);

            
            for (int i = 0; i < randomAvatarCount; i++)
            {
                int index = i;
                _startEventSequence.InsertCallback(delay,
                    () =>
                    {
                        opponentUI.SetName(nameList[index]);
                        opponentUI.SetAvatar(Config.playerAvatarConfig.GetRandomAvatarSprite());
                    });
                delay += tweenTimes[i];
            }
            
            // _startEventSequence.InsertCallback(delay,
            //     () =>
            //     {
            //         opponentUI.UpdateUI();
            //     });
            //
            // delay += tweenTimes[randomAvatarCount];
            
            _startEventSequence.InsertCallback(delay,
                () =>
                {
                    _eventStarted = true;
                    playButton.gameObject.SetActive(true);
                    
                    playButton.transform.localScale = new Vector3(0, 1, 1);
                    playButton.transform.DOScaleX(1, 0.2f).SetEase(Ease.OutBack);

                    InitEvent();
                    
                    DOTween.Kill(_startEventSequence);
                    _startEventSequence = null;
                });
        }

        private void SetEventStartUI()
        {
            UpdateRewardUI();
            _eventStarted = false;
            playButton.gameObject.SetActive(false);
            startEventButton.gameObject.SetActive(true);
            
            progressBarUI.UpdateProgressBarUI(null, null);
            
            playerUI.SetAvatar(Config.playerAvatarConfig.GetAvatarSprite(PlayerData.playerAvatarSpriteName));
            playerUI.SetName(PlayerData.playerName);
            playerUI.SetProgressText("0");
            
            opponentUI.SetAvatar(randomAvatar);
            opponentUI.SetName(randomName);
            opponentUI.SetProgressText("0");
        }

        private void OnPlayButtonClick()
        {
            Hide();
            Locator.Main.PopupBooster.Show();
        }

        private void OnRetryButtonClick()
        {
            _eventStarted = true;
            var eventSaveData = _eventRankController.GetEventSaveData(eventType);

            eventSaveData.claimedReward = true;
            
            bool active = Locator.Main.ScreenMain.trophyTab.IsEventCanShow(eventType);

            if (!active || IsResetChompOff())
            {
                PlayerData.ResetEventSaveData(EventType.ChompOff);
                Hide();
                return;
            }
            
            _currentTimeEndEvent = eventSaveData.timeEnd;
            
            InitEvent();
            //playButton.gameObject.SetActive(true);
            SetEventStartUI();
            retryButton.gameObject.SetActive(false);
            
            // playButton.transform.localScale = new Vector3(0, 1, 1);
            // playButton.transform.DOScaleX(1, 0.2f).SetEase(Ease.OutBack);
        }

        protected override void OnClaimReward()
        {
            base.OnClaimReward();

            _eventStarted = false;

            if (IsResetChompOff())
            {
                PlayerData.ResetEventSaveData(EventType.ChompOff);
            }
        }

        public bool IsResetChompOff()
        {
            int daysPlayed = PlayerData.GetDaysSinceDownload();
            var chompOffData = PlayerData.GetEventSaveData(EventType.ChompOff);
            int daysLastChompOff = PlayerData.GetDaysFromDownloadToTime(chompOffData.timeStart);
            int chompOffDuration = Mathf.FloorToInt(Config.AllGameEventConfig.GetGameEventConfig(EventType.ChompOff)
                .eventDurationInHours / 24);
            
            bool isNewEvent = (daysPlayed - daysLastChompOff) >= chompOffDuration;

            return isNewEvent && (chompOffData.claimedReward || !IsEventStageCompleted());
        }

        protected override void ShowStartPopup()
        {
            Show();
        }

        protected override void CheckShowReward()
        {
            base.CheckShowReward();

            if (claimRewardButton.gameObject.activeInHierarchy)
            {
                playButton.gameObject.SetActive(false);
            }

            if (!IsEventStageCompleted())
            {
                claimRewardGroup.SetActive(false);
                retryButton.gameObject.SetActive(true);
            }
        }

        private void OnDisable()
        {
            if (_startEventSequence != null)
            {
                //DOTween.Kill(_startEventSequence);
                _startEventSequence.Kill();
                _startEventSequence = null;

                DOTween.Kill(playButton.transform);
                playButton.transform.localScale = Vector3.one;
                playButton.gameObject.SetActive(true);

                _eventStarted = true;
                
                InitEvent();
            }
        }

        public override void Hide()
        {
            base.Hide();
        }
        
        protected override bool IsShowInfo()
        {
            return false;
        }

        protected override void OnShowInfo()
        {
            if (infoGroup != null && infoGroup.activeInHierarchy)
            {
                mainPanel.gameObject.SetActive(true);
                Show();
            }
            else
            {
                mainPanel.gameObject.SetActive(false);
            }
            base.OnShowInfo();
        }

        public override void Show()
        {
            startEventButton.gameObject.SetActive(false);
            retryButton.gameObject.SetActive(false);
            playButton.gameObject.SetActive(true);
            
            base.Show();
            //
            //var eventSaveData = _eventRankController.GetEventSaveData(eventType);
            
            if (!PlayerData.isShownPopupInfoChompOff)
            {
                //mainPanel.gameObject.SetActive(false);
                OnShowInfo();
                PlayerData.isShownPopupInfoChompOff = true;
            }

            _currentTimeEndEvent = Config.eventLiveOpsConfig.GetTimeEndEvent(
                eventType, 
                Mathf.FloorToInt(_eventConfig.eventDurationInHours/24), 
                PlayerData.GetDaysSinceDownload());

            
            // Init();
            //
            // if (PlayerData.level < _eventConfig.levelUnlock)
            //     return;
            
            UpdateUI();
            
            if (CheckShowStartPopup())
            {
                SetEventStartUI();
            }
            else
            {
                _eventStarted = true;
            }
        }

        public override void InitEvent()
        {            
            if(_eventStarted)
                base.InitEvent();
            
            UpdateUI();
        }

        protected override void UpdateUI()
        {
            var eventSaveData = _eventRankController.GetEventSaveData(eventType);
            var eventData = _eventConfig.GetEventData(eventSaveData.eventIndex);
            
            var player = eventSaveData.listPlayerData.Find(data => data.isPlayer);
            var opponent = eventSaveData.listPlayerData.Find(data => !data.isPlayer);

            if (player == null)
                player = new EventPlayerData()
                {
                    id = PlayerData.playerName
                };
                
            playerUI.Setup(_eventConfig, eventData, player, 0);

            if (opponent != null)
            {
                opponent.progress = NumberSolver.GetClosestSmaller(opponent.progress,
                    Config.AllGameEventConfig.chompOffProgressConfig.listProgressAfterWin);
                opponentUI.Setup(_eventConfig, eventData, opponent, 0);
            }

            if (!_eventStarted)
            {
                opponentUI.SetAvatar(randomAvatar);
                opponentUI.SetName(randomName);
            }
            
            progressBarUI.UpdateProgressBarUI(player, opponent);
            
            UpdateRewardUI();

            _currentTimeEndEventStage = eventSaveData.timeStart.AddSeconds(eventData.eventStageTimeInSeconds);
            _currentTimeEndEvent = Config.eventLiveOpsConfig.GetTimeEndEvent(
                eventType, 
                Mathf.FloorToInt(_eventConfig.eventDurationInHours/24), 
                PlayerData.GetDaysSinceDownload());
        }

        public void UpdateCurrentTimeEndEvent()
        {
            var eventSaveData = _eventRankController.GetEventSaveData(eventType);
            var eventData = _eventConfig.GetEventData(eventSaveData.eventIndex);
            _currentTimeEndEventStage = eventSaveData.timeStart.AddSeconds(eventData.eventStageTimeInSeconds);
            _currentTimeEndEvent = Config.eventLiveOpsConfig.GetTimeEndEvent(
                eventType, 
                Mathf.FloorToInt(_eventConfig.eventDurationInHours/24), 
                PlayerData.GetDaysSinceDownload());
        }

        private void UpdateRewardUI()
        {
            var eventSaveData = _eventRankController.GetEventSaveData(eventType);

            int activeIndex = 0;

            if (eventSaveData != null)
            {
                activeIndex = eventSaveData.eventIndex;
            }

            for (int i = 0; i < listRewardUI.Count; i++)
            {
                listRewardUI[i].SetUITextActive(i == activeIndex);

                if (i < activeIndex)
                {
                    listRewardUI[i].SetRewardUIState(ChompOffRewardUIState.Opened);
                }
                else if (i == activeIndex && IsEventStageCompleted())
                {
                    if(eventSaveData != null && eventSaveData.claimedReward)
                        listRewardUI[i].SetRewardUIState(ChompOffRewardUIState.Opened);
                    else
                        listRewardUI[i].SetRewardUIState(ChompOffRewardUIState.Available);
                }
                else
                {
                    listRewardUI[i].SetRewardUIState(ChompOffRewardUIState.Unavailable);
                }
            }
        }

        public override DateTime GetTimeEndStage()
        {
            var eventSaveData = _eventRankController.GetEventSaveData(eventType);
            var data = _eventConfig.GetEventData(eventSaveData.eventIndex);

            return eventSaveData.timeStart.AddSeconds(data.eventStageTimeInSeconds);
        }

        public override bool IsEventCompleted()
        {
            if (IsEndEvent())
                return true;
            
            return IsEventStageCompleted() && !IsHaveNextEventStage();
        }

        public override bool IsEndEventStage()
        {
            if (_eventRankController == null)
            {
                Init();
            }
            
            var eventSaveData = _eventRankController.GetEventSaveData(eventType);
            var data = _eventConfig.GetEventData(eventSaveData.eventIndex);

            return (DateTime.Now - eventSaveData.timeStart).TotalSeconds >= data.eventStageTimeInSeconds;
        }

        public override bool IsEventStageCompleted()
        {
            if (!IsEndEventStage())
                return false;
            var eventSaveData = _eventRankController.GetEventSaveData(eventType);

            //Event only have player and opponent
            var player = eventSaveData.listPlayerData.Find(data => data.isPlayer);
            var opponent = eventSaveData.listPlayerData.Find(data => !data.isPlayer);

            if (player == null || opponent == null)
                return false;

            return player.progress > opponent.progress;
        }

        protected override void UpdateTimer()
        {
            if (!_eventStarted)
            {
                eventTimerText.text =
                    DateTimeHelper.GetFormattedTimeHMS(
                        _currentTimeEndEvent - DateTime.Now);
            }
            else if (_currentTimeEndEventStage <= DateTime.Now)
            {
                eventTimerText.text = "Finished";
            }
            else
            {
                eventTimerText.text =
                    DateTimeHelper.GetFormattedTimeHMS(
                        _currentTimeEndEventStage - DateTime.Now);
            }
        }

        public string GetTimerText(bool forcedGetTimeEndStage = false)
        {
            if (forcedGetTimeEndStage)
            {
                if (_currentTimeEndEventStage <= DateTime.Now)
                    _currentTimeEndEventStage = GetTimeEndStage();
                
                return 
                    DateTimeHelper.GetFormattedTimeHMS(
                        _currentTimeEndEventStage - DateTime.Now);
            }
            
            if (!_eventStarted)
            {
                if (_currentTimeEndEvent <= DateTime.Now)
                    _currentTimeEndEvent = Config.eventLiveOpsConfig.GetTimeEndEvent(
                        eventType,
                        Mathf.FloorToInt(_eventConfig.eventDurationInHours / 24),
                        PlayerData.GetDaysSinceDownload());
                
                return 
                    DateTimeHelper.GetFormattedTimeHMS(
                        _currentTimeEndEvent - DateTime.Now);
            }
            else if (_currentTimeEndEventStage <= DateTime.Now)
            {
                return "Finished";
            }
            else
            {
                return 
                    DateTimeHelper.GetFormattedTimeHMS(
                        _currentTimeEndEventStage - DateTime.Now);
            }
        }
    }
    
    public static class TweenDurationDistributor
    {
        // Ease-in-out cubic function
        private static float EaseInOutCubic(float t)
        {
            return 3f * t * t - 2f * t * t * t;
        }

        public static float[] GetEasedDurations(int tweenCount, float totalDuration, float curvePower = 0.5f)
        {
            float[] weights = new float[tweenCount];
            float totalWeight = 0f;

            for (int i = 0; i < tweenCount; i++)
            {
                float x = (float)i / (tweenCount - 1);             // normalized position [0, 1]
                float cosValue = Mathf.Cos(x * Mathf.PI);          // 1 → -1 over [0, 1]
                float weight = Mathf.Pow(Mathf.Abs(cosValue), curvePower);  // peak at edges, dip in middle
                weights[i] = weight;
                totalWeight += weight;
            }

            float[] durations = new float[tweenCount];
            for (int i = 0; i < tweenCount; i++)
            {
                durations[i] = (weights[i] / totalWeight) * totalDuration;
            }

            return durations;
        }
    }
    
    public class NumberSolver
    {
        // Check if target can be expressed using any combination of the given numbers
        public static bool CanBeExpressed(int target, List<int> values)
        {
            // DP table: dp[i] is true if i can be made with the values
            bool[] dp = new bool[target + 1];
            dp[0] = true;

            for (int i = 1; i <= target; i++)
            {
                foreach (int val in values)
                {
                    if (i - val >= 0 && dp[i - val])
                    {
                        dp[i] = true;
                        break;
                    }
                }
            }

            return dp[target];
        }

        // Get the closest smaller number that can be expressed with the given values
        public static int GetClosestSmaller(int n, List<int> values)
        {
            for (int k = n; k >= 0; k--)
            {
                if (CanBeExpressed(k, values))
                    return k;
            }
            return 0;
        }
    }
}
