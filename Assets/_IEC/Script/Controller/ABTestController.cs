using _IECModules;
using System.Linq;
using UnityEngine;

namespace IEC
{
    public class ABTestController : _BaseController, IController, IEventListener, IEventAwake, IEventStart
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

            var abValue = Services.ABTest().GetValue(abTestConfig.KeyName, 0);
            UpdateAbFullAds(abValue);

            ModuleController.cheatCode.AddCheatCodeButtonWithDropdown(abTestConfig.KeyName,
                cheatCodeParam =>
                {
                    abValue = cheatCodeParam.dropdownParam.value;
                    UpdateAbFullAds(abValue);
                    // set new AB Test Value
                }, abTestConfig.ListEventsAB.ToList()
                , Services.ABTest().GetValue(abTestConfig.KeyName, 0)
            );
        }

        public void UpdateAbFullAds(int abValue)
        {
            Game.timeDelayFullAds = (abValue == 0) ? 30f : 60f;
        }
    }
}