using UnityEngine;
using _IECModules;

namespace IEC
{
    public class GameController : _BaseBehaviour, IController, IEventListener, IEventAwake, IEventStart
    {
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
                case EventList.GamePlay.NEW_GAME:
                    Game.SetGameState(GameState.Playing);
                    OnNewGame();
                    break;
                case EventList.GamePlay.LEVEL_WIN:
                    Game.SetGameState(GameState.Win);
                    OnGameOver();
                    break;
                case EventList.GamePlay.LEVEL_LOSE:
                    Game.SetGameState(GameState.Lose);
                    OnGameOver();
                    break;
            }
        }

        public void BehaviourAwake()
        {
        }

        public void BehaviourStart()
        {
            EventController.RaiseEvent(EventList.GamePlay.NEW_GAME);
        }

        void OnNewGame()
        {
            Game.SetTimeStartLevel();
            PlayerData.levelTracking = PlayerData.level;

            PushPlayLevelEvent();
        }

        void OnGameOver()
        {
            Game.SetTimeEndLevel();
            if (Game.IsWin)
            {
                PlayerData.AddLevel();
            }
            EventController.RaiseEvent(EventList.SAVE_DATA);
            Services.PushEvent(new TrackingEvent.GameOver());
        }

        void PushPlayLevelEvent()
        {
            var level = PlayerData.levelTracking;
            Services.PushEvent(new TrackingEvent.PlayLevel());

            if (PlayerData.levelFirstTime != level)
            {
                PlayerData.levelFirstTime = level;
                Services.PushEvent(new TrackingEvent.PlayLevelFirstTime());
                Services.SetUserProperty("Level", level.ToString());
            }

            if (level % Mathf.FloorToInt(Mathf.Pow(10, level.ToString().Length - 1)) == 0)
                Services.PushEvent($"play_level_{level}");
        }
    }
}
