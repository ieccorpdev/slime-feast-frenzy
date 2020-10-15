using UnityEngine;

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
                    break;
                case EventList.GamePlay.LEVEL_WIN:
                    Game.SetGameState(GameState.Win);
                    break;
                case EventList.GamePlay.LEVEL_LOSE:
                    Game.SetGameState(GameState.Lose);
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
    }
}