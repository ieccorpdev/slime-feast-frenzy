using UnityEngine;
using _IECModules;
using System.Collections.Generic;

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
                int level = int.Parse(param.inputParams[0].value.ToString());
                PlayerData.level = level;
                OnRestartLevel();
            }, new CheatCode.InputParam
            {
                value = PlayerData.level
            });

            ModuleController.cheatCode.AddCheatCodeButton("Set GameOver", cheatCodeParam =>
            {
                GameController.GameOver(true);
                EventController.RaiseEventDelay(.1f, EventList.GamePlay.GAME_OVER_UI, new EventParam(this, true));
            });

            ModuleController.cheatCode.AddCheatCodeButton("Is Cheat Level: " + Game.isCheatLevel, cheatCodeParam =>
            {
                Game.isCheatLevel = !Game.isCheatLevel;
                cheatCodeParam.buttonText = "Is Cheat Level: " + Game.isCheatLevel;
            });

            ModuleController.cheatCode.AddCheatCodeButtonWithInput("Set Background(0-3)", param =>
            {
                int valueInput = int.Parse(param.inputParams[0].value.ToString());
                var bgIndex = valueInput % ShopConfig.listBGConfig.Count;
                PlayerData.UnlockBackground(bgIndex);
                GamePlay.SetBackground();
            }, new CheatCode.InputParam
            {
                value = PlayerData.backgroundSelected
            });

            ModuleController.cheatCode.AddCheatCodeButtonWithInput("Set Theme(0-3)", param =>
            {
                int valueInput = int.Parse(param.inputParams[0].value.ToString());
                var themeIndex = valueInput % ShopConfig.listThemeConfigs.Count;
                PlayerData.UnlockTheme(themeIndex);
                LevelController.SetupTheme(true);
            }, new CheatCode.InputParam
            {
                value = PlayerData.themeSelected
            });

            ModuleController.cheatCode.AddCheatCodeButton("Is Cheat Water Color: " + Game.isCheatWaterColor, cheatCodeParam =>
            {
                Game.isCheatWaterColor = !Game.isCheatWaterColor;
                cheatCodeParam.buttonText = "Is Cheat Water Color: " + Game.isCheatWaterColor;
            });

            List<string> listWaterColor = new List<string>();

            for (int i = 0; i < 12; i++)
            {
                listWaterColor.Add(i.ToString());
            }

            ModuleController.cheatCode.AddCheatCodeButtonWithDropdown("Set water color",
            cheatCodeParam =>
            {
                Game.classicColor = cheatCodeParam.dropdownParam.value;
                OnRestartLevel();
            },
            listWaterColor,
            Game.classicColor
            );
        }

        private void OnRestartLevel()
        {
            EventController.RaiseEvent(EventList.GamePlay.GAME_SPAWN_LEVEL);
        }

    }
}