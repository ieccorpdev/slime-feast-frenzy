using MoreMountains.NiceVibrations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public class UIController : _BaseBehaviour, IController, IEventListener, IEventAwake
    {
        private static readonly List<string> TextWinBonus = new List<string>
        {"You've done well", "Cool", "Bravo", "Awesome!", "Great", "Nicely Done!"};

        public void LocateController()
        {
            Controllers.Add(this);
        }

        public bool IsActive()
        {
            return gameObject.activeSelf;
        }

        public void BehaviourAwake()
        {
            InitButtonEvent();
        }

        public void OnEventRaise(string gameEvent, EventParam param)
        {
            switch (gameEvent)
            {
                case EventList.GamePlay.GAME_SPAWN_LEVEL:
                    OnInitNewLevel();
                    break;
                case EventList.GamePlay.GAME_OVER:
                    GameOver();
                    break;
            }
        }

        private void InitButtonEvent()
        {
            UI.btnRestart.onClick.AddListener(() => {
                Services.PushEvent(new IEC.TrackingEvent.ReplayLevel());
                EventController.RaiseEvent(EventList.GamePlay.GAME_RESTART);
                if (PlayerData.isVibrateOn)
                {
                    MMVibrationManager.Haptic(HapticTypes.Failure);
                }
            });

            UI.btnSkip.onClick.AddListener(() => {
                if (Game.isTutorial) return;
                Services.ShowRewardAds(() => {
                    Services.PushEvent(new IEC.TrackingEvent.SkipLevel());
                    PlayerData.level += 1;
                    EventController.RaiseEvent(EventList.GamePlay.GAME_SPAWN_LEVEL);
                });
            });

            UI.btnGONext.onClick.AddListener(() => {
                EventController.RaiseEvent(EventList.GamePlay.GAME_RESTART);
            });

            UI.btnSetting.onClick.AddListener(() => {
                UI.popupSettings.gameObject.SetActive(true);
                Game.isShowPopup = true;
            });

            UI.popupSettings.btnClose.onClick.AddListener(() => {
                UI.popupSettings.gameObject.SetActive(false);
                Game.isShowPopup = false;
            });

            UI.popupSettings.btnSound.onClick.AddListener(() => {
                PlayerData.isSoundOn = !PlayerData.isSoundOn;
                SetStatusSetting();
            });

            UI.popupSettings.btnVibrate.onClick.AddListener(() => {
                PlayerData.isVibrateOn = !PlayerData.isVibrateOn;
                SetStatusSetting();
            });
        }

        public void SetupAbUI()
        {
            UI.btnSetting.gameObject.SetActive(Game.isHaveSetting);
            SetStatusSetting();
            Sound.SetupAudio();
        }

        private void OnInitNewLevel()
        {
            Game.isShowPopup = false;
            UI.popupSettings.gameObject.SetActive(false);
            UI.groupGameOver.gameObject.SetActive(false);
            UI.txtLevel.text = "LEVEL " + StringHelper.GetNumber(PlayerData.level);
            SetStatusSetting();
        }

        private void SetStatusSetting()
        {
            UI.popupSettings.SetStatus(PlayerData.isSoundOn, PlayerData.isVibrateOn);
            Sound.SetupAudio();
        }

        private void GameOver()
        {
            EventController.RaiseEvent(EventList.GamePlay.PLAY_SOUND, new EventParam(SoundName.VICTORY));
            UI.groupGameOver.gameObject.SetActive(true);
            UI.txtGO.text = TextWinBonus[Random.Range(0, TextWinBonus.Count)];
            if (PlayerData.isVibrateOn)
            {
                MMVibrationManager.Haptic(HapticTypes.Success);
            }
        }
    }
}
