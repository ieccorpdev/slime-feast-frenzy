using _IECModules;
using System.Linq;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;

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
            var listAbTestConfig = Services.config.GetListAbTestConfig();
            var abIndex = listAbTestConfig.FindLastIndex(x => serviceAbTest.HasValue(x.KeyName));

            var forceSetAb = "ForceSetAB";
            var forceSetAbName = Services.DataSecure().GetString(forceSetAb, "No Force Set");
            var forceSetAbIndex = listAbTestConfig.FindLastIndex(x => x.KeyName == forceSetAbName);
            abIndex = (forceSetAbIndex >= 0) ? forceSetAbIndex : abIndex;

            var abConfig = listAbTestConfig.ElementAtOrDefault(abIndex);

            PresetAllAbTest();

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
                    Services.DataSecure().SetString(forceSetAb, listAbTestConfig[cheatAbIndex].KeyName);
                    ApplyAbTest(listAbTestConfig[cheatAbIndex]);
                },
                listAbTestConfig.Select(x => x.KeyName).ToList(),
                abIndex
            );

            ApplyAbTest(abConfig);
        }

        void ApplyAbTest(ABTestConfig abConfig)
        {
            Pool.Set(new Game());
            var serviceAbTest = Services.ABTest();
            var listAbTestConfig = Services.config.GetListAbTestConfig();
            var abIndex = listAbTestConfig.IndexOf(abConfig);

            if (abConfig == null)
            {
                Debug.LogError("ApplyAbTest found no AB test");
                return;
            }

            var abValue = serviceAbTest.GetValue(abConfig.KeyName, 0);
            var dictOverride = GetDictOverrideResult().OrderByDescending(x => x.Key).ToDictionary(x => x.Key, x => x.Value);

            var allMethods = GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly).ToList();
            var listResultAb = GetListAction(ABTestConfig.RESULT_PREFIX, allMethods, listAbTestConfig);
            var listUpdateAb = GetListAction(ABTestConfig.UPDATE_PREFIX, allMethods, listAbTestConfig);

            var curTestDate = abConfig.GetAbTestDate(abValue);
            var activeDate = listAbTestConfig.FindLast(x => x.IsActive).GetAbTestDate(0);

            if (listUpdateAb.Count != listAbTestConfig.Count)
            {
                Debug.LogError("List Update AB Action Not Equal List AB Test");
                return;
            }

            var isOverride = curTestDate.IsOverrideResult(activeDate, dictOverride, out var overrideDate);

            var param = new object[] { };
            for (int i = 0; i < listResultAb.Count; i++)
            {
                var testDate = listAbTestConfig[i].GetAbTestDate(0);
                if (testDate.IsApplyResult(curTestDate, overrideDate))
                {
                    listResultAb[i].Invoke(this, param);
                }
            }

            param = new object[] { abValue };
            if (curTestDate.IsApplyUpdate(overrideDate))
            {
                var updateAbAction = listUpdateAb[abIndex];
                updateAbAction.Invoke(this, param);
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
                ApplyAbTest(abTestConfig);
                ResultAllAbTest();
            };

            cheatCode.dropdownParam = new CheatCode.DropdownParam
            {
                options = abTestConfig.ListEventsAB.ToList(),
                value = abValue
            };
        }

        List<MethodInfo> GetListAction(string prefix, List<MethodInfo> allMethods, List<ABTestConfig> listAbTest)
        {
            var dictMethods = allMethods.Where(m => m.Name.StartsWith(prefix)).ToDictionary(m => m.Name, m => m);
            return listAbTest.Select(x => dictMethods[x.GetAbTestAction(prefix)]).ToList();
        }

#endregion

        void PresetAllAbTest()
        {
            Game.timeDelayFullAds = 30f;
        }

        void ResultAllAbTest()
        {
        }

        Dictionary<int, Func<int, int, bool>> GetDictOverrideResult()
        {
            return new Dictionary<int, Func<int, int, bool>>
            {
                //{ 20210718, (date, version) => version == 91 },
                //{ 20210718, (date, version) => date == 20210714 },
                //{ 20210718, null },
            };
        }

        /*  Example AB Test Result & Update Function
        void ResultAb20150101() {}
        void UpdateAb20150101(int abValue) {}
        */
    }
}