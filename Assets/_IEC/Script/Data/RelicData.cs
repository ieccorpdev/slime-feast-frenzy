using System;
using System.Collections.Generic;

namespace IEC
{
    [Serializable]
    public class RelicData
    {
        public int relicItemIndex = 0;
        public int currentRelicTokenCount; 
        public int curRelicLevel = 1;

        public Dictionary<int, bool> levelClaimDict = new()
        {
            { 0, false },
            { 1, false },
            { 2, false },
        };
        public DateTime lastTimeResetEvent;
        public DateTime eventTimeEnd;
        
        public void SaveTimeResetEvent()
        {
            lastTimeResetEvent = DateTime.Now;
        }

        public void LevelUp(int tokenToLevelUp)
        {
            curRelicLevel++;
            levelClaimDict = new(){
                { 0, false },
                { 1, false },
                { 2, false },
            };
            currentRelicTokenCount -= tokenToLevelUp;

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