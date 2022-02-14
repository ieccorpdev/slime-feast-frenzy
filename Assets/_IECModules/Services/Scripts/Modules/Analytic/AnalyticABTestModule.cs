using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Diagnostics;

namespace IEC
{
    public class AnalyticABTestModule : IAnalyticABTestService
    {
        IDataService m_Data;
        Dictionary<string, ABTestConfig> m_Configs;
        bool m_IsNewInstall;

        public AnalyticABTestModule(IDataService dataService)
        {
            m_Data = dataService;
            m_Configs = new Dictionary<string, ABTestConfig>();
        }

        public bool IsActive(string keyNameAB)
        {
            if (m_Configs.ContainsKey(keyNameAB))
            {
                return m_Configs[keyNameAB].IsActive;
            }

            return false;
        }

        public ABTestConfig GetConfig(string keyNameAB)
        {
            if (m_Configs.ContainsKey(keyNameAB))
            {
                return m_Configs[keyNameAB];
            }

            return null;
        }

        public bool HasValue(string keyNameAB)
        {
            return m_Data.HasKey(keyNameAB);
        }

        public int GetValue(string keyNameAB, int defaultValue)
        {
            if (m_Configs.ContainsKey(keyNameAB))
            {
                var config = m_Configs[keyNameAB];
                if (config.IsForceValue)
                {
                    return config.ForceValue;
                }

                int data = m_Data.GetInt(keyNameAB, config.DefaultValue);
                if (data >= 0)
                    data = data % config.ListEventsAB.Length;
                return data;
            }

            return defaultValue;
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

        public void SetUpTest(ABTestConfig config, int versionCode, bool isNewInstall, Action<string> actionLog)
        {
            string AB = "AB ";
            bool isError = false;
            if (string.IsNullOrWhiteSpace(config.KeyName))
            {
                isError = true;
                Debug.LogError(AB + "Key name is empty");
            }

            if (m_Configs.ContainsKey(config.KeyName))
            {
                isError = true;
                Debug.LogError(AB + "Duplicate key name");
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

            m_Configs[config.KeyName] = config;

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

            bool isNeedSetUp = false;
            string versionKey = "Ver" + config.KeyName;

            if (config.IsOverride &&
                m_Data.HasKey(versionKey) &&
                versionCode > m_Data.GetInt(versionKey, versionCode))
            {
                isNeedSetUp = true;
            }

            //Debug.Log(config.KeyName + ":" + m_Data.HasKey(config.KeyName));
            if (m_Data.HasKey(config.KeyName))
            {
                //Debug.Log("Have key name");
                isNeedSetUp = false;
            }
            else
            {
                //Debug.Log("No have key name");
                isNeedSetUp = true;
            }

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
                m_Data.SetInt(versionKey, versionCode);

                actionLog(config.ListEventsAB[randValue]);

                Debug.Log("AB Test: " + config.KeyName);
                Debug.Log("AB Value: " + randValue);
            }
        }
    }
}