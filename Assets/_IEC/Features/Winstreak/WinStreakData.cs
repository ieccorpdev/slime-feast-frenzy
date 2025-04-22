using System;
using System.Collections.Generic;
using _IEC.Features.Common.Extension;

namespace IEC
{
    public class WinStreakData: IData
    {
        public int countLevelWin;    
        public bool isKilledAppInGameplay;
        
        public void AddLevelWin(int levelCount = 1)
        {
            countLevelWin += levelCount;
            //this.RaiseEvent(EventList.WinStreak.ON_WINSTREAK_WIN);
        }
        
        public void SetLevelWin(int count)
        {
            countLevelWin = count;
            //this.RaiseEvent(EventList.WinStreak.ON_WINSTREAK_WIN);
        }
        
        public void ResetWinStreak()
        {
            countLevelWin = 0;
            //this.RaiseEvent(EventList.WinStreak.ON_WINSTREAK_RESET);
        }
    }
}