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
        public int currentFuelJourney;
        public bool purchasedJourneyPass;
        public bool wasOpenJourney;
        public DateTime timeStartJourneyTrip;

        public JourneyItemStatus GetStatusVipPass(int id)
        {
            if (!vipJourneyPass.ContainsKey(id)) vipJourneyPass[id] = JourneyItemStatus.LOCK;
            if (id < currentTargetJourney && vipJourneyPass[id] == JourneyItemStatus.LOCK || vipJourneyPass[id] == JourneyItemStatus.UNLOCK)
            {
                if (purchasedJourneyPass)
                {
                    vipJourneyPass[id] = JourneyItemStatus.CANCLAIM;
                }
                else
                {
                    vipJourneyPass[id] = JourneyItemStatus.UNLOCK;
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
            if (!regularJourneyPass.ContainsKey(id)) regularJourneyPass[id] = JourneyItemStatus.UNLOCK;
            if (id < currentTargetJourney && (regularJourneyPass[id] == JourneyItemStatus.LOCK || regularJourneyPass[id] == JourneyItemStatus.UNLOCK))
            {
                regularJourneyPass[id] = JourneyItemStatus.CANCLAIM;
            }
            return regularJourneyPass[id];
        }

    }
}
