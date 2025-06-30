using System;
using System.Collections.Generic;
using UnityEngine;
using _IECModules;
using System.Linq;
using System.Threading.Tasks;
using _IEC.Features.Common.Extension;
using _IEC.Features.Gameplay;
using DG.Tweening;
using UnityEditor;

namespace IEC
{
    public class GameController : MonoBehaviour, IEventListener, IEventAwake, IEventStart
    {
        protected static InGamePlayGroup GamePlay => PlayableController.Instance.InGamePlayGroup;
        public ScreenInGame ScreenInGame;
        private bool m_IsUpdateTimeResume;
        private bool m_IsPlayWithUnlimitedLives;

      

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
                    EventController.RaiseEvent(EventList.Audio.BACKGROUND_INGAME);
                    OnNewGame();
                    PlayableController.Instance.ConfettiGroup.StopConfetti();
                    //Pool.Get<ConfettiGroup>().StopConfetti();
//                    Services.SetActiveBanner(AdsEnum.BANNER_BOTTOM,true);
                    break;
                case EventList.GamePlay.NEW_GAME_HARD_LEVEL:
                    EventController.RaiseEvent(EventList.Audio.BACKGROUND_INGAME_HARD);
                    OnNewGameHardLevel();
                    PlayableController.Instance.ConfettiGroup.StopConfetti();
                    //Pool.Get<ConfettiGroup>().StopConfetti();
                    break;
                case EventList.GamePlay.LEVEL_WIN:
                    EventController.RaiseEvent(EventList.GamePlay.SET_MUTE_BACKGROUND_MUSIC);
                    EventController.RaiseEvent(EventList.Audio.WIN);
//                    PlayerData.IncreaseBoosterStreak();
   //                 PlayerData.AddProgressToProgressReward(GamePlay.levelController.SpecialProgressRewardObjCollected);
                    // if(GamePlay.IsInLevelHard)
                    //     GamePlay.SetGameState(Locator.Game.ScreenLevelWinHard);
                    // else
                    //GamePlay.IsWin = true;
                    OnGameOver();
                    PlayableController.Instance.ConfettiGroup.ShowConfetti();

                    //Pool.Get<ConfettiGroup>().ShowConfetti();
                    break;
                case EventList.GamePlay.LEVEL_LOSE:
                    EventController.RaiseEvent(EventList.GamePlay.SET_MUTE_BACKGROUND_MUSIC);
                    EventController.RaiseEvent(EventList.Audio.LOSE);

                    //GamePlay.IsWin = false;
                    OnGameOver();
                    break;
                case EventList.GamePlay.LEVEL_SKIP:
                    GamePlay.SetWin();
                    break;
                case EventList.GamePlay.TUTORIAL:
                    break;
                case EventList.GamePlay.TUTORIAL_DONE:
                    break;
                case EventList.Service.PUSH_SHOW_INTER:
                    if (param.p0 != null)
                    {
                        long countShowInter = (long)param.p0;
                        
                    }
                    break;
            }
        }

        public void BehaviourAwake()
        {
            Input.multiTouchEnabled = false;
        }

        public void BehaviourStart()
        {
        
            // Locator.Service.ScreenLoading.ShowLoading(0.5f, () =>
            // {
                // Scene.SendEvent(EventList.GamePlay.GO_MAIN);
                // if (PlayerData.level < Config.levelWinBackHome)
                // {
                //     OpenScene(GameScene.Game);
                // }
                // else
                // {
                //     EventController.RaiseEvent(EventList.Audio.BACKGROUND_HOME);
                // }
                
                //Check reset progress reward when player start game
  
            //});
            // EventController.RaiseEvent(EventList.GamePlay.NEW_GAME);
        }

        public void OnNewGame()
        {
            GamePlay.SetNewLevel();
            ScreenInGame.LoadLevelData(GamePlay.GetLevelController().GetLevelData());
            ScreenInGame.Show();
            GamePlay.StartGamePlay();
            //GamePlay.SetGameState(Locator.Game.ScreenInGame);
            this.RaiseEvent(EventList.GamePlay.RESUME_GAME);
            PushPlayLevelEvent();
        }
   
        void OnNewGameHardLevel()
        {
            GamePlay.SetNewLevelHard();
            PushPlayLevelEvent();
        }

        void OnGameOver()
        {
            // Game.SetTimeEndLevel();
            // if (GamePlay.IsWin)
            // {
            //     PlayerData.AddLevel();
            //     Locator.Main.PopupPiggyBank.AddGoldToPiggyBank(Config.dataPiggyBankConfig
            //         .goldEarnedEachLevel); //CustomGold here
            //     PlayerData.AddScrewBuildCity(Config.buildCityConfig.screwAddEndgame);
            // }
            // else
            // {
            //     PlayerData.DecreasePlayerLife();
            // }
            // EventController.RaiseEvent(EventList.SAVE_DATA);
            PlayableController.Instance.ShowEndCard();
        }

        void PushPlayLevelEvent()
        {
       
            // bool isFirstTracking = PlayerData.levelFirstTimeTracking[Game.crLevelType] == 0;
            //
            // if (isNewLevel || isFirstTracking)
            // {
            //     if (isNewLevel)
            //     {
            //         PlayerData.ClearAndInitTrackingLevelType();
            //         PlayerData.levelFirstTime = level;
            //     }
            //
            //     PlayerData.levelFirstTimeTracking[Game.crLevelType] = 1;
            //     PlayerData.countPlayLevel = 1;
            //     PlayerData.sessionStartLevel = PlayerData.session;
            //
            //  
            //
            // }


       
        }
        
   
        public static void OpenScene(GameScene scene, Action onLoadDone = null, bool raiseFirstEventScene = true)
        {
            string sceneName = EventList.GamePlay.NEW_GAME;
            if (!String.IsNullOrEmpty(sceneName))
            {
                DOTween.KillAll();
                //MeoMew custome tracking level 
   
                // Scene.SendEvent(sceneName,new EventParam(), () =>
                // {
                //     Locator.Service.ScreenLoading.ShowLoading(0.5f, () =>
                //     {
                //         if (scene != GameScene.Game)
                //         {
                //             onLoadDone?.Invoke();
                //             if(raiseFirstEventScene) EventController.RaiseEvent(sceneName);
                //         }
                //     }, () =>
                //     {
                //         if (scene == GameScene.Game)
                //         {
                //             onLoadDone?.Invoke();
                //             if(raiseFirstEventScene) EventController.RaiseEvent(sceneName);
                //         }
                //     });
                // });
            }
        }
        
    
    }
}
public enum GameScene
{
    Game
}
