using UnityEngine;
using _IECModules;

namespace IEC
{
    public class CheatCodeController : _BaseBehaviour, IController, IEventListener, IEventAwake, IEventStart
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
                case EventList.NONE:
                    Debug.Log("event none");
                    break;
            }
        }

        public void BehaviourAwake()
        {
        }

        public void BehaviourStart()
        {
            ModuleController.cheatCode.AddCheatCodeButton("RESET Player Data!!!!!", cheatCodeParam =>
            {
                Game.isDebugReset = true;
                PlayerPrefs.DeleteAll();
                Application.Quit();

#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
            });

            ModuleController.cheatCode.AddCheatCodeButton("Toggle NoAds", cheatCodeParam =>
            {
                var data = Pool.Get<ServiceData>();
                Services.ActiveNoAds(!data.isNoAds);
                data.isNoAdsReward = data.isNoAds;

                cheatCodeParam.buttonText = "NoAds: " + data.isNoAds;
            });

            ModuleController.cheatCode.AddCheatCodeButtonWithInput("Set Level", param =>
            {
                Game.isCheatLevel = true;
                int level = int.Parse(param.inputParams[0].value.ToString());
                Pool.Get<PlayerSecret>().level = level;
                OnRestartLevel();
            }, new CheatCode.InputParam
            {
                value = 1
            });

            ModuleController.cheatCode.AddCheatCodeButtonWithInput("Set Background(0-5)", param =>
            {
                int bgIndex = int.Parse(param.inputParams[0].value.ToString());
                Game.backGroundIndex = bgIndex % 6;
                OnRestartLevel();
            }, new CheatCode.InputParam
            {
                value = Game.backGroundIndex
            });

            ModuleController.cheatCode.AddCheatCodeButtonWithInput("Set Theme(0-5)", param =>
            {
                int themeIndex = int.Parse(param.inputParams[0].value.ToString());
                Game.themeIndex = themeIndex % 6;
                OnRestartLevel();
            }, new CheatCode.InputParam
            {
                value = Game.themeIndex
            });
        }

        private void OnRestartLevel()
        {
            EventController.RaiseEvent(EventList.GamePlay.GAME_SPAWN_LEVEL);
        }

    }
}