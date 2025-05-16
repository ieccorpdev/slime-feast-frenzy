using System;
using System.Collections;
using System.Collections.Generic;
using IEC.Common.Event;
using IEC.Helper;
using IEC.TrackingEvent;
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

        private void Awake()
        {
            if (eventType == EventType.StarChase)
                _gameEventPopup = Locator.Main.PopupStarChase;
            else if (eventType == EventType.SharkRace)
                _gameEventPopup = Locator.Main.PopupSharkRace;

            _eventRankController = Controllers.Get<EventRankController>();
            
            gameObject.OnEvent(EventList.GamePlay.UPDATE_GAME_EVENT_UI, InitUI);
        }

        public void InitUI()
        {
            completedGroup.SetActive(false);
            unlockedGroup.SetActive(false);
            lockedGroup.SetActive(false);
            joinGroup.SetActive(false);
            claimRewardGroup.SetActive(false);

            var data = _eventRankController.GetEventSaveData(eventType);
            
            currentRankText.text = "Rank " +
                             (data.listPlayerData.FindIndex(data => data.isPlayer) +
                              1).ToString();

            if (PlayerData.level < Config.AllGameEventConfig.GetGameEventConfig(eventType).levelUnlock)
            {
                levelUnlockText.text = "Lvl. " + Config.AllGameEventConfig.GetGameEventConfig(eventType).levelUnlock;
                lockedGroup.SetActive(true);
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
                            if (completedGroup != null)
                            {
                                completedGroup.SetActive(true);
                                unlockedGroup.SetActive(false);
                            }
                            currentRankText.gameObject.SetActive(false);
                            timerText.gameObject.SetActive(true);
                            button.interactable = false;
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


        public void UpdateUI()
        {
            if (timerText.gameObject.activeInHierarchy)
            {
                timerText.text = DateTimeHelper.GetFormattedTimeHMS(
                    PlayerData.GetEventSaveData(eventType).timeEnd - DateTime.Now);
            }
        }
    }
}
