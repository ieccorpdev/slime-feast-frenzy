using _IECModules;
using System.Linq;
using UnityEngine;

namespace IEC
{
    public class ABTestController : MonoBehaviour, IController, IEventListener, IEventAwake, IEventStart
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
            var servicesController = Controllers.Get<ServicesController>();
            var abTestConfigs = servicesController.config.abTestConfigs;
            if (abTestConfigs.Length == 0) return;
            var abTestConfig = servicesController.config.abTestConfigs.Last();

            var abValue = servicesController.ABTest().GetValue(abTestConfig.KeyName, 0);
            UpdateAbFullAds(abValue);

            ModuleController.cheatCode.AddCheatCodeButtonWithDropdown(abTestConfig.KeyName,
                cheatCodeParam =>
                {
                    abValue = cheatCodeParam.dropdownParam.value;
                    UpdateAbFullAds(abValue);
                    // set new AB Test Value
                }, abTestConfig.ListEventsAB.ToList()
                , servicesController.ABTest().GetValue(abTestConfig.KeyName, 0)
            );
        }

        public void UpdateAbFullAds(int abValue)
        {
            var game = Pool.Get<Game>();
            game.timeDelayFullAds = (abValue == 0) ? 30f : 60f;
        }
    }
}