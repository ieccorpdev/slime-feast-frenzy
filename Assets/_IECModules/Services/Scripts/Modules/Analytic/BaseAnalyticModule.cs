using System;
using System.Collections.Generic;

namespace IEC
{
    public class BaseAnalyticModule
    {
        public const string ANALYTIC_DATA_KEY = "AnalyticEventData";

        protected AdjustAnalyticEventConfig eventConfig;
        protected AnalyticSaveData AnalyticData => Pool.Get<AnalyticSaveData>();

        public void InitEventConfig(AdjustAnalyticEventConfig adjustAnalyticEventConfig)
        {
            eventConfig = adjustAnalyticEventConfig;
        }

        public bool CheckPushedUniqueEvent(string eventName, AnalyticDataParamType type)
        {
            var eventParam = eventConfig.GetEventParam(eventName);
            if (eventParam == null) return false;
            if (eventParam.PlatformParam.isUnique)
            {
                if (IsPushedUniqueEvent(eventName, type)) return true;
                else
                {
                    AddAnalyticSaveData(eventName, type);
                }
            }
            return false;
        }

        public bool IsPushedUniqueEvent(string eventName, AnalyticDataParamType type)
        {
            return AnalyticData.IsPushedEvent(eventName, type);
        }

        public void AddAnalyticSaveData(string eventName, AnalyticDataParamType type)
        {
            AnalyticData.AddData(eventName, type);
            EventController.RaiseEvent(EventList.Service.ON_ADD_ANALYTIC_SAVE_DATA);
        }
    }

    [Serializable]
    public class AnalyticSaveData
    {
        public List<AnalyticDataParam> listParam = new List<AnalyticDataParam>();

        public void AddData(string eventName, AnalyticDataParamType type)
        {
            var data = listParam.Find(x => x.eventName == eventName);
            if (data == null)
            {
                listParam.Add(new AnalyticDataParam()
                {
                    eventName = eventName,
                    listType = new List<AnalyticDataParamType> { type }
                });
            }
            else
            {
                if (!data.listType.Contains(type))
                {
                    data.listType.Add(type);
                }
            }
        }

        public bool IsPushedEvent(string eventName, AnalyticDataParamType type)
        {
            var data = listParam.Find(x => x.eventName == eventName);
            if (data == null) return false;
            return data.listType.Contains(type);
        }
    }

    [Serializable]
    public class AnalyticDataParam
    {
        public string eventName;
        public List<AnalyticDataParamType> listType = new List<AnalyticDataParamType>();
    }

    public enum AnalyticDataParamType
    {
        Adjust = 1,
        Firebase = 2,
    }
}