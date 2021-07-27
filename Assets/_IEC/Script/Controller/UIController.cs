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
            SetEventUI(UI);
        }

        void SetEventUI(UIGroup UI)
        {
            SetEvent(UI.ScreenInGame.btnRestart, () =>
            {
                EventController.RaiseEvent(EventList.GamePlay.END_LEVEL_ADS);
            });

            // test button
            SetEvent(UI.ScreenInGame.btnLevelWin, () =>
            {
                EventController.RaiseEvent(EventList.GamePlay.LEVEL_WIN);
            });

            // test button
            SetEvent(UI.ScreenInGame.btnLevelLose, () =>
            {
                EventController.RaiseEvent(EventList.GamePlay.LEVEL_LOSE);
            });

            SetEvent(UI.ScreenLevelLose.btnRetry, () =>
            {
                EventController.RaiseEvent(EventList.GamePlay.END_LEVEL_ADS);
            });

            SetEvent(UI.ScreenLevelWin.btnNext, () =>
            {
                EventController.RaiseEvent(EventList.GamePlay.END_LEVEL_ADS);
            });
        }

        void SetEvent(UnityEngine.UI.Button button, System.Action action)
        {
            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() =>
                {
                    action?.Invoke();
                });
            }
        }
    }
}