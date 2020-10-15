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
                    UI.ShowInGame();
                    break;
                case EventList.GamePlay.LEVEL_WIN:
                    UI.ShowLevelWin();
                    break;
                case EventList.GamePlay.LEVEL_LOSE:
                    UI.ShowLevelLose();
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
            SetEvent(UI.screenInGame.btnRestart, () =>
            {
                EventController.RaiseEvent(EventList.GamePlay.END_LEVEL_ADS);
            });

            // test button
            SetEvent(UI.screenInGame.btnLevelWin, () =>
            {
                EventController.RaiseEvent(EventList.GamePlay.LEVEL_WIN);
            });

            // test button
            SetEvent(UI.screenInGame.btnLevelLose, () =>
            {
                EventController.RaiseEvent(EventList.GamePlay.LEVEL_LOSE);
            });

            SetEvent(UI.screenLevelLose.btnRetry, () =>
            {
                EventController.RaiseEvent(EventList.GamePlay.END_LEVEL_ADS);
            });

            SetEvent(UI.screenLevelWin.btnNext, () =>
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