using MoreMountains.NiceVibrations;
using UnityEngine;

namespace IEC
{
    public class UIController : _BaseBehaviour, IController, IEventListener, IEventAwake, IEventUpdate
    {
        private float m_CountVibrateHint, m_NextTimeVibrate;

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
            UI.OnInit();
            InitButtonEvent();
        }

        public void OnEventRaise(string gameEvent, EventParam param)
        {
            switch (gameEvent)
            {
                case EventList.GamePlay.GAME_OVER_UI:
                    GameOver((bool)param.p0);
                    break;
                case EventList.GamePlay.GAME_START_PLAY:
                    if (Game.isHaveUIAnimation)
                    {
                        UI.mainUIGroup.TweenPannelTop(true);
                    }

                    if (Game.isBossLevelLimitMoves)
                    {
                        UI.mainUIGroup.TweenBossLevel();
                    }

                    if (!PlayerData.haveAlreadyRate)
                    {
                        var level = PlayerData.level;
                        if (level <= 60 && level % 20 == 0 &&
                            !PlayerData.levelShowRate.Contains(level))
                        {
                            PlayerData.levelShowRate.Add(level);
                            Services.PushEvent(new IEC.TrackingEvent.ShowPopupRating());
                            UI.popUpRating.gameObject.SetActive(true);
                        }
                    }
                    break;
                case EventList.GamePlay.GAME_ADD_MOVE:
                    var grid = (BaseComponent)param.sender;
                    Game.AddUndoData(grid.Row, grid.Col);
                    Game.moveRemains -= 1;
                    UI.mainUIGroup.UpdateMove();
                    Game.moveCount += 1;
                    break;
            }
        }

        public void BehaviourUpdate(float deltaTime)
        {
            if (!Game.isHaveUIAnimation) return;
            m_CountVibrateHint += deltaTime;
            if(m_CountVibrateHint >= m_NextTimeVibrate)
            {
                UI.mainUIGroup.OnVibrateHint();
                ResetVibrateHint();
            }
        }

        private void InitButtonEvent()
        {

            UI.mainUIGroup.btnRestart.onClick.AddListener(() => {
                if (Game.isBlockButton) return;
                Game.isPlaying = false;
                Services.PushEvent(new IEC.TrackingEvent.ReplayLevel());
                EventController.RaiseEvent(EventList.GamePlay.GAME_RESTART);
                if (PlayerData.isVibrateOn)
                {
                    MMVibrationManager.Haptic(HapticTypes.Failure);
                }
            });

            UI.mainUIGroup.btnSkip.onClick.AddListener(() => {
                if (Game.isBlockButton) return;
                Services.ShowRewardAds(() => {
                    Services.PushEvent(new IEC.TrackingEvent.SkipLevel());
                    PlayerData.level += 1;
                    EventController.RaiseEvent(EventList.GamePlay.GAME_SPAWN_LEVEL);
                });
            });


            UI.mainUIGroup.btnSetting.onClick.AddListener(() => {
                if (Game.isBlockButton) return;
                UI.popupSettings.gameObject.SetActive(true);
                Game.isShowPopup = true;
            });

            UI.mainUIGroup.btnHint.onClick.AddListener(() => {
                if (Game.isBlockButton) return;
                OnHandleButtonHint();
            });

            UI.mainUIGroup.btnUndo.onClick.AddListener(OnHandleUndo);
        }

        private void OnHandleUndo()
        {
            if (Game.listUndo.Count < 1) return;
            if(Game.undoRemains > 0)
            {
                if (Game.IsAllowUndo())
                {
                    EventController.RaiseEvent(EventList.GamePlay.GAME_UNDO);
                    Game.undoRemains -= 1;
                    UI.mainUIGroup.UpdateUndoText();
                    Game.moveRemains += 1;
                    UI.mainUIGroup.UpdateMove();

                    Services.PushEvent(new IEC.TrackingEvent.UndoLevel());
                }
            }
            else
            {
                Services.ShowRewardAds(() => {
                    Game.undoRemains = 3;
                    UI.mainUIGroup.UpdateUndoText();
                    Services.PushEvent(new IEC.TrackingEvent.ShowRewardUndo());
                });
            }
        }

        private void OnHandleButtonHint()
        {
            if (Game.isHintStyleV3)
            {
                if (Game.hintRemains > 0)
                {
                    Game.hintRemains -= 1;
                    UI.mainUIGroup.UpdateHintText();
                    OnHandleHint(false);
                }
                else
                {
                    Services.ShowRewardAds(() => {
                        Game.hintRemains = 3;
                        UI.mainUIGroup.UpdateHintText();
                        Services.PushEvent(new IEC.TrackingEvent.HintLevel());
                    });
                }
            }
            else
            {
                Services.ShowRewardAds(() => {
                    Game.isWaitAfterRV = true;
                    OnHandleHint(true);
                    Services.PushEvent(new IEC.TrackingEvent.HintLevel());
                });
            }
        }

        private void OnHandleHint(bool isDelay)
        {
            var delayTime = (Application.isEditor || Services.IsConfigNoAds()) ? 0f : 1f;
            if (!isDelay)
            {
                delayTime = 0f;
            }
            EventController.RaiseEventDelay(delayTime, EventList.GamePlay.GAME_START_HINT);
        }

        public void SetActiveButtons(bool isActive)
        {
            UI.mainUIGroup.SetActiveTopButtons(isActive);
        }

        public void SetupAbUI()
        {
            UI.mainUIGroup.SetupAbUI();
        }

        public void OnResetAll()
        {
            if (!Game.isHaveUIAnimation)
            {
                UI.mainUIGroup.TweenPannelTop(true);
            }

            UI.OnResetAll();
            UI.mainUIGroup.UpdateUndoText();
            UI.mainUIGroup.UpdateHintText();
            Game.isShowPopup = false;
            SetActiveButtons(true);

            ResetVibrateHint();
        }

        public void SetLevel(bool isBossLevel)
        {
            UI.mainUIGroup.SetupBossLevel(isBossLevel);
            UI.txtLevel.text = "LEVEL " + StringHelper.GetNumber(PlayerData.level);
        }

        private void GameOver(bool isWin)
        {
            EventController.RaiseEvent(EventList.GamePlay.PLAY_SOUND, new EventParam(SoundName.VICTORY));
            UI.OnGameOver(true);
            SetActiveButtons(true);

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
