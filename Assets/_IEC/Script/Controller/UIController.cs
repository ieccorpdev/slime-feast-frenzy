using MoreMountains.NiceVibrations;
using UnityEngine;

namespace IEC
{
    public class UIController : _BaseBehaviour, IController, IEventListener, IEventAwake, IEventUpdate
    {
        private float m_CountVibrateHint, m_NextTimeVibrate;

        private int m_TimeLeft, m_TimeLeftUI;
        private bool m_IsWatchedAdsAddTime;
        private bool m_IsShowGOPanel;

        private bool m_IsAllowCountDown;

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
                    if (m_IsShowGOPanel) return;
                    m_IsShowGOPanel = true;
                    GameOver((bool)param.p0);
                    break;
                case EventList.GamePlay.GAME_START_PLAY:
                    if (!PlayerData.haveAlreadyRate)
                    {
                        var level = PlayerData.level;
                        if (level <= 60 && level % 20 == 0 &&
                            !PlayerData.levelShowRate.Contains(level))
                        {
                            Game.isShowPopup = true;
                            PlayerData.levelShowRate.Add(level);
                            Services.PushEvent(new IEC.TrackingEvent.ShowPopupRating());
                            UI.popUpRating.gameObject.SetActive(true);
                            Game.SetGameState(GameState.Pause);
                        }
                    }

                    if (Game.isHaveUIAnimation)
                    {
                        UI.mainUIGroup.TweenPannelTop(true);
                    }

                    if (Game.isBossLevel)
                    {
                        if (!Game.isShowPopup)
                        {
                            TweenBossLevel();
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
            if (!Game.isPlaying) return;
            if (!m_IsAllowCountDown) return;
            if (Game.isHaveUIAnimation)
            {
                m_CountVibrateHint += deltaTime;
                if (m_CountVibrateHint >= m_NextTimeVibrate)
                {
                    UI.mainUIGroup.OnVibrateHint();
                    ResetVibrateHint();
                }
            }

            if (Game.isHaveBossTimerLevels && Game.isBossLevel)
            {
                Game.timeLeftBossLevel -= deltaTime;

                UpdateTimerLevel();

                if (Game.timeLeftBossLevel <= 0f)
                {
                    GameController.GameOver(false);
                }else if((Game.timeLeftBossLevel <= Game.timeShowRVAddTime) && !m_IsWatchedAdsAddTime)
                {
                    UI.mainUIGroup.OnEffectTime(true);
                }
            }
        }

        private void InitButtonEvent()
        {

            UI.mainUIGroup.btnRestart.onClick.AddListener(() => {
                if (Game.isBlockButton) return;
                Game.SetGameState(GameState.Init);
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

            UI.mainUIGroup.btnTime.onClick.AddListener(OnHandleAddTime);
        }

        public void TweenBossLevel()
        {
            if (Game.isBossLevel)
            {
                Game.isShowPopup = true;
                UI.mainUIGroup.TweenBossLevel(() => {
                    m_IsAllowCountDown = true;
                    Game.isShowPopup = false;
                });
            }
            else
            {
                Game.isShowPopup = false;
            }
            
        }

        private void OnHandleAddTime()
        {
            if (Game.isBlockButton) return;
            if (m_IsWatchedAdsAddTime) return;
            m_IsWatchedAdsAddTime = true;

            Services.ShowRewardAds(() =>
            {
                Game.timeLeftBossLevel = Mathf.Clamp(Game.timeLeftBossLevel + 20, 20, Game.timeMaxBossLevel);
                UI.mainUIGroup.OnEffectTime(Game.timeLeftBossLevel <= Game.timeShowRVAddTime);
            });
        }

        private void OnHandleUndo()
        {
            if (Game.isBlockButton) return;
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

            m_IsWatchedAdsAddTime = false;
            m_IsShowGOPanel = false;
            m_IsAllowCountDown = false;
        }

        public void SetLevel(bool isBossLevel)
        {
            UI.mainUIGroup.SetupBossLevel(isBossLevel);
            UI.txtLevel.text = "LEVEL " + StringHelper.GetNumber(PlayerData.level);
        }

        private void GameOver(bool isWin)
        {
            UI.mainUIGroup.OnEffectTime(false);
            UI.gameOverUIGroup.SetTextGameOver(isWin);
            EventController.RaiseEvent(EventList.GamePlay.PLAY_SOUND, new EventParam(SoundName.VICTORY));
            
            SetActiveButtons(true);

            if (PlayerData.isVibrateOn)
            {
                MMVibrationManager.Haptic(isWin ? HapticTypes.Success : HapticTypes.Failure);
            }

            Invoke(nameof(ShowGameOver), .2f);
        }

        private void ShowGameOver()
        {
            UI.OnGameOver(true);
        }

        private void ResetVibrateHint()
        {
            m_CountVibrateHint = 0f;
            m_NextTimeVibrate = Random.Range(5f, 10f);
        }

        public void UpdateTimerLevel()
        {
            m_TimeLeft = (int)Game.timeLeftBossLevel;

            if (m_TimeLeft != m_TimeLeftUI)
            {
                m_TimeLeftUI = m_TimeLeft;
                UI.mainUIGroup.SetTextTimer(m_TimeLeftUI);
            }
        }
    }
}
