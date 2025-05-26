using System;

namespace IEC
{
    public partial class PlayerSecret
    {
        [Serializable]
        public class ArtifactData
        {
            public int currentArtifactTokenCount;
            public DateTime lastTimeResetEvent;

            public void SaveTimeResetEvent()
            {
                lastTimeResetEvent = DateTime.Now;
            }
            public void ResetTokenCount()
            {
                currentArtifactTokenCount = 0;
            }
            public void AddTokenCount(int amount)
            {
                currentArtifactTokenCount += amount;
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
}