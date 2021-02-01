using System;
using System.Collections.Generic;
using UnityEngine;
namespace IEC
{
    public class DailyMissionModule : IDailyMissionService
    {
        string m_SaveKey;
        string m_LastDayKey;
        int m_HourChangeDay;

        IDataService m_Data;

        DateTime? m_LastDay = null;

        Dictionary<string, DailyMissionConfig> m_Configs;

        Dictionary<string, MissionData> m_MissionData;

        public DailyMissionModule(string saveKey, string lastDayKey,
            int hourChangeDay, IDataService data)
        {
            m_SaveKey = saveKey;
            m_LastDayKey = lastDayKey;
            m_HourChangeDay = hourChangeDay;
            m_Data = data;
            m_Configs = new Dictionary<string, DailyMissionConfig>();
            m_MissionData = new Dictionary<string, MissionData>();
        }

        public void SetConfig(DailyMissionConfig[] configs)
        {
            for (int i = 0; i < configs.Length; i++)
            {
                m_Configs[configs[i].KeyName] = configs[i];
            }

            m_MissionData = m_Data.GetObject<Dictionary<string, MissionData>>(m_SaveKey, null);

            //Debug.Log("Mission data:" + m_Data.GetString(m_SaveKey, string.Empty));
            if (m_MissionData == null)
            {
                SetEmptyMissionData();
            }
        }

        void SetEmptyMissionData()
        {
            //Debug.Log("Set empty");
            if (m_MissionData != null)
            {
                m_MissionData.Clear();
            }
            else
            {
                m_MissionData = new Dictionary<string, MissionData>();
            }
            foreach (var key in m_Configs.Keys)
            {
                m_MissionData[key] = new MissionData(key);
            }

            m_Data.SetObject(m_SaveKey, m_MissionData);
            //ServicesManager.instance.IsClaimTotalReward = false;
        }

        void CheckNewDay()
        {
            var lastDay = LoadLastDay();
            var nowDay = DateTime.Now;

            if (lastDay.DayOfYear == nowDay.DayOfYear)
            {
                // old day
            }
            else
            {
                if (nowDay.Hour < m_HourChangeDay)
                {
                    // old day
                }
                else
                {
                    // new day
                    SetEmptyMissionData();
                    SaveLastDay();
                }
            }
        }

        DateTime LoadLastDay()
        {
            if (m_LastDay == null)
            {
                DateTime dateTime = m_Data.GetObject(m_LastDayKey, DateTime.Now);
                m_LastDay = dateTime;
            }

            return m_LastDay.Value;
        }

        void SaveLastDay()
        {
            var now = DateTime.Now;
            m_LastDay = now;
            m_Data.SetObject(m_LastDayKey, now);
        }

        public int GetValue(string keyName)
        {
            CheckNewDay();
            return m_MissionData[keyName].Value;
        }

        public int GetConditionValue(string keyName)
        {
            CheckNewDay();
            return m_Configs[keyName].ConditionValue;
        }

        public bool IsClaimedReward(string keyName)
        {
            CheckNewDay();
            return m_MissionData[keyName].IsClaimed;
        }

        public void SaveData()
        {
            m_Data.SetObject(m_SaveKey, m_MissionData);
            m_Data.Save();
        }

        public void ClaimReward(string keyName)
        {
            CheckNewDay();
            m_MissionData[keyName].IsClaimed = true;
        }

        public void AddValue(string keyName, int value)
        {
            CheckNewDay();
            m_MissionData[keyName].Value += value;
        }

        public void SetValue(string keyName, int value)
        {
            CheckNewDay();
            m_MissionData[keyName].Value = value;
        }

        public void SetValueIfHigher(string keyName, int value)
        {
            CheckNewDay();
            if (value > m_MissionData[keyName].Value)
            {
                m_MissionData[keyName].Value = value;
            }
        }

        public bool IsProgressDone(string keyName)
        {
            CheckNewDay();
            return m_MissionData[keyName].Value >= m_Configs[keyName].ConditionValue;
        }

        public int GetReward(string keyName)
        {
            return m_Configs[keyName].RewardEarn;
        }

        public int GetSkipCost(string keyName)
        {
            return m_Configs[keyName].SkipCost;
        }

        public bool IsCanSkip(string keyName)
        {
            return m_Configs[keyName].IsCanSkip;
        }

        public bool IsRewardUnClaim(string keyName)
        {
            return IsProgressDone(keyName) && !IsClaimedReward(keyName);
        }

        public uint GetSecondsRemainTime()
        {
            var nowDay = DateTime.Now;
            DateTime newDay;

            if (nowDay.Hour < 4)
            {
                newDay = nowDay.Date.AddHours(4);
            }
            else
            {
                newDay = nowDay.Date.AddDays(1).AddHours(4);
            }

            //Debug.Log("now day: " + nowDay.ToString());
            //Debug.Log("new day: " + newDay.ToString());

            return (uint)(newDay - nowDay).TotalSeconds;
        }
    }
}
