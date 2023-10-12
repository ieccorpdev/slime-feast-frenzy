using System.Collections.Generic;
using UnityEngine;
using _IECModules;

namespace IEC
{
    public class GameController : _BaseBehaviour, IController, IEventListener, IEventAwake, IEventStart
    {
        private List<int> m_ListLevelAdjust = new List<int>();
        private List<int> m_ListLevelFirstTimeAdjust = new List<int>();
        public List<int> ListEventInterAdjust = new List<int>();
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
                    GamePlay.SetGameState(UI.ScreenInGame);
                    OnNewGame();
                    break;
                case EventList.GamePlay.LEVEL_WIN:
                    GamePlay.SetGameState(UI.ScreenLevelWin);
                    OnGameOver();
                    Pool.Get<ConfettiGroup>().ShowConfetti();
                    break;
                case EventList.GamePlay.LEVEL_LOSE:
                    GamePlay.SetGameState(UI.ScreenLevelLose);
                    OnGameOver();
                    break;
                case EventList.GamePlay.LEVEL_SKIP:
                    GamePlay.SetFinishPlay();
                    break;
                case EventList.Service.PUSH_SHOW_INTER:
                    if (param.p0 != null)
                    {
                        long countShowInter = (long) param.p0;
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
        }

        public void BehaviourStart()
        {
            if (Services.config.adjustEventConfig != null)
            {
                m_ListLevelAdjust = Services.config.adjustEventConfig.GetListPlayLevel();
                m_ListLevelFirstTimeAdjust = Services.config.adjustEventConfig.GetListPlayLevelFirstTime();
                ListEventInterAdjust = Services.config.adjustEventConfig.GetListInter();
            }
            EventController.RaiseEvent(EventList.GamePlay.NEW_GAME);
        }

        void OnNewGame()
        {
            Game.SetTimeStartLevel();
            
            PushPlayLevelEvent();

            GamePlay.SetNewLevel(PlayerData.level);
        }

        void OnGameOver()
        {
            Game.SetTimeEndLevel();
            if (GamePlay.IsWin)
            {
                PlayerData.AddLevel();
            }
            EventController.RaiseEvent(EventList.SAVE_DATA);
            Services.PushEvent(new TrackingEvent.GameOver());
        }

        void PushPlayLevelEvent()
        {
            PlayerData.countPlayLevel++;
            PlayerData.levelTracking = PlayerData.level;

            var level = PlayerData.levelTracking;
            
            if (PlayerData.levelFirstTime != level)
            {
                PlayerData.levelFirstTime = level;
                PlayerData.countPlayLevel = 1;
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
    }
}
