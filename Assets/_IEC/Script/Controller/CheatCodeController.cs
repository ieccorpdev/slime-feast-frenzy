using UnityEngine;
using _IECModules;

namespace IEC
{
    public class CheatCodeController : MonoBehaviour, IController, IEventListener, IEventAwake, IEventStart
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
                Pool.Get<Game>().isDebugReset = true;
                PlayerPrefs.DeleteAll();
                Application.Quit();

#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
            });

            ModuleController.cheatCode.AddCheatCodeButtonWithInput("Set Level", cheatCodeParam =>
            {
                // add code here
            }, new CheatCode.InputParam
            {
                value = 50
            });
        }
    }
}