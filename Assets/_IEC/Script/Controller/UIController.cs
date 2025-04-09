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
                case EventList.GamePlay.APPLY_AB_TEST:
                    break;
                case EventList.GamePlay.NEW_GAME:
                    UI.ShowScreen(Locator.Game.ScreenInGame);
                    //Locator.Game.ScreenInGame.SetLevel(PlayerData.level);
                    break;
                case EventList.GamePlay.NEW_GAME_HARD_LEVEL:
                    UI.ShowScreen(Locator.Game.ScreenInGame);
                    //Locator.Game.ScreenInGame.SetLevelHard();
                    break;
                case EventList.GamePlay.LEVEL_WIN:
                    // UI.ShowScreen(UI.ScreenLevelWin);
                    if(GamePlay.IsInLevelHard)
                        UI.ShowScreen(Locator.Game.ScreenLevelWinHard);
                    else
                        UI.ShowScreen(Locator.Game.ScreenLevelWin);
                    break;
                case EventList.GamePlay.LEVEL_LOSE:
                    // UI.ShowScreen(UI.ScreenLevelLose);
                    UI.ShowScreen(Locator.Game.ScreenLevelLose);
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