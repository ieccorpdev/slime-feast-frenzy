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
        public Balaso.AppTrackingTransparency.AuthorizationStatus? attStatus;

        [NonSerialized]
        public Dictionary<string, Func<Dictionary<string, object>>> dictCustomParams;

        public void SetDictEventParam(string eventName, Func<Dictionary<string, object>> funcParams)
        {
            if (funcParams == null) return;

            if (dictCustomParams == null)
            {
                dictCustomParams = new Dictionary<string, Func<Dictionary<string, object>>>();
            }
            dictCustomParams[eventName] = funcParams;
        }
    }
}
