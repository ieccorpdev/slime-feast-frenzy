using UnityEngine;
using _IECModules;

namespace IEC
{
    public class CheatCodeController : _BaseBehaviour, IController, IEventListener, IEventAwake, IEventStart, IEventUpdate
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
            ModuleController.cheatCode.positionActivate = CheatCode.PositionActivate.TopRight | CheatCode.PositionActivate.BotLeft;
            ModuleController.cheatCode.topOffset = 0f;
            Reporter.topOffset = 0f;
            ShowFPS.topOffset = 0f;

            ModuleController.cheatCode.AddCheatCodeButton("RESET Player Data!!!!!", cheatCodeParam =>
            {
                Game.isDebugReset = true;
                PlayerPrefs.DeleteAll();
                Application.Quit();

#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
            });

            ModuleController.cheatCode.AddCheatCodeButton("RESET Player Data but same AB test", cheatCodeParam =>
            {
                Game.isDebugReset = true;
                Services.DataSecure().SetString(DataController.PLAYER_DATA_KEY, string.Empty);
                Services.DataSecure().Save();
                Application.Quit();

#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
            });

            ModuleController.cheatCode.AddCheatCodeButtonWithInput("Set Level", param =>
            {
                int level = int.Parse(param.inputParams[0].value.ToString());
                PlayerData.level = level;
                EventController.RaiseEvent(EventList.GamePlay.NEW_GAME);
            }, new CheatCode.InputParam
            {
                value = PlayerData.level
            });

            ModuleController.cheatCode.AddCheatCodeButton("Set LEVEL_WIN", cheatCodeParam =>
            {
                EventController.RaiseEvent(EventList.GamePlay.LEVEL_WIN);
            });

            ModuleController.cheatCode.AddCheatCodeButton("Set LEVEL_LOSE", cheatCodeParam =>
            {
                EventController.RaiseEvent(EventList.GamePlay.LEVEL_LOSE);
            });

            ModuleController.cheatCode.AddCheatCodeButton("Toggle NoAds", cheatCodeParam =>
            {
                var data = Pool.Get<ServiceData>();
                Services.ActiveNoAds(!data.isNoAds);
                data.isNoAdsReward = data.isNoAds;

                cheatCodeParam.buttonText = "NoAds: " + data.isNoAds;
            });

            ModuleController.cheatCode.onCheatCodeOpen.AddListener(() =>
            {
                ModuleController.cheatCode.AddCheatCodeFromObject("Game", Game, () =>
                {
                    EventController.RaiseEvent(EventList.GamePlay.APPLY_AB_TEST);
                });
            });

            ModuleController.cheatCode.onCheatCodeStateChange.AddListener(() =>
            {
                if (ModuleController.cheatCode.showCheatCodeState == CheatCode.ShowCheatCodeState.ShowPasswordInput)
                {
                    Services.PushEvent(new TrackingEvent.OpenCheat());
                }
                else if (ModuleController.cheatCode.showCheatCodeState == CheatCode.ShowCheatCodeState.ShowCheatCode)
                {
                    Services.PushEvent(new TrackingEvent.UnlockCheat());
                }
            });

            ModuleController.cheatCode.AddCheatCodeButton("Show User Version", param =>
            {
                var userVerData = Pool.Get<UserVersionData>();
                foreach (var userVer in userVerData.listInfo)
                {
                    Debug.LogError(userVer.JsonSerialize());
                }
            });
        }

        public void BehaviourUpdate(float deltaTime)
        {
            if (!Application.isMobilePlatform)
            {
                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    if (!GamePlay.IsWin) PlayerData.level = Mathf.Max(1, PlayerData.level + 1);
                    EventController.RaiseEvent(EventList.GamePlay.NEW_GAME);
                }
                else if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    PlayerData.level = Mathf.Max(1, PlayerData.level - 1);
                    EventController.RaiseEvent(EventList.GamePlay.NEW_GAME);
                }
            }
        }
    }
}