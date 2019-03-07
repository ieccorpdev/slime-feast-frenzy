using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Root
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

        public int GetValue(string keyNameAB)
        {
            if (m_Configs.ContainsKey(keyNameAB))
            {
                var config = m_Configs[keyNameAB];
                if (config.IsForceValue)
                {
                    return config.ForceValue;
                }
                int data = m_Data.GetInt(keyNameAB, config.DefaultValue);
                data = data % config.ListEventsAB.Length;
                return data;
            }
            return -1;
        }

        public void SetUpTest(ABTestConfig[] configs, int versionCode, Func<bool> funcIsNewInstall)
        {
            if (configs == null)
                return;

            m_IsNewInstall = funcIsNewInstall();

            for (int i = 0; i < configs.Length; i++)
            {
                var config = configs[i];

                m_Configs[config.KeyName] = config;

                if (config.IsActive == false ||
                    config.ListEventsAB == null ||
                    config.ListEventsAB.Length == 0)
                {
                    continue;
                }

                if (config.IsOnlyNewInstall && m_IsNewInstall == false)
                {
                    continue;
                }

                bool isNeedSetUp = false;
                string versionKey = "Ver" + config.KeyName;

                if (config.IsOverride &&
                    m_Data.HasKey(versionKey) &&
                    versionCode > m_Data.GetInt(versionKey, versionCode))
                {
                    isNeedSetUp = true;
                }

                if (!m_Data.HasKey(config.KeyName))
                {
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

                    Debug.Log("AB Test: " + config.KeyName);
                    Debug.Log("AB Value: " + randValue);
                }
            }
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

        public void SetLogAnalytic(Action<string> actionLog)
        {
            foreach (var config in m_Configs.Values)
            {
                if (config.IsActive)
                {
                    if (config.IsOnlyNewInstall && m_IsNewInstall == false)
                    {
                        return;
                    }
                    string isLogKey = "Log" + config.KeyName;

                    if (!m_Data.GetBool(isLogKey, false))
                    {
                        m_Data.SetBool(isLogKey, true);

                        int randValue = m_Data.GetInt(config.KeyName, config.DefaultValue);
                        randValue = randValue % config.ListEventsAB.Length;

                        actionLog(config.ListEventsAB[randValue]);
                    }
                }
            }
        }

    }
}

