using System.Collections.Generic;
namespace IEC
{
    public interface IAnalyticEvent
    {
        void Init();
        void Init(AdjustAnalyticEventConfig adjustAnalyticEventConfig);
        void PushCachedEvents();
        void PushEvent(string eventName, float value = 1f, Dictionary<string, object> param = null);
        void SetUserProperty(string name, string property);
        bool IsDisable { get; set; }
    }

    public class AnalyticEventData
    {
        public string eventName;
        public float value = 1;
        public Dictionary<string, object> param;
    }
}

