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
            var abIndex = listAbTestConfigs.FindLastIndex(x => serviceAbTest.HasValue(x.KeyName));

            var forceSetAb = "ForceSetAB";
            var forceSetAbName = Services.DataSecure().GetString(forceSetAb, "No Force Set");
            var forceSetAbIndex = listAbTestConfigs.FindLastIndex(x => x.KeyName == forceSetAbName);
            abIndex = (forceSetAbIndex >= 0) ? forceSetAbIndex : abIndex;

            var abConfig = listAbTestConfigs.ElementAtOrDefault(abIndex);

            if (abConfig == null)
            {
                Debug.LogError("No AB Test");
                return;
            }

            ModuleController.cheatCode.AddCheatCodeButtonWithDropdown("AB Test Config",
                cheatCodeParam =>
                {
                    var cheatAbIndex = cheatCodeParam.dropdownParam.value;
                    Services.DataSecure().SetString(forceSetAb, abTestConfigs[cheatAbIndex].KeyName);
                    Pool.Set(new Game());
                    ApplyAbTest(cheatAbIndex);
                },
                abTestConfigs.Select(x => x.KeyName).ToList(),
                abIndex
            );

            ApplyAbTest(abIndex);
        }

        void ApplyAbTest(int abIndex)
        {
            var serviceAbTest = Services.ABTest();
            var listAbTestConfigs = Services.config.abTestConfigs.ToList();
            var abConfig = (abIndex >= 0) ? listAbTestConfigs[abIndex] : null;

            if (abConfig == null)
            {
                Debug.LogError("ApplyAbTest found no AB test");
                return;
            }

            var abValue = serviceAbTest.GetValue(abConfig.KeyName, 0);
            var listResultAb = GetListResultAction();
            var listUpdateAb = GetListUpdateAbAction();

            if (listUpdateAb.Count != listAbTestConfigs.Count)
            {
                Debug.LogError("List Update AB Action Not Equal List AB Test");
                return;
            }

            for (int i = 0; i < listResultAb.Count; i++)
            {
                if (i < abIndex)
                {
                    listResultAb[i].Invoke();
                }
            }

            var updateAbAction = listUpdateAb[abIndex];
            updateAbAction.Invoke(abValue);

            ResultAllAbTest();

            AddCheatCodeVersionAB(abConfig, updateAbAction, abValue);

            var abEventName = abConfig.ListEventsAB[abValue];
            Game.abVersion = int.Parse(abEventName.Split('v').Last());
        }

        void AddCheatCodeVersionAB(ABTestConfig abTestConfig, Action<int> updateAbAction, int abValue)
        {
            var cheatCodeName = "AB Test Version";
            var cheatCode = ModuleController.cheatCode.GetCheatCodeParam(cheatCodeName);
            if (cheatCode == null)
            {
                ModuleController.cheatCode.AddCheatCodeButtonWithDropdown(cheatCodeName, null, null);
                cheatCode = ModuleController.cheatCode.GetCheatCodeParam(cheatCodeName);
            }

            cheatCode.action = cheatCodeParam =>
            {
                abValue = cheatCodeParam.dropdownParam.value;
                Services.DataNormal().SetInt(abTestConfig.KeyName, abValue);
                updateAbAction(abValue);
                EventController.RaiseEvent(EventList.GamePlay.RESET_CONFIG);
            };

            cheatCode.dropdownParam = new CheatCode.DropdownParam
            {
                options = abTestConfig.ListEventsAB.ToList(),
                value = abValue
            };
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