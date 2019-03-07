using System.Collections.Generic;
namespace Root
{
    public interface IAnalyticEvent
    {
        void Init();
        void PushEvent(string eventName, float value = 1f, Dictionary<string, object> param = null);
    }
}

