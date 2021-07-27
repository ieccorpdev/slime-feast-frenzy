using UnityEngine;

namespace IEC
{
    public class UIController : _BaseBehaviour, IController, IEventListener, IEventAwake, IEventStart
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
                    UI.ShowScreen(UI.ScreenInGame);
                    UI.ScreenInGame.SetLevel(PlayerData.level);
                    break;
                case EventList.GamePlay.LEVEL_WIN:
                    UI.ShowScreen(UI.ScreenLevelWin);
                    break;
                case EventList.GamePlay.LEVEL_LOSE:
                    UI.ShowScreen(UI.ScreenLevelLose);
                    break;
            }
        }

        public void BehaviourAwake()
        {
        }

        public void BehaviourStart()
        {
        }
    }
}