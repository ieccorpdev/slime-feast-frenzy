using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace IEC
{
    public enum RevAdType
    {
        BANNER,
        INTER,
        REWARD,
        INTER_N_REWARD,
    }
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
        public List<string> listPercentileEvent = new List<string>();
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

        public void Init(AdjustAnalyticEventConfig config, Action<string, float, Dictionary<string, object>> pushEventAction)
        {
            InitRevMilestone(config);
            m_PushEventAction = pushEventAction;
            if (dictMilestoneBanner == null) dictMilestoneBanner = new Dictionary<int, float>();
            if (dictMilestoneInter == null) dictMilestoneInter = new Dictionary<int, float>();
            if (dictMilestoneReward == null) dictMilestoneReward = new Dictionary<int, float>();
            if (dictMilestoneRewardNInter == null) dictMilestoneRewardNInter = new Dictionary<int, float>();
        }

        public float revTotalAd;

        public float revInterAd;
        public Dictionary<int, float> dictMilestoneInter;

        public float revRewardAd;
        public Dictionary<int, float> dictMilestoneReward;

        public float revBannerAd;
        public Dictionary<int, float> dictMilestoneBanner;

        public float revInterNReward;
        public Dictionary<int, float> dictMilestoneRewardNInter;

        public float lastRevFBPurchase;

        public float GetValueToSumRevAd(RevAdType adType, int milestone)
        {
            var dictMilestone = GetDictMilestoneAd(adType);
            if (!dictMilestone.ContainsKey(milestone)) dictMilestone[milestone] = 0;
            return (GetRevAd(adType) - dictMilestone[milestone]);
        }

        public void SetValueToSumRevAd(RevAdType adType, int milestone)
        {
            var dictMilestone = GetDictMilestoneAd(adType);
            dictMilestone[milestone] = GetRevAd(adType);
        }

        public Dictionary<int, float> GetDictMilestoneAd(RevAdType adType)
        {
            var dictMilestoneAd = dictMilestoneBanner;
            if (adType == RevAdType.INTER) dictMilestoneAd = dictMilestoneInter;
            else if (adType == RevAdType.REWARD) dictMilestoneAd = dictMilestoneReward;
            else if (adType == RevAdType.INTER_N_REWARD) dictMilestoneAd = dictMilestoneRewardNInter;
            return dictMilestoneAd;
        }

        public string GetPrefixEvent(RevAdType adType)
        {
            if (adType == RevAdType.INTER) return "rev_inter_";
            if (adType == RevAdType.REWARD) return "rev_reward_";
            if (adType == RevAdType.INTER_N_REWARD) return "rev_inter_n_reward_";
            return "rev_banner_";
        }

        private float GetRevAd(RevAdType adType)
        {
            if (adType == RevAdType.INTER) return revInterAd;
            if (adType == RevAdType.REWARD) return revRewardAd;
            if (adType == RevAdType.INTER_N_REWARD) return revInterNReward;
            return revBannerAd;
        }

        public List<RevMilestone> ListRevMilestones;

        private List<RevMilestone> m_ListSetup = new List<RevMilestone>()
        {
            new RevMilestone("rev_all_ad_1_cent", 0.01f),
            new RevMilestone("rev_all_ad_5_cent", 0.05f),
            new RevMilestone("rev_all_ad_10_cent", 0.1f),
            new RevMilestone("rev_all_ad_50_cent", 0.5f),
            new RevMilestone("rev_all_ad_100_cent", 1f)
        };

        public void OnAdRevenue(AdsEnum adType, float revenue)
        {
            revTotalAd += revenue;
            switch (adType)
            {
               case AdsEnum.REWARD:
                   revRewardAd += revenue;
                   revInterNReward += revenue;
                   Debug.Log("Rev Reward:" + revRewardAd);
                   Debug.Log("Rev Reward & Inter:" + revInterNReward);
                   break;
               case AdsEnum.INTERSTITIAL:
                   revInterAd += revenue;
                   revInterNReward += revenue;
                   Debug.Log("Rev Inter:" + revInterAd);
                   Debug.Log("Rev Reward & Inter:" + revInterNReward);
                   break;
               case AdsEnum.BANNER_TOP:
               case AdsEnum.BANNER_BOTTOM:
                   revBannerAd += revenue;
                   Debug.Log("Rev Banner:" + revBannerAd);
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

        void InitRevMilestone(AdjustAnalyticEventConfig config)
        {
            ListRevMilestones = (config != null) ? config.GetListRevAllAdCentMilestone() : m_ListSetup;
        }

        public float GetRevFBPurchaseValueToSum()
        {
            Debug.Log("Last Rev For Purchase:" + lastRevFBPurchase);
            var valueToSum = revTotalAd - lastRevFBPurchase;
            lastRevFBPurchase = revTotalAd;
            Debug.Log("Value to sum purchase:" + valueToSum);
            return valueToSum;
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
            if (value < step) value = step;
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
