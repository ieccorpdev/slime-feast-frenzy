using _IECModules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace IEC
{
    public class ABTestController : _BaseBehaviour, IController, IEventListener, IEventAwake, IEventStart
    {
#region COMMON
        const string SEMI_OVERRIDE_KEY = "Override.";
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

            if (abConfig == null)
            {
                Debug.LogError("No AB Test, Use Latest AB Test");
                abIndex = listAbTestConfig.FindLastIndex(x => x.IsActive);
                abConfig = listAbTestConfig.ElementAtOrDefault(abIndex);
                Services.SetUpAbTest(abConfig, true);
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
            PresetAllAbTest();

            var serviceAbTest = Services.ABTest();
            var listAbTestConfig = Services.config.GetListAbTestConfig();
            var abIndex = listAbTestConfig.IndexOf(abConfig);

            if (abConfig == null)
            {
                Debug.LogError("ApplyAbTest found no AB test");
                return;
            }

            var listAbTestDate = listAbTestConfig.Select(x => x.Date).ToList();
            var dictAbTest = listAbTestConfig.ToDictionary(x => x.Date, x => x);
            var abValue = serviceAbTest.GetValue(abConfig.KeyName, 0);
            var dictOverride = GetDictOverrideResult().OrderByDescending(x => x.Key).Where(x => listAbTestDate.Contains(x.Key)).ToDictionary(x => x.Key, x => x.Value);

            var allMethods = GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly).ToList();
            var listResultAbAction = GetListResultAbAction();
            var listUpdateAbAction = GetListUpdateAbAction();
            var dictResultAb = GetDictAction(ABTestConfig.RESULT_PREFIX, allMethods, listAbTestConfig);
            var dictUpdateAb = GetDictAction(ABTestConfig.UPDATE_PREFIX, allMethods, listAbTestConfig);

            var curTestDate = abConfig.GetAbTestDate(abValue);
            var activeAb = listAbTestConfig.FindLast(x => x.IsActive);
            var activeDate = activeAb.GetAbTestDate(0);
            var isNewAbDate = curTestDate.config.KeyName != activeDate.config.KeyName;
            PlayerData.AddUserVersion(Application.version, isNewAbDate ? activeDate.FullVersion : curTestDate.FullVersion);

            CheckSemiOverride(curTestDate, activeDate, activeAb, dictOverride);

            var dictApplyOverride = dictOverride.ToDictionary(kv => kv.Key, kv =>
            {
                if (!dictAbTest[kv.Key].isSemiOverride) return true;
                var checkOverride = serviceAbTest.GetValue(SEMI_OVERRIDE_KEY + kv.Key, 1);
                return checkOverride == 1;
            });
            var isOverride = curTestDate.IsOverrideResult(activeDate, dictOverride, dictApplyOverride, out var overrideDate);

            var param = new object[] { };
            for (int i = 0; i < listAbTestConfig.Count; i++)
            {
                var ab = listAbTestConfig[i];
                var testDate = ab.GetAbTestDate(0);
                if (testDate.IsApplyResult(curTestDate, overrideDate))
                {
                    if (i < listResultAbAction.Count)
                    {
                        listResultAbAction[i].Invoke();
                    }
                    else
                    {
                        dictResultAb[ab]?.Invoke(this, param);
                    }
                }
            }

            if (curTestDate.IsApplyUpdate(overrideDate))
            {
                if (abIndex < listUpdateAbAction.Count)
                {
                    listUpdateAbAction[abIndex].Invoke(abValue);
                }
                else
                {
                    param = new object[] { abValue };
                    var updateAbAction = dictUpdateAb[abConfig];
                    updateAbAction?.Invoke(this, param);
                }
            }

            ResultAllAbTest();

            AddCheatCodeVersionAB(abConfig, abIndex, abValue);

            Game.abVersion = curTestDate.version;

            EventController.RaiseEvent(EventList.GamePlay.APPLY_AB_TEST);
        }

        void CheckSemiOverride(ABTestDate curTestDate, ABTestDate activeDate, ABTestConfig activeAb, Dictionary<int, Func<int, int, bool>> dictOverride)
        {
            var serviceAbTest = Services.ABTest();

            if (serviceAbTest.HasValue(SEMI_OVERRIDE_KEY + activeDate.date)) return;
            if (!dictOverride.ContainsKey(activeDate.date)) return;

            var ignoreFunc = dictOverride[activeDate.date];
            var isIgnore = ignoreFunc != null && ignoreFunc.Invoke(curTestDate.date, curTestDate.version);

            if (isIgnore) return;

            var checkOverride = activeAb.CheckSemiOverride();
            serviceAbTest.SetValue(SEMI_OVERRIDE_KEY + activeDate.date, checkOverride);

            string oldVersion = "AB_" + curTestDate.date + "_v" + curTestDate.version;
            string newVersion = "AB_" + activeDate.date + "_v" + checkOverride;
            PlayerData.AddUserVersion(Application.version, newVersion, true);
            string eventName = "Override_AB_";
            eventName += activeDate.date;

            Services.PushEvent(eventName, 0,
                new Dictionary<string, object>
                {
                        { "new_version", newVersion },
                        { "old_version", oldVersion },
                        { "levelUpdate", PlayerData.level }
                });
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

            Pool.Get<ServiceData>().curAbTestDate = abTestConfig.ListEventsAB[abValue];
            cheatCode.action = cheatCodeParam =>
            {
                abValue = cheatCodeParam.dropdownParam.value;
                Services.DataNormal().SetInt(abTestConfig.KeyName, abValue);
                Pool.Get<ServiceData>().curAbTestDate = abTestConfig.ListEventsAB[abValue];
                ApplyAbTest(abTestConfig);
                ResultAllAbTest();
            };

            cheatCode.dropdownParam = new CheatCode.DropdownParam
            {
                options = abTestConfig.ListEventsAB.ToList(),
                value = abValue
            };
        }

        Dictionary<ABTestConfig, MethodInfo> GetDictAction(string prefix, List<MethodInfo> allMethods, List<ABTestConfig> listAbTest)
        {
            var dictMethods = allMethods.Where(m => m.Name.StartsWith(prefix)).ToDictionary(m => m.Name, m => m);
            return listAbTest.ToDictionary(x => x, x => GetMethodInfo(dictMethods, x.GetAbTestAction(prefix)));
        }

        MethodInfo GetMethodInfo(Dictionary<string, MethodInfo> dictMethods, string abTestActionName)
        {
            if (dictMethods.TryGetValue(abTestActionName, out var value))
                return value;
            return null;
        }

        #endregion

        List<Action> GetListResultAbAction()
        {
            return new List<Action>()
            {
                //ResultAb20150101,
            };
        }

        List<Action<int>> GetListUpdateAbAction()
        {
            return new List<Action<int>>()
            {
                //UpdateAb20150101,
            };
        }

        void PresetAllAbTest()
        {
            Game.timeDelayFullAds = 120f;
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
        void ResultAb20250508()
        {

        }
        void UpdateAb20250508(int abValue)
        {
        }             
        void ResultAb20250506()
        {

        }
        void UpdateAb20250506(int abValue)
        {
        }     
        
        void ResultAb20250501()
        {

        }
        void UpdateAb20250501(int abValue)
        {
        }     
        void ResultAb20250430()
        {

        }
        void UpdateAb20250430(int abValue)
        {
        }
    }
}