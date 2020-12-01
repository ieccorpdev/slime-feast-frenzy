using MoreMountains.NiceVibrations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public class UIController : _BaseBehaviour, IController, IEventListener, IEventAwake, IEventUpdate
    {
        private float m_CountVibrateHint, m_NextTimeVibrate;

        [SerializeField]
        private Gradient m_MoveColor;

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
                case EventList.GamePlay.GAME_OVER_UI:
                    GameOver((bool)param.p0);
                    break;
                case EventList.GamePlay.GAME_START:
                    if (Game.isUIEffect)
                    {
                        UI.TweenPannelTop(true);
                    }
                    break;
                case EventList.GamePlay.GAME_ADD_MOVE:
                    var grid = (BaseComponent)param.sender;
                    Game.AddUndoData(grid.Row, grid.Col);
                    Game.moveRemains -= 1;
                    UpdateMove();
                    break;
            }
        }

        public void BehaviourUpdate(float deltaTime)
        {
            if (!Game.isUIEffect) return;
            m_CountVibrateHint += deltaTime;
            if(m_CountVibrateHint >= m_NextTimeVibrate)
            {
                UI.OnVibrateHint();
                ResetVibrateHint();
            }
        }

        private void InitButtonEvent()
        {
            UI.btnRestart.onClick.AddListener(() => {
                if (Game.isBlockButton) return;
                Game.isPlaying = false;
                Services.PushEvent(new IEC.TrackingEvent.ReplayLevel());
                EventController.RaiseEvent(EventList.GamePlay.GAME_RESTART);
                if (PlayerData.isVibrateOn)
                {
                    MMVibrationManager.Haptic(HapticTypes.Failure);
                }
            });

            UI.btnSkip.onClick.AddListener(() => {
                if (Game.isBlockButton) return;
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
                if (Game.isBlockButton) return;
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

            UI.btnHint.onClick.AddListener(() => {
                if (Game.isBlockButton) return;
                Services.ShowRewardAds(() => {
                    Game.isWaitAfterRV = true;
                    var delayTime = (Application.isEditor || Services.IsConfigNoAds()) ? 0f : 1f;
                    EventController.RaiseEventDelay(delayTime, EventList.GamePlay.GAME_START_HINT);
                });
            });

            UI.btnUndo.onClick.AddListener(OnHandleUndo);
        }

        private void OnHandleUndo()
        {
            if (Game.listUndo.Count < 1) return;
            if(Game.undoCount > 0)
            {
                if (Game.IsAllowUndo())
                {
                    EventController.RaiseEvent(EventList.GamePlay.GAME_UNDO);
                    Game.undoCount -= 1;
                    UpdateUndoText();

                    Game.moveRemains += 1;
                    UpdateMove();
                }
            }
            else
            {
                Services.ShowRewardAds(() => {
                    Game.undoCount = 3;
                    UpdateUndoText();
                });
            }
        }

        private void UpdateUndoText()
        {
            UI.txtUndo.text = StringHelper.GetNumber(Game.undoCount);
            UI.undoAds.SetActive(Game.undoCount <= 0);
            UI.undoCount.SetActive(Game.undoCount > 0);
        }

        public void UpdateMove()
        {
            UI.txtMove.text = "Move: " + StringHelper.GetNumber(Game.moveRemains);
            float valueColor = (float)Game.moveRemains / Game.totalMove;
            UI.txtMove.color = m_MoveColor.Evaluate(valueColor);
        }

        public void SetActiveButtons(bool isActive)
        {
            UI.btnSetting.interactable = isActive;
            UI.btnSkip.interactable = isActive;
            UI.btnRestart.interactable = isActive;
        }

        public void SetupAbUI()
        {
            UI.btnSetting.gameObject.SetActive(Game.isHaveSetting);
            UI.btnSkip.gameObject.SetActive(!Game.isHaveHint);
            SetStatusSetting();
            Sound.SetupAudio();
        }

        public void OnResetAll()
        {
            if (!Game.isUIEffect)
            {
                UI.TweenPannelTop(true);
            }
            SetActiveButtons(true);
            Game.isShowPopup = false;
            UI.popupSettings.gameObject.SetActive(false);
            UI.TweenGameOver(false);
            SetStatusSetting();
            ResetVibrateHint();

            UpdateUndoText();
        }

        public void SetupBossLevel(bool isBossLevel)
        {
            UI.btnHint.gameObject.SetActive(Game.isHaveHint && !isBossLevel);
            UI.btnUndo.gameObject.SetActive(isBossLevel);
            UI.txtMove.gameObject.SetActive(isBossLevel);

            UI.txtLevel.text = isBossLevel ? "BOSS LEVEL" : ("LEVEL " + StringHelper.GetNumber(PlayerData.level));
        }

        private void SetStatusSetting()
        {
            UI.popupSettings.SetStatus(PlayerData.isSoundOn, PlayerData.isVibrateOn);
            Sound.SetupAudio();
        }

        private void GameOver(bool isWin)
        {
            EventController.RaiseEvent(EventList.GamePlay.PLAY_SOUND, new EventParam(SoundName.VICTORY));
            UI.TweenGameOver(true);
            SetActiveButtons(true);
            UI.txtGO.text = isWin ? TextWinBonus[Random.Range(0, TextWinBonus.Count)] : "Mission Fail!";

            if (PlayerData.isVibrateOn)
            {
                MMVibrationManager.Haptic(isWin ? HapticTypes.Success : HapticTypes.Failure);
            }
        }

        private void ResetVibrateHint()
        {
            m_CountVibrateHint = 0f;
            m_NextTimeVibrate = Random.Range(5f, 10f);
        }
    }
}
