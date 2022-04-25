using _IECModules;
using System.Linq;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace IEC
{
    public class ABTestController : _BaseBehaviour, IController, IEventListener, IEventAwake, IEventStart
    {
#region COMMON
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
            var listAbTestConfig = Services.config.abTestConfigs.ToList();
            var abIndex = listAbTestConfig.FindLastIndex(x => serviceAbTest.HasValue(x.KeyName));

            var forceSetAb = "ForceSetAB";
            var forceSetAbName = Services.DataSecure().GetString(forceSetAb, "No Force Set");
            var forceSetAbIndex = listAbTestConfig.FindLastIndex(x => x.KeyName == forceSetAbName);
            abIndex = (forceSetAbIndex >= 0) ? forceSetAbIndex : abIndex;

            var abConfig = listAbTestConfig.ElementAtOrDefault(abIndex);

            if (abConfig == null)
            {
                Debug.LogError("No AB Test");
                ResultAllAbTest();
                EventController.RaiseEvent(EventList.GamePlay.APPLY_AB_TEST);
                return;
            }

            ModuleController.cheatCode.AddCheatCodeButtonWithDropdown("AB Test Config",
                cheatCodeParam =>
                {
                    var cheatAbIndex = cheatCodeParam.dropdownParam.value;
                    Services.DataSecure().SetString(forceSetAb, abTestConfigs[cheatAbIndex].KeyName);
                    ApplyAbTest(cheatAbIndex);
                },
                abTestConfigs.Select(x => x.KeyName).ToList(),
                abIndex
            );

            ApplyAbTest(abIndex);
        }

        void ApplyAbTest(int abIndex)
        {
            Pool.Set(new Game());
            var serviceAbTest = Services.ABTest();
            var listAbTestConfig = Services.config.abTestConfigs.ToList();
            var abConfig = (abIndex >= 0) ? listAbTestConfig[abIndex] : null;

            if (abConfig == null)
            {
                Debug.LogError("ApplyAbTest found no AB test");
                return;
            }

            var abValue = serviceAbTest.GetValue(abConfig.KeyName, 0);
            var abEventName = abConfig.ListEventsAB[abValue];
            var listResultAb = GetListResultAction();
            var listUpdateAb = GetListUpdateAbAction();
            var dictOverride = GetDictOverrideResult().OrderByDescending(x => x.Key).ToDictionary(x => x.Key, x => x.Value);

            var listAbTestDate = listAbTestConfig.Select((x, i) =>
                new ABTestDate()
                {
                    config = x,
                    date = int.Parse(x.KeyName.Split('_').Last())
                })
                .ToList();
            var curTestDate = listAbTestDate[abIndex];
            curTestDate.version = int.Parse(abEventName.Split('v').Last());

            if (listUpdateAb.Count != listAbTestConfig.Count)
            {
                Debug.LogError("List Update AB Action Not Equal List AB Test");
                return;
            }

            var isOverride = curTestDate.IsOverrideResult(dictOverride, out var overrideDate);

            for (int i = 0; i < listResultAb.Count; i++)
            {
                var testDate = listAbTestDate[i];
                if (testDate.IsApplyResult(curTestDate, overrideDate))
                {
                    listResultAb[i].Invoke();
                }
            }

            if (curTestDate.IsApplyUpdate(overrideDate))
            {
                var updateAbAction = listUpdateAb[abIndex];
                updateAbAction.Invoke(abValue);
            }

            ResultAllAbTest();

            AddCheatCodeVersionAB(abConfig, abIndex, abValue);

            Game.abVersion = curTestDate.version;

            EventController.RaiseEvent(EventList.GamePlay.APPLY_AB_TEST);
        }

        void AddCheatCodeVersionAB(ABTestConfig abTestConfig, int abIndex, int abValue)
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
                ApplyAbTest(abIndex);
                ResultAllAbTest();
            };

            cheatCode.dropdownParam = new CheatCode.DropdownParam
            {
                options = abTestConfig.ListEventsAB.ToList(),
                value = abValue
            };
        }
#endregion

        Dictionary<int, Func<int, int, bool>> GetDictOverrideResult()
        {
            return new Dictionary<int, Func<int, int, bool>>
            {
                //{ 20210718, (date, version) => version == 91 },
                //{ 20210718, (date, version) => date == 20210714 },
                //{ 20210718, null },
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
                //ResultAb20210423,
            };
        }

        List<Action<int>> GetListUpdateAbAction()
        {
            return new List<Action<int>>()
            {
                //UpdateAbFullAds,
                //UpdateAb20210423,
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