using _IECModules;
using System.Linq;
using UnityEngine;

namespace IEC
{
    public class ABTestController : _BaseBehaviour, IController, IEventListener, IEventAwake, IEventStart
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
                    break;
            }
        }

        public void BehaviourAwake()
        {
        }

        public void BehaviourStart()
        {
            var abTestConfigs = Services.config.abTestConfigs;
            if (abTestConfigs.Length == 0) return;
            var abTestConfig = Services.config.abTestConfigs.Last();

            ResultAbFullAds();
            Result20201103();

            var abValue = Services.ABTest().GetValue(abTestConfig.KeyName, 0);
            UpdateAb20201106(abValue);

            ModuleController.cheatCode.AddCheatCodeButtonWithDropdown(abTestConfig.KeyName,
                cheatCodeParam =>
                {
                    abValue = cheatCodeParam.dropdownParam.value;
                    PlayerData.isFirstSetup = false;
                    UpdateAb20201106(abValue);
                    Services.DataNormal().SetInt(abTestConfig.KeyName, abValue);
                    // EventController.RaiseEvent(EventList.GamePlay.RESET_CONFIG);
                    EventController.RaiseEvent(EventList.GamePlay.GAME_INIT);
                    // set new AB Test Value
                },
                abTestConfig.ListEventsAB.ToList(),
                abValue
            );
        }

        public void UpdateAb20201109(int abValue)
        {
            Game.isHaveSetting = abValue != 0;

            if (abValue == 1 || abValue == 2)
            {
                if (!PlayerData.isFirstSetup)
                {
                    PlayerData.isFirstSetup = true;
                    PlayerData.isSoundOn = abValue == 1;
                    PlayerData.isVibrateOn = abValue == 2;
                }
            }
            else
            {
                PlayerData.isSoundOn = false;
                PlayerData.isVibrateOn = false;
            }

            UIController.SetupAbUI();
        }

        public void UpdateAb20201106(int abValue)
        {
            Game.isFasterRotate = abValue == 1; 
        }

        public void Result20201103()
        {
            Game.isHaveMixColor = false;
            GameController.GetLevelsData();

            Game.isHaveSetting = false;
            PlayerData.isSoundOn = false;
            PlayerData.isVibrateOn = false;

            UIController.SetupAbUI();
        }

        public void UpdateAb20201103(int abValue)
        {
            Game.isHaveMixColor = abValue == 1;
            GameController.GetLevelsData();


            Game.isHaveSetting = false;
            PlayerData.isSoundOn = false;
            PlayerData.isVibrateOn = false;

            UIController.SetupAbUI();
        }

        public void ResultAbFullAds()
        {
            Game.timeDelayFullAds = 30f;
        }

        public void UpdateAbFullAds(int abValue)
        {
            Game.timeDelayFullAds = (abValue == 0) ? 30f : 60f;
        }

    }
}