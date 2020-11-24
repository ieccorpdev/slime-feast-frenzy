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
            Result20201106();
            Result20201109();
            Result20201120();

            var abValue = Services.ABTest().GetValue(abTestConfig.KeyName, 0);
            UpdateAb20201125(abValue);

            ModuleController.cheatCode.AddCheatCodeButtonWithDropdown(abTestConfig.KeyName,
                cheatCodeParam =>
                {
                    abValue = cheatCodeParam.dropdownParam.value;
                    UpdateAb20201125(abValue);
                    Services.DataNormal().SetInt(abTestConfig.KeyName, abValue);
                    // EventController.RaiseEvent(EventList.GamePlay.RESET_CONFIG);
                    EventController.RaiseEvent(EventList.GamePlay.GAME_SPAWN_LEVEL);
                    // set new AB Test Value
                },
                abTestConfig.ListEventsAB.ToList(),
                abValue
            );
        }

        private void UpdateAb20201125(int abValue)
        {
            Game.isLevelDifficultyV2 = abValue == 1;
            Game.isRandomColorClassicMode = abValue == 2;
        }

        private void Result20201120()
        {
            Game.isHaveHint = true;
            Game.isReduceHintGrid = true;
            UIController.SetupAbUI();
        }

        private void UpdateAb20201120(int abValue)
        {
            Game.isReduceHintGrid = abValue == 1;
            Game.isHaveHint = (abValue == 1) || (abValue == 2);
            Game.isFixedHintGrid = abValue == 2;
            Game.isFixLevelDifficulty = abValue == 3;

            UIController.SetupAbUI();
        }

        private void Result20201117()
        {
            Game.isHaveHint = true;
            UIController.SetupAbUI();
        }

        private void UpdateAb20201117(int abValue)
        {
            Game.isHaveHint = abValue == 1;
            Game.isHaveTutorital = abValue == 2;
            Game.isHaveMixColor = abValue == 2;

            GameController.GetLevelsData();
            UIController.SetupAbUI();
        }

        private void UpdateAb20201112(int abValue)
        {
            Game.isHaveTutorital = abValue != 0;
            Game.isHaveMixColor = abValue == 2;

            GameController.GetLevelsData();
        }

        private void Result20201109()
        {
            Game.isHaveSetting = true;

            if (!PlayerData.isFirstSetup)
            {
                PlayerData.isFirstSetup = true;
                PlayerData.isSoundOn = true;
                PlayerData.isVibrateOn = false;
            }
            //UIController.SetupAbUI();
        }

        private void UpdateAb20201109(int abValue)
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

            //UIController.SetupAbUI();
        }

        private void Result20201106()
        {
            Game.isFasterRotate = true;
        }

        private void UpdateAb20201106(int abValue)
        {
            Game.isFasterRotate = abValue == 1; 
        }

        private void Result20201103()
        {
            Game.isHaveMixColor = false;
            GameController.GetLevelsData();
        }

        private void UpdateAb20201103(int abValue)
        {
            Game.isHaveMixColor = abValue == 1;
            GameController.GetLevelsData();
        }

        private void ResultAbFullAds()
        {
            Game.timeDelayFullAds = 30f;
        }

        private void UpdateAbFullAds(int abValue)
        {
            Game.timeDelayFullAds = (abValue == 0) ? 30f : 60f;
        }

    }
}