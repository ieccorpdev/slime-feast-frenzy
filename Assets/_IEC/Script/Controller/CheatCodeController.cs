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

            ModuleController.cheatCode.AddCheatCodeButton("Toggle NoAds", cheatCodeParam =>
            {
                var data = Pool.Get<ServiceData>();
                Services.ActiveNoAds(!data.isNoAds);
                data.isNoAdsReward = data.isNoAds;

                cheatCodeParam.buttonText = "NoAds: " + data.isNoAds;
            });

            ModuleController.cheatCode.onCheatCodeOpen.AddListener(() =>
            {
                ModuleController.cheatCode.AddCheatCodeFromObject("Game", Game);
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
        }
    }
}