using _IECModules;
using System.Linq;
using UnityEngine;
using System;
using System.Collections.Generic;

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
            var serviceAbTest = Services.ABTest();
            var abTestConfigs = Services.config.abTestConfigs;
            var listAbTestConfigs = Services.config.abTestConfigs.ToList();
            var oldAbIndex = listAbTestConfigs.FindLastIndex(x => serviceAbTest.HasValue(x.KeyName));
            var oldAbConfig = (oldAbIndex >= 0) ? listAbTestConfigs[oldAbIndex] : null;
            var abTestConfig = Services.config.abTestConfigs.Where(x => x.IsActive).LastOrDefault();
            var isLatestAbUser = (oldAbIndex == listAbTestConfigs.Count - 1);

            var listResultAb = GetListResultAction();
            var listUpdateAb = GetListUpdateAbAction();
            var latestUpdateAb = listUpdateAb.LastOrDefault();

            if (listUpdateAb.Count != listAbTestConfigs.Count)
            {
                Debug.LogError("List Update AB Action Not Equal List AB Test");
                return;
            }

            for (int i = 0; i < listResultAb.Count; i++)
            {
                if (i < oldAbIndex)
                {
                    listResultAb[i].Invoke();
                }
            }
            if (oldAbIndex >= 0 && oldAbIndex < listUpdateAb.Count && !isLatestAbUser && oldAbConfig != null)
            {
                listUpdateAb[oldAbIndex].Invoke(serviceAbTest.GetValue(oldAbConfig.KeyName, 0));
            }

            ResultAllAbTest();

            if (!isLatestAbUser || abTestConfig == null || latestUpdateAb == null)
            {
                Debug.LogError("No AB Test");
                return;
            }

            var abValue = Services.ABTest().GetValue(abTestConfig.KeyName, 0);
            var abEventName = abTestConfig.ListEventsAB[abValue];
            Game.abVersion = int.Parse(abEventName.Split('v').Last());
            latestUpdateAb(abValue);

            ModuleController.cheatCode.AddCheatCodeButtonWithDropdown(abTestConfig.KeyName,
                cheatCodeParam =>
                {
                    abValue = cheatCodeParam.dropdownParam.value;
                    Services.DataNormal().SetInt(abTestConfig.KeyName, abValue);
                    latestUpdateAb(abValue);
                    EventController.RaiseEvent(EventList.GamePlay.RESET_CONFIG);
                    // set new AB Test Value
                },
                abTestConfig.ListEventsAB.ToList(),
                abValue
            );
        }

        void ResultAllAbTest()
        {
            ResultAbFullAds();
        }

        List<Action> GetListResultAction()
        {
            return new List<Action>()
            {
                //ResultAbFullAds,
            };
        }

        List<Action<int>> GetListUpdateAbAction()
        {
            return new List<Action<int>>()
            {
                //UpdateAbFullAds,
                //UpdateAbFull20210423,
            };
        }

        void ResultAbFullAds()
        {
            Game.timeDelayFullAds = 30f;
        }

        void UpdateAbFullAds(int abValue)
        {
            Game.timeDelayFullAds = (abValue == 0) ? 30f : 60f;
        }
    }
}