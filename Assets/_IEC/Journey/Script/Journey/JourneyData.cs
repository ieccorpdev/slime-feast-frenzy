using System;
using System.Collections.Generic;
namespace IEC
{
    [Serializable]
    public class JourneyData
    {
        public Dictionary<int, JourneyItemStatus> regularJourneyPass = new Dictionary<int, JourneyItemStatus>();//0 to lock, 1 to unlock, 2 is can claimed,3 is unlock
        public Dictionary<int, JourneyItemStatus> vipJourneyPass = new Dictionary<int, JourneyItemStatus>();
        public int currentTargetJourney;
        public int currentTargetJourneyClaimed;
        public int currentFuelJourney;
        public bool isEnableMaxHeart;
        public bool isBuyIAP;
        public bool wasOpenJourney;
        public int lifeCycleCount;
        public bool inJourneyProgress;
        public DateTime timeStartJourneyTrip;
        public bool IsJourneyExpired;
        public bool isShowTutorialInPopup;

        public void SetStartJourneyTimeNow()
        {
            if (!inJourneyProgress)
            {
                timeStartJourneyTrip = DateTime.Now;
                inJourneyProgress = true;
            }
            if (!wasOpenJourney)
            {
                lifeCycleCount++;
                wasOpenJourney = true;
            }
        }

        private void ResetJourneyData()
        {
            regularJourneyPass.Clear();
            vipJourneyPass.Clear();
            currentTargetJourney = 0;
            currentTargetJourneyClaimed = 0;
            currentFuelJourney = 0;
            isEnableMaxHeart = false;
            isBuyIAP = false;
            wasOpenJourney = false;
            IsJourneyExpired = false;
        }

        public string GetTimeRemainingString()
        {
            TimeSpan remaining = GetTimeRemaining();
            if (remaining == TimeSpan.Zero)
            {
                ResetJourneyData();
                wasOpenJourney = false;
                return "Finished";
            }
            return remaining.Days > 0 ? remaining.Days + "d " + remaining.Hours + "h" :
                   remaining.Hours + "h " + remaining.Minutes + "mn";
        }


        public string GetTimeRemainingStringInPopup()
        {
            TimeSpan remaining = GetTimeRemaining();

            if (remaining <= TimeSpan.Zero)
            {
                wasOpenJourney = false;
                return "Finished";
            }

            if (remaining.Days > 0)
                return $"{remaining.Days:D2}d:{remaining.Hours:D2}h:{remaining.Minutes:D2}m"; // dd:hh:mm
            else
                return $"{remaining.Hours:D2}h:{remaining.Minutes:D2}m:{remaining.Seconds:D2}s"; // hh:mm:ss
        }

        private TimeSpan GetTimeRemaining()
        {
            DateTime endTime = timeStartJourneyTrip.AddDays(JourneyTripConfig.maxDay * lifeCycleCount);
            TimeSpan remaining = endTime - DateTime.Now;
            return remaining < TimeSpan.Zero ? TimeSpan.Zero : remaining;
        }

        public JourneyItemStatus GetStatusVipPass(int id)
        {
            if (!vipJourneyPass.ContainsKey(id)) vipJourneyPass[id] = JourneyItemStatus.LOCK;
            if (id < currentTargetJourney && vipJourneyPass[id] == JourneyItemStatus.LOCK || vipJourneyPass[id] == JourneyItemStatus.UNLOCK)
            {
                if (isBuyIAP)
                {
                    vipJourneyPass[id] = JourneyItemStatus.CANCLAIM;
                }
                else
                {
                    vipJourneyPass[id] = JourneyItemStatus.LOCK;
                }
            }
            return vipJourneyPass[id];
        }

        public void SetStatusVipPass(int id, JourneyItemStatus stt)
        {
            vipJourneyPass[id] = stt;
        }

        public void SetStatusRegularPass(int id, JourneyItemStatus stt)
        {
            regularJourneyPass[id] = stt;
        }

        public JourneyItemStatus GetStatusRegularPass(int id)
        {
            if (!regularJourneyPass.ContainsKey(id)) 
                regularJourneyPass[id] = JourneyItemStatus.LOCK;
            if (id >= 0 && id < currentTargetJourney && (regularJourneyPass[id] == JourneyItemStatus.LOCK || regularJourneyPass[id] == JourneyItemStatus.UNLOCK))
            {
                regularJourneyPass[id] = JourneyItemStatus.CANCLAIM;
            }
            return regularJourneyPass[id];
        }

    }
}
