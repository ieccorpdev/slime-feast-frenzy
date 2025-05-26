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
        public LayoutElement playerProgressImg;
        public GameEventPlayerDataUI opponentUI;
        public LayoutElement opponentProgressImg;
        public RectTransform connectorImage;
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
                    startEventButton.gameObject.SetActive(false);
                    
                    playButton.transform.localScale = new Vector3(0, 1, 1);
                    playButton.transform.DOScaleX(1, 0.2f).SetEase(Ease.OutBack);
                    
                    UpdateUI();

                    DOTween.Kill(_startEventSequence);
                    _startEventSequence = null;
                });
        }

        private void SetEventStartUI()
        {
            _eventStarted = false;
            playButton.gameObject.SetActive(false);
            startEventButton.gameObject.SetActive(true);
            
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
            Init();
            var eventSaveData = _eventRankController.GetEventSaveData(eventType);
            
            if (eventSaveData != null)
            {
                _currentTimeEndEvent = eventSaveData.timeEnd;
            }
            else
            {
                _currentTimeEndEvent = DateTime.Now.Date.AddHours(_eventConfig.eventDurationInHours).Date;
            }
            
            base.Show();
            
            if (CheckShowStartPopup())
            {
                SetEventStartUI();
            }
            
            // Init();
            //
            // if (PlayerData.level < _eventConfig.levelUnlock)
            //     return;
            
            //UpdateUI();
        }

        public override void InitEvent()
        {
            base.InitEvent();
            
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

            if (!_eventStarted)
            {
                opponentUI.SetAvatar(randomAvatar);
                opponentUI.SetName(randomName);
            }
            
            UpdateProgressBarUI(player, opponent);
            
            UpdateRewardUI();

            _currentTimeEndEventStage = eventSaveData.timeStart.AddSeconds(eventData.eventStageTimeInSeconds);
            _currentTimeEndEvent = eventSaveData.timeEnd;
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
            connectorImage.anchoredPosition = new Vector3(localPoint.x, connectorImage.anchoredPosition.y);
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
}
