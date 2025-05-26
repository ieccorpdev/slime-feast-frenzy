using System;

namespace IEC
{
    [Serializable]
    public class RelicData
    {
        public int relicItemIndex = 0;
        public int currentRelicTokenCount;       
        public DateTime lastTimeResetEvent;
        
        public void SaveTimeResetEvent()
        {
            lastTimeResetEvent = DateTime.Now;
        }
        public void ResetTokenCount()
        {
            currentRelicTokenCount = 0;
        }
        public void AddTokenCount(int amount)
        {
            currentRelicTokenCount += amount;
        }

        public void CheckResetTimeEvent(int daysToReset)
        {
            if ((DateTime.Now - lastTimeResetEvent).TotalDays >= daysToReset)
            {
                SaveTimeResetEvent();
                ResetTokenCount();
            }
        }
    }
}