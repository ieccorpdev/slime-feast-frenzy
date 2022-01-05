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

        [NonSerialized]
        public Dictionary<string, Func<Dictionary<string, object>>> dictCustomParams;

        public Action<string, float, Dictionary<string, object>> pushEventAction;

        public void SetDictEventParam(string eventName, Func<Dictionary<string, object>> funcParams)
        {
            if (funcParams == null) return;

            if (dictCustomParams == null)
            {
                dictCustomParams = new Dictionary<string, Func<Dictionary<string, object>>>();
            }
            dictCustomParams[eventName] = funcParams;
        }
        
        
        public const string APPLOVIN_REV_ALL_AD = "APPLOVIN_REV_ALL_AD";
        public const string REV_ALL_AD = "REV_ALL_AD";
        public const string REV_INTER_AD = "REV_INTER_AD";
        public const string REV_REWARD_AD = "REV_REWARD_AD";
        public const string REV_BANNER_AD = "REV_BANNER_AD";
        
        public float RevAd
        {
            set
            {
                if (PlayerPrefs.HasKey(APPLOVIN_REV_ALL_AD))
                {
                    PlayerPrefs.DeleteKey(APPLOVIN_REV_ALL_AD);
                }
                PlayerPrefs.SetFloat(REV_ALL_AD,value);
            }
            get
            {
                if (PlayerPrefs.HasKey(APPLOVIN_REV_ALL_AD))
                {
                    var saveValue = PlayerPrefs.GetFloat(APPLOVIN_REV_ALL_AD, 0f);
                    PlayerPrefs.SetFloat(REV_ALL_AD, saveValue);
                    PlayerPrefs.DeleteKey(APPLOVIN_REV_ALL_AD);
                }
                return PlayerPrefs.GetFloat(APPLOVIN_REV_ALL_AD, 0f);
            }
        }

        public float RevInterAd
        {
            set => PlayerPrefs.SetFloat(REV_INTER_AD, value);
            get => PlayerPrefs.GetFloat(REV_INTER_AD, 0f);
        }

        public float RevRewardAd
        {
            set => PlayerPrefs.SetFloat(REV_REWARD_AD, value);
            get => PlayerPrefs.GetFloat(REV_REWARD_AD, 0f);
        }

        public float RevBannerAd
        {
            set => PlayerPrefs.SetFloat(REV_BANNER_AD, value);
            get => PlayerPrefs.GetFloat(REV_BANNER_AD, 0f);
        }
        
        class RevMilestone
        {
            public float step;

            public float Value
            {
                set
                {
                    if (PlayerPrefs.HasKey(saveMilestoneKey))
                    {
                        PlayerPrefs.DeleteKey(saveMilestoneKey);
                    }
                    PlayerPrefs.SetFloat(saveMilestoneKeyNew,value);
                }
                get
                {
                    if (PlayerPrefs.HasKey(saveMilestoneKey))
                    {
                        var saveValue = PlayerPrefs.GetFloat(saveMilestoneKey);
                        PlayerPrefs.SetFloat(saveMilestoneKeyNew, saveValue);
                        PlayerPrefs.DeleteKey(saveMilestoneKey);
                    }
                    return PlayerPrefs.GetFloat(saveMilestoneKeyNew, step);
                }
            }

            public float LastRev
            {
                set
                {
                    if (PlayerPrefs.HasKey(saveLastRevKey))
                    {
                        PlayerPrefs.DeleteKey(saveLastRevKey);
                    }
                    PlayerPrefs.SetFloat(saveLastRevKeyNew,value);
                }
                get
                {
                    if (PlayerPrefs.HasKey(saveLastRevKey))
                    {
                        var saveValue = PlayerPrefs.GetFloat(saveLastRevKey);
                        PlayerPrefs.SetFloat(saveLastRevKeyNew, saveValue);
                        PlayerPrefs.DeleteKey(saveLastRevKey);
                    }
                    return PlayerPrefs.GetFloat(saveLastRevKey, 0f);
                }
            }
            
            public string eventName;

            private string saveMilestoneKey => $"APPLOVIN_REV_{eventName}";
            private string saveMilestoneKeyNew => $"REV_{eventName}";
            private string saveLastRevKey => $"APPLOVIN_LAST_REV_{eventName}";
            private string saveLastRevKeyNew => $"LAST_REV_{eventName}";
            public RevMilestone(string eventName,float step)
            {
                this.eventName = eventName;
                this.step = step;
            }

            public bool IsEnoughRev(float revCheck)
            {
                if (revCheck < Value) return false;
                var multi = (int) (revCheck / step);
                Value = (multi + 1) * step;
                Debug.Log($"{eventName} new milestone: {Value}");
                return true;
            }

            public float RaiseRev(float rev)
            {
                var valueToSum = rev - LastRev;
                Debug.Log($"{eventName} lastRev: {LastRev}");
                Debug.Log($"{eventName} valueToSum: {valueToSum}");
                LastRev = rev;
                return valueToSum;
            }
        }
        
        private List<RevMilestone> m_ListRevMilestones = new List<RevMilestone>
        {
            new RevMilestone("rev_all_ad_1_cent", 0.01f),
            new RevMilestone("rev_all_ad_5_cent", 0.05f),
            new RevMilestone("rev_all_ad_10_cent", 0.1f),
            new RevMilestone("rev_all_ad_50_cent", 0.5f),
            new RevMilestone("rev_all_ad_100_cent", 1f)
        };

        public void OnAdRevenue(AdsEnum adType, float revenue)
        {
            RevAd += revenue;
            switch (adType)
            {
               case AdsEnum.REWARD:
                   RevRewardAd += revenue;
                   break;
               case AdsEnum.INTERSTITIAL:
                   RevInterAd += revenue;
                   break;
               case AdsEnum.BANNER_TOP:
               case AdsEnum.BANNER_BOTTOM:
                   RevBannerAd += revenue;
                   break;
            }
            Debug.Log("Total Rev:" + RevAd);
            foreach (var milestone in m_ListRevMilestones)
            {
                if (milestone.IsEnoughRev(RevAd))
                {
                    var valueToSum = milestone.RaiseRev(RevAd);
                    pushEventAction?.Invoke(milestone.eventName,valueToSum,null);
                }
            }
            
            PlayerPrefs.Save();
        }
    }
}
