using System.Collections;
using System.Collections.Generic;

namespace IEC
{
    public class EventCountModule
    {
        public const string COUNT_PREFIX = "EventCount.";

        IDataService m_Data;

        public EventCountModule(IDataService dataService)
        {
            m_Data = dataService;
        }

        public string GetSaveKey(string eventID)
        {
            return COUNT_PREFIX + eventID;
        }

        public int GetCount(string eventID)
        {
            return m_Data.GetInt(GetSaveKey(eventID), 0);
        }

        public int IncreaseCount(string eventID)
        {
            var key = GetSaveKey(eventID);
            var count = m_Data.GetInt(key, 0) + 1;
            m_Data.SetInt(key, count);
            return count;
        }

        public void ResetCount(string eventID)
        {
            m_Data.SetInt(GetSaveKey(eventID), 0);
        }
    }
}

