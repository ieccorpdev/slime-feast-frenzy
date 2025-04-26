using System;
using System.Collections.Generic;
using UnityEngine;
using _IECModules;
using System.Linq;
using System.Threading.Tasks;
using _IEC.Features.Common.Extension;
using _IEC.Features.Gameplay;
using DG.Tweening;
using MoreMountains.NiceVibrations;
using UnityEditor;

namespace IEC
{
    public class GameController : _BaseBehaviour, IController, IEventListener, IEventAwake, IEventStart
    {
        private List<int> m_ListLevelAdjust = new List<int>();
        private List<int> m_ListLevelFirstTimeAdjust = new List<int>();
        public List<int> ListEventInterAdjust = new List<int>();
        public bool isInitABDone = false;
        private bool m_IsUpdateTimeResume;
        private bool m_IsPlayWithUnlimitedLives;
        private LifeConfig m_LifeConfig => Config.hybridConfig.lifeConfig;
        public void LocateController()
        {
            Controllers.Add(this);
        }

        public bool IsActive()
        {
            return gameObject.activeSelf;
        }

        public void OnEventRaise(string gameEvent, EventParam param)
        {
            switch (gameEvent)
            {
                case EventList.GamePlay.APPLY_AB_TEST:
                    break;
                case EventList.GamePlay.NEW_GAME:
                    GamePlay.SetGameState(Locator.Game.ScreenInGame);
                    OnNewGame();
                    Pool.Get<ConfettiGroup>().StopConfetti();
                    Services.SetActiveBanner(AdsEnum.BANNER_BOTTOM,false);
                    break;
                case EventList.GamePlay.NEW_GAME_HARD_LEVEL:
                    EventController.RaiseEvent(EventList.Audio.BACKGROUND_INGAME_HARD);
                    GamePlay.SetGameState(Locator.Game.ScreenInGame);
                    OnNewGameHardLevel();
                    Pool.Get<ConfettiGroup>().StopConfetti();
                    Services.SetActiveBanner(AdsEnum.BANNER_BOTTOM,false);
                    break;
                case EventList.GamePlay.LEVEL_WIN:
                    EventController.RaiseEvent(EventList.GamePlay.SET_MUTE_BACKGROUND_MUSIC);
                    EventController.RaiseEvent(EventList.Audio.WIN);
                    PlayerData.IncreaseBoosterStreak();
                    PlayerData.AddProgressToProgressReward(GamePlay.levelController.SpecialProgressRewardObjCollected);
                    if(GamePlay.IsInLevelHard)
                        GamePlay.SetGameState(Locator.Game.ScreenLevelWinHard);
                    else
                        GamePlay.SetGameState(Locator.Game.ScreenLevelWin);
                    //GamePlay.IsWin = true;
                    OnGameOver();
                    Pool.Get<ConfettiGroup>().ShowConfetti();
                    PlayerData.Vibrate(HapticTypes.Success);
                    Services.SetActiveBanner(AdsEnum.BANNER_BOTTOM,false);
                    break;
                case EventList.GamePlay.LEVEL_LOSE:
                    EventController.RaiseEvent(EventList.GamePlay.SET_MUTE_BACKGROUND_MUSIC);
                    EventController.RaiseEvent(EventList.Audio.LOSE);

                    GamePlay.SetGameState(Locator.Game.ScreenLevelLose);
                    //GamePlay.IsWin = false;
                    OnGameOver();
                    PlayerData.Vibrate(HapticTypes.LightImpact);
                    Services.SetActiveBanner(AdsEnum.BANNER_BOTTOM,false);
                    break;
                case EventList.GamePlay.LEVEL_SKIP:
                    GamePlay.SetWin();
                    break;
                case EventList.GamePlay.TUTORIAL:
                    Locator.Game.ScreenInGame.btnSetting.gameObject.SetActive(false);
                    break;
                case EventList.GamePlay.TUTORIAL_DONE:
                    Locator.Game.ScreenInGame.btnSetting.gameObject.SetActive(true);
                    break;
                case EventList.Service.PUSH_SHOW_INTER:
                    if (param.p0 != null)
                    {
                        long countShowInter = (long)param.p0;
                        if (ListEventInterAdjust.Contains((int)countShowInter))
                        {
                            Controllers.Get<ServicesController>().PushEvent("inter_" + countShowInter);
                        }
                    }
                    break;
            }
        }

        public void BehaviourAwake()
        {
            Input.multiTouchEnabled = false;
            Pool.Get<GameConfig>().PreloadAllConfig();
        }

        public void BehaviourStart()
        {
            if (Services.config.adjustEventConfig != null)
            {
                m_ListLevelAdjust = Services.config.adjustEventConfig.GetListPlayLevel();
                m_ListLevelFirstTimeAdjust = Services.config.adjustEventConfig.GetListPlayLevelFirstTime();
                ListEventInterAdjust = Services.config.adjustEventConfig.GetListInter();
            }
            // Locator.Service.ScreenLoading.ShowLoading(0.5f, () =>
            // {
                // Scene.SendEvent(EventList.GamePlay.GO_MAIN);
                if (PlayerData.level < Config.levelWinBackHome)
                {
                    OpenScene(GameScene.Game);
                }
                else
                {
                    OpenScene(GameScene.Main);
                    EventController.RaiseEvent(EventList.Audio.BACKGROUND_HOME);
                }
                
                //Check reset progress reward when player start game
                PlayerData.CheckResetTimeProgressReward();
  
            //});
            // EventController.RaiseEvent(EventList.GamePlay.NEW_GAME);
        }

        void OnNewGame()
        {
            Game.SetTimeStartLevel();
            GamePlay.SetNewLevel(PlayerData.level);
            Locator.Game.ScreenInGame.LoadLevelData(GamePlay.GetLevelController().GetLevelData());
            Locator.Game.ScreenInGame.Show();
            GamePlay.StartGamePlay();
            GamePlay.SetGameState(Locator.Game.ScreenInGame);
            this.RaiseEvent(EventList.GamePlay.RESUME_GAME);
            PushPlayLevelEvent();
        }

        void OnNewGameHardLevel()
        {
            Game.SetTimeStartLevel();
            GamePlay.SetNewLevelHard();
            PushPlayLevelEvent();
        }

        void OnGameOver()
        {
            Game.SetTimeEndLevel();
            if (GamePlay.IsWin)
            {
                PlayerData.AddLevel();
                Locator.Main.PopupPiggyBank.AddGoldToPiggyBank(Config.dataPiggyBankConfig
                    .goldEarnedEachLevel); //CustomGold here
                PlayerData.AddScrewBuildCity(Config.buildCityConfig.screwAddEndgame);
            }
            else
            {
                PlayerData.DecreasePlayerLife();
            }
            EventController.RaiseEvent(EventList.SAVE_DATA);
            Services.PushEvent(new TrackingEvent.GameOver());
            Services.PushEvent(new TrackingEvent.PlayLevelDetail());
        }

        void PushPlayLevelEvent()
        {
            PlayerData.countPlayLevel++;
            PlayerData.levelTracking = PlayerData.level;

            var level = PlayerData.levelTracking;

            bool isNewLevel = PlayerData.levelFirstTime != level;
            bool isFirstTracking = PlayerData.levelFirstTimeTracking[Game.crLevelType] == 0;

            if (isNewLevel || isFirstTracking)
            {
                if (isNewLevel)
                {
                    PlayerData.ClearAndInitTrackingLevelType();
                    PlayerData.levelFirstTime = level;
                }

                PlayerData.levelFirstTimeTracking[Game.crLevelType] = 1;
                PlayerData.countPlayLevel = 1;
                PlayerData.sessionStartLevel = PlayerData.session;

                Services.PushEvent(new TrackingEvent.PlayLevelFirstTime());

                if (m_ListLevelFirstTimeAdjust.Contains(level))
                {
                    Services.PushEvent($"play_level_first_time_{level}");
                }

                Services.SetUserProperty("Level", level.ToString());
            }

            Services.PushEvent(new TrackingEvent.PlayLevel());

            if (level % Mathf.FloorToInt(Mathf.Pow(10, level.ToString().Length - 1)) == 0 || m_ListLevelAdjust.Contains(level))
                Services.PushEvent($"play_level_{level}");

            EventController.RaiseEvent(EventList.SAVE_DATA);
        }
        
        private void UpdateTimeLeftApplicationResume()
        {
            m_IsUpdateTimeResume = true;
            var lastTime = new DateTime(PlayerData.startTickPlayTime);
            var timeSpanSec = (int)((DateTime.Now - lastTime).TotalSeconds);

            PlayerData.hybridData.AddResource(ResourceType.UnlimitedLive, -timeSpanSec);

            if (timeSpanSec < PlayerData.hybridData.timeRefillLeft)
            {
                PlayerData.hybridData.timeRefillLeft -= timeSpanSec;
            }
            else
            {
                var remainSec = timeSpanSec - PlayerData.hybridData.timeRefillLeft;
                PlayerData.hybridData.AddResource(ResourceType.Live, 1);
                while (remainSec > m_LifeConfig.timeRefill)
                {
                    remainSec -= m_LifeConfig.timeRefill;
                    PlayerData.hybridData.AddResource(ResourceType.Live, 1);
                }

                PlayerData.hybridData.timeRefillLeft = m_LifeConfig.timeRefill - remainSec;
            }
            EventController.RaiseEvent(EventList.GamePlay.UPDATE_RESOURCE_UI,
                new EventParam { p0 = ResourceType.Live });
            m_IsUpdateTimeResume = false;
        }
        public static void OpenScene(GameScene scene, Action onLoadDone = null, bool raiseFirstEventScene = true)
        {
            string sceneName = scene switch
            {
                GameScene.Main => EventList.GamePlay.GO_MAIN,
                GameScene.Game => EventList.GamePlay.NEW_GAME,
                _ => ""
            };
            if (!String.IsNullOrEmpty(sceneName))
            {
                DOTween.KillAll();
                Locator.Service.ScreenLoading.ShowLoading();
                //MeoMew custome tracking level 
                if (scene == GameScene.Main)
                {
                    //Reset tracking to None when back home 
                    Game.crLevelType = LevelType.NONE;
                }
                Scene.SendEvent(sceneName,new EventParam(), () =>
                {
                    Locator.Service.ScreenLoading.ShowLoading(0.5f, () =>
                    {
                        //if (scene != GameScene.Game)
                        {
                            onLoadDone?.Invoke();
                            if(raiseFirstEventScene) EventController.RaiseEvent(sceneName);
                        }
                    });
                });
            }
        }
        
        private void UpdateTimeLifePerSec()
        {
            if (m_IsUpdateTimeResume) return;
            if (PlayerData.hybridData.GetResourceCount(ResourceType.UnlimitedLive) > 0)
            {
                PlayerData.hybridData.AddResource(ResourceType.UnlimitedLive, -1);
            }
            if (PlayerData.hybridData.GetResourceCount(ResourceType.Live) >= m_LifeConfig.maxLife) return;
            PlayerData.hybridData.timeRefillLeft--;
            if (PlayerData.hybridData.timeRefillLeft <= 0)
            {
                PlayerData.hybridData.timeRefillLeft = Config.hybridConfig.lifeConfig.timeRefill;
                PlayerData.hybridData.AddResource(ResourceType.Live, 1);
            }
            EventController.RaiseEvent(EventList.GamePlay.UPDATE_RESOURCE_UI,
                new EventParam { p0 = ResourceType.Live });
        }
    }
}
public enum GameScene
{
    Loading,
    Service,
    Main,
    Game
}
