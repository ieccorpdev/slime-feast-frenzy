using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Diagnostics;

namespace IEC
{
    public class AnalyticABTestModule : IAnalyticABTestService
    {
        IDataService m_Data;
        Dictionary<string, ABTestConfig> m_Configs;

        public AnalyticABTestModule(IDataService dataService, ABTestConfig[] abTestConfigs)
        {
            m_Data = dataService;
            m_Configs = abTestConfigs.ToDictionary(x => x.KeyName, x => x);
        }

        public bool IsActive(string keyNameAB)
        {
            if (m_Configs.TryGetValue(keyNameAB, out var config))
            {
                return config.IsActive;
            }

            return false;
        }

        public ABTestConfig GetConfig(string keyNameAB)
        {
            if (m_Configs.TryGetValue(keyNameAB, out var config))
            {
                return config;
            }

            return null;
        }

        public void SetValue(string keyNameAB, int value)
        {
            m_Data.SetInt(keyNameAB, value);
        }

        public bool HasValue(string keyNameAB)
        {
            return m_Data.HasKey(keyNameAB);
        }

        public int GetValue(string keyNameAB, int defaultValue)
        {
            if (m_Configs.TryGetValue(keyNameAB, out var config))
            {
                if (config.IsForceValue)
                {
                    return config.ForceValue;
                }

                int data = m_Data.GetInt(keyNameAB, config.DefaultValue);
                if (data >= 0)
                    data = data % config.ListEventsAB.Length;
                return data;
            }

            return m_Data.GetInt(keyNameAB, defaultValue);
        }

        int GetRandomIndexByRatioList(int[] ratio, int itemLength)
        {
            if (ratio == null || ratio.Length == 0)
            {
                return UnityEngine.Random.Range(0, itemLength);
            }

            float sum = 0;
            for (int i = 0; i < ratio.Length; i++)
            {
                sum += ratio[i];
            }

            float randValue = UnityEngine.Random.Range(0, sum);
            sum = 0;
            for (int i = 0; i < ratio.Length; i++)
            {
                if (ratio[i] <= 0)
                    continue;
                sum += ratio[i];
                if (randValue <= sum)
                {
                    return i;
                }
            }

            return UnityEngine.Random.Range(0, itemLength);
        }

        public void SetUpTest(ABTestConfig config, bool isNewInstall, Action<string> actionLog)
        {
            string AB = "AB ";
            bool isError = false;
            if (string.IsNullOrWhiteSpace(config.KeyName))
            {
                isError = true;
                Debug.LogError(AB + "Key name is empty");
            }

            var listEvent = new HashSet<string>();
            for (int i = 0; i < config.ListEventsAB.Length; i++)
            {
                var eventName = config.ListEventsAB[i];
                if (string.IsNullOrWhiteSpace(eventName))
                {
                    isError = true;
                    Debug.LogError(AB + "event name is empty:" + config.KeyName);
                }

                if (listEvent.Contains(eventName))
                {
                    isError = true;
                    Debug.LogError(AB + "Duplicate event name:" + config.KeyName);
                }

                listEvent.Add(eventName);
            }

            if (config.ListEventsAB.Length != config.ListRandomRatio.Length)
            {
                isError = true;
                Debug.LogError(AB + "Event array and ratio array are not same size:" + config.KeyName);
            }

            var sumRatio = 0;
            for (int i = 0; i < config.ListRandomRatio.Length; i++)
            {
                if (config.ListRandomRatio[i] == 0)
                {
                    Debug.LogWarning(AB + "ratio = 0: " + config.KeyName + "," + config.ListEventsAB[i] + " (warning)");
                }

                sumRatio += config.ListRandomRatio[i];
            }

            if (sumRatio == 0)
            {
                isError = true;
                Debug.LogError(AB + "Sum ratio = 0:" + config.KeyName);
            }

            if (isError)
            {
                if (!Application.isEditor)
                {
                    Utils.ForceCrash(ForcedCrashCategory.Abort);
                }

                return;
            }

            if (config.IsActive == false ||
                config.ListEventsAB == null ||
                config.ListEventsAB.Length == 0)
            {
                return;
            }

            if (config.IsOnlyNewInstall && isNewInstall == false)
            {
                return;
            }

            string versionKey = "Ver" + config.KeyName;
            var isNeedSetUp = !m_Data.HasKey(config.KeyName);
            
            if (isNeedSetUp)
            {
                int randValue = GetRandomIndexByRatioList(config.ListRandomRatio, config.ListEventsAB.Length);

                if (config.IsForceValue)
                {
                    randValue = config.ForceValue;
                }
                else
                {
                    randValue = randValue % config.ListEventsAB.Length;
                }

                m_Data.SetInt(config.KeyName, randValue);
                m_Data.SetInt(versionKey, 1);

                actionLog(config.ListEventsAB[randValue]);

                Debug.Log("AB Test: " + config.KeyName);
                Debug.Log("AB Value: " + randValue);
            }
        }

        public int GetOldUserAbTestValue(ABTestConfig config)
        {
            int randValue = GetRandomIndexByRatioList(config.ListRandomRatio, config.ListEventsAB.Length);
            return randValue;
        }
    }
}