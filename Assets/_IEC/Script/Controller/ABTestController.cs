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
            Result20201125();
            Result20201201();
            Result20201204();
            Result20201209();

            var abValue = Services.ABTest().GetValue(abTestConfig.KeyName, 0);
            UpdateAb20201215(abValue);

            ModuleController.cheatCode.AddCheatCodeButtonWithDropdown(abTestConfig.KeyName,
                cheatCodeParam =>
                {
                    abValue = cheatCodeParam.dropdownParam.value;
                    UpdateAb20201215(abValue);
                    Services.DataNormal().SetInt(abTestConfig.KeyName, abValue);
                    // EventController.RaiseEvent(EventList.GamePlay.RESET_CONFIG);
                    EventController.RaiseEvent(EventList.GamePlay.GAME_SPAWN_LEVEL);
                    // set new AB Test Value
                },
                abTestConfig.ListEventsAB.ToList(),
                abValue
            );
        }

        private void UpdateAb20201215(int abValue)
        {

        }

        private void Result20201209()
        {
            Game.isLevelDifficultyV4 = false;
            GameController.GetLevelDifficulty();
        }

        private void UpdateAb20201209(int abValue)
        {
            Game.isLevelDifficultyV4 = abValue == 2;
            Game.abTheme = abValue == 3 ? 5 : 3;
            //GameController.GetLevelDifficulty();
        }

        private void Result20201204()
        {
            Game.isHaveNotification = true;
            Game.abTheme = 3;
            Game.isHaveBossLevelsMixColor = true;

            GameController.GetLevelsMixColor();
        }

        private void UpdateAb20201204(int abValue)
        {
            Game.isHaveNotification = abValue == 1;
            if(abValue == 2)
            {
                Game.abTheme = 1;
            }
            else if(abValue == 3)
            {
                Game.abTheme = 3;
            }
            else
            {
                Game.abTheme = 0;
            }

            Game.isHaveBossLevelsMixColor = abValue == 4;

            if (Game.isHaveBossLevelsMixColor)
            {
                GameController.GetLevelsMixColor();
            }
        }

        private void Result20201201()
        {
            Game.isHaveNewWaterEffect = true;
        }

        private void UpdateAb20201201(int abValue)
        {
            Game.isHaveUIAnimation = abValue == 1;
            Game.isLevelDifficultyV3 = abValue == 2;
            Game.isHaveNewWaterEffect = abValue == 3;
            Game.isHaveBossLevel = abValue == 4;

            //GameController.GetLevelDifficulty();
        }

        private void Result20201125()
        {
            Game.isLevelDifficultyV2 = true;
            Game.isRandomColorClassicMode = true;
            Game.isStartLevelEffect = false;
        }

        private void UpdateAb20201125(int abValue)
        {
            Game.isLevelDifficultyV2 = abValue == 1;
            Game.isRandomColorClassicMode = abValue == 2;
            Game.isStartLevelEffect = abValue == 3;
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

            UIController.SetupAbUI();
        }

        private void UpdateAb20201112(int abValue)
        {
            Game.isHaveTutorital = abValue != 0;
            Game.isHaveMixColor = abValue == 2;
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