using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    [System.Serializable]
    public class ServiceData
    {
        public bool isNoAds;
        public bool isNoAdsReward;
        public bool isInterFallbackForReward;
        public TimeCheckType timeCheckType;
#if UNITY_IOS
        public Balaso.AppTrackingTransparency.AuthorizationStatus? attStatus;
#endif

        [NonSerialized] public Dictionary<string, Func<Dictionary<string, object>>> dictCustomParams;

        private Action<string, float, Dictionary<string, object>> m_PushEventAction;

        public void SetDictEventParam(string eventName, Func<Dictionary<string, object>> funcParams)
        {
            if (funcParams == null) return;

            if (dictCustomParams == null)
            {
                dictCustomParams = new Dictionary<string, Func<Dictionary<string, object>>>();
            }

            dictCustomParams[eventName] = funcParams;
        }

        public void Init(Action<string, float, Dictionary<string, object>> pushEventAction)
        {
            InitRevMilestone();
            m_PushEventAction = pushEventAction;
        }

        public float revTotalAd;

        public float revInterAd;

        public float revRewardAd;

        public float revBannerAd;

        public List<RevMilestone> ListRevMilestones;

        public void OnAdRevenue(AdsEnum adType, float revenue)
        {
            revTotalAd += revenue;
            switch (adType)
            {
               case AdsEnum.REWARD:
                   revRewardAd += revenue;
                   break;
               case AdsEnum.INTERSTITIAL:
                   revInterAd += revenue;
                   break;
               case AdsEnum.BANNER_TOP:
               case AdsEnum.BANNER_BOTTOM:
                   revBannerAd += revenue;
                   break;
            }
            Debug.Log("Total Rev:" + revTotalAd);
            
            foreach (var milestone in ListRevMilestones)
            {
                if (milestone.IsEnoughRev(revTotalAd))
                {
                    var valueToSum = milestone.RaiseRev(revTotalAd);
                    m_PushEventAction?.Invoke(milestone.eventName,valueToSum ,null);
                }
            }
        }

        void InitRevMilestone()
        {
            if(ListRevMilestones != null && ListRevMilestones.Count > 0) return;
            ListRevMilestones = new List<RevMilestone>
            {
                new RevMilestone("rev_all_ad_1_cent", 0.01f),
                new RevMilestone("rev_all_ad_5_cent", 0.05f),
                new RevMilestone("rev_all_ad_10_cent", 0.1f),
                new RevMilestone("rev_all_ad_50_cent", 0.5f),
                new RevMilestone("rev_all_ad_100_cent", 1f)
            };
        }
    }
    
    [Serializable]
    public class RevMilestone
    {
        public float step;

        public float value;

        public float lastRev;
            
        public string eventName;
        public RevMilestone(string eventName,float step)
        {
            this.eventName = eventName;
            this.step = step;
        }

        public bool IsEnoughRev(float revCheck)
        {
            if (revCheck < value) return false;
            var multi = (int) (revCheck / step);
            value = (multi + 1) * step;
            Debug.Log($"{eventName} new milestone: {value}");
            return true;
        }

        public float RaiseRev(float rev)
        {
            var valueToSum = rev - lastRev;
            Debug.Log($"{eventName} lastRev: {lastRev}");
            Debug.Log($"{eventName} valueToSum: {valueToSum}");
            lastRev = rev;
            return valueToSum;
        }
    }
}
