using System;
using System.Collections;
using System.Collections.Generic;
using IEC.Common.Event;
using IEC.Helper;
using IEC.TrackingEvent;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace IEC
{
    public class GameEventHomeButton : _BaseBehaviour
    {
        public EventType eventType;
        public Button button;
        public TextMeshProUGUI levelUnlockText;
        public TextMeshProUGUI currentRankText;
        public TextMeshProUGUI timerText;

        public GameObject unlockedGroup;
        public GameObject lockedGroup;
        public GameObject joinGroup;
        public GameObject claimRewardGroup;
        public GameObject completedGroup;

        private PopupGameEvent _gameEventPopup;

        private EventRankController _eventRankController;
        private SkeletonGraphic  _skeletonGraphic;

        private void Awake()
        {
            _gameEventPopup = eventType switch
            {
                EventType.StarChase => Locator.Main.PopupStarChase,
                EventType.SharkRace => Locator.Main.PopupSharkRace,
                EventType.ChompOff => Locator.Main.PopupChompOff,
                _=> null,
            };

            _eventRankController = Controllers.Get<EventRankController>();
            
            gameObject.OnEvent(EventList.GamePlay.UPDATE_GAME_EVENT_UI, InitUI);
            _skeletonGraphic = GetComponentInChildren<SkeletonGraphic>();
        }

        public bool IsEventCanShow()
        {
            int level = PlayerData.level;
            int levelUnlock = Config.AllGameEventConfig.GetGameEventConfig(eventType).levelUnlock;
            return (level + Config.AllGameEventConfig.diffLevelShow >= levelUnlock);
        }
        public void InitUI() 
        {
            bool show = IsEventCanShow();
            gameObject.SetActive(show);
            if(!show) return;
            completedGroup.SetActive(false);
            unlockedGroup.SetActive(false);
            lockedGroup.SetActive(false);
            joinGroup.SetActive(false);
            claimRewardGroup.SetActive(false);

            var data = _eventRankController.GetEventSaveData(eventType);
            
            currentRankText.text = "Rank " +
                             (data.listPlayerData.FindIndex(data => data.isPlayer) +
                              1).ToString();
            
            if (eventType == EventType.ChompOff && !Locator.Main.PopupChompOff.IsEndEventStage()) // Custom for chomp off
            {
                currentRankText.text = Locator.Main.PopupChompOff.GetTimerText(true);
            }

            if (PlayerData.level < Config.AllGameEventConfig.GetGameEventConfig(eventType).levelUnlock)
            {
                levelUnlockText.text = "Lvl. " + Config.AllGameEventConfig.GetGameEventConfig(eventType).levelUnlock;
                lockedGroup.SetActive(true);
                _skeletonGraphic?.gameObject.SetActive(false);
                button.interactable = false;
            }
            else
            {
                if (data.listPlayerData.Count == 0)
                {
                    joinGroup.gameObject.SetActive(true);
                    button.interactable = true;
                }
                else
                {
                    if (_gameEventPopup.IsEndEvent() && data.claimedReward)
                    {
                        joinGroup.gameObject.SetActive(true);
                        button.interactable = true;
                    }
                    else if (_gameEventPopup.IsEndEventStage())
                    {
                        if (!data.claimedReward)
                        {
                            claimRewardGroup.SetActive(true);
                            button.interactable = true;
                        }
                        else
                        {
                            if (_gameEventPopup.IsHaveNextEventStage())
                            {
                                joinGroup.gameObject.SetActive(true);
                                button.interactable = true;
                            }
                            else
                            {
                                if (completedGroup != null)
                                {
                                    completedGroup.SetActive(true);
                                    _skeletonGraphic?.gameObject.SetActive(false);
                                    
                                    unlockedGroup.SetActive(false);
                                }

                                currentRankText.gameObject.SetActive(false);
                                timerText.gameObject.SetActive(true);
                                button.interactable = false;
                            }
                        }
                    }
                    else
                    {
                        unlockedGroup.SetActive(true);
                        button.interactable = true;
                    }
                }
            }
        }

        private void Update()
        {
            UpdateUI();
        }

        private TimeSpan countdownTimeSpan;
        public void UpdateUI()
        {
            if (timerText.gameObject.activeInHierarchy)
            {
                countdownTimeSpan = PlayerData.GetEventSaveData(eventType).timeEnd - DateTime.Now;
                timerText.text = DateTimeHelper.GetFormattedTimeHMS(
                    countdownTimeSpan);

                if (countdownTimeSpan.TotalSeconds <= 0)
                {
                    InitUI();
                }
            }
            
            if (eventType == EventType.ChompOff && currentRankText.gameObject.activeInHierarchy) // Custom for chomp off
            {
                currentRankText.text = Locator.Main.PopupChompOff.GetTimerText(true);
            }
        }
    }
}
