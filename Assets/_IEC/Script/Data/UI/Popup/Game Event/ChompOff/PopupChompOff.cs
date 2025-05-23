using System;
using System.Collections;
using System.Collections.Generic;
using IEC.Helper;
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
        public LayoutElement playerProgressImg;
        public GameEventPlayerDataUI opponentUI;
        public LayoutElement opponentProgressImg;
        public RectTransform connectorImage;
        public List<ChompOffRewardUI> listRewardUI = new();

        [Header("Random Opponent Avatar")] 
        public Sprite randomAvatar;
        public string randomName = "???";

        private DateTime _currentTimeEndEventStage;
        protected override void Awake()
        {
            base.Awake();
            
            SetEvent(playButton, OnPlayButtonClick);
            SetEvent(retryButton, OnRetryButtonClick);

            IsEventMultipleStage = true;
        }

        private void OnStartEventButtonClick()
        {
            
        }

        private void OnPlayButtonClick()
        {
            Hide();
            Locator.Main.PopupBooster.Show();
        }

        private void OnRetryButtonClick()
        {
            InitEvent();
        }

        protected override void OnClaimReward()
        {
            base.OnClaimReward();
        }

        protected override void ShowStartPopup()
        {
            Show();
        }

        public override void Show()
        {
            //base.Show();
            
            Init();
            
            if (PlayerData.level < _eventConfig.levelUnlock)
                return;
            
            UpdateUI();
        }
        
        protected override void UpdateUI()
        {
            var eventSaveData = _eventRankController.GetEventSaveData(eventType);
            var eventData = _eventConfig.GetEventData(eventSaveData.eventIndex);
            
            var player = eventSaveData.listPlayerData.Find(data => data.isPlayer);
            var opponent = eventSaveData.listPlayerData.Find(data => !data.isPlayer);
            
            playerUI.Setup(_eventConfig, eventData, player, 0);
            opponentUI.Setup(_eventConfig, eventData, opponent, 0);
            
            UpdateProgressBarUI(player, opponent);
            
            UpdateRewardUI();

            _currentTimeEndEventStage = eventSaveData.timeStart.AddSeconds(eventData.eventStageTimeInSeconds);
        }

        private void UpdateRewardUI()
        {
            var eventSaveData = _eventRankController.GetEventSaveData(eventType);

            for (int i = 0; i < listRewardUI.Count; i++)
            {
                listRewardUI[i].SetUITextActive(i == eventSaveData.eventIndex);

                if (i < eventSaveData.eventIndex)
                {
                    listRewardUI[i].SetRewardUIState(ChompOffRewardUIState.Opened);
                }
                else if (i == eventSaveData.eventIndex && IsEventStageCompleted())
                {
                    if(eventSaveData.claimedReward)
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

        private void UpdateProgressBarUI(EventPlayerData player, EventPlayerData opponent)
        {
            float total = player.progress + opponent.progress;
            if (total == 0) total = 1;
            
            playerProgressImg.flexibleWidth = player.progress / total;
            opponentProgressImg.flexibleWidth = opponent.progress / total;
            
            // Get world corners of imageA
            Vector3[] corners = new Vector3[4];
            playerProgressImg.GetComponent<RectTransform>().GetWorldCorners(corners);
            Vector3 rightEdgeWorld = corners[3]; // Top Right

            // Convert world position to local position relative to the parent canvas or connector's parent
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                connectorImage.parent as RectTransform,
                RectTransformUtility.WorldToScreenPoint(null, rightEdgeWorld),
                null,
                out localPoint
            );

            // Set connector position
            connectorImage.anchoredPosition = localPoint;
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

            return player.progress > opponent.progress;

        }

        protected override void UpdateTimer()
        {
            if (_currentTimeEndEventStage <= DateTime.Now)
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
    }
    
    public static class TweenDurationDistributor
    {
        // Ease-in-out cubic function
        private static float EaseInOutCubic(float t)
        {
            return 3f * t * t - 2f * t * t * t;
        }

        public static float[] GetEasedDurations(int tweenCount, float totalDuration)
        {
            float[] times = new float[tweenCount + 1];
            float[] durations = new float[tweenCount];

            // Sample eased progress points
            for (int i = 0; i <= tweenCount; i++)
            {
                float linearT = i / (float)tweenCount;
                times[i] = EaseInOutCubic(linearT);
            }

            // Convert eased progress differences into durations
            for (int i = 0; i < tweenCount; i++)
            {
                float segmentProgress = times[i + 1] - times[i];
                durations[i] = segmentProgress * totalDuration;
            }

            return durations;
        }
    }
}
