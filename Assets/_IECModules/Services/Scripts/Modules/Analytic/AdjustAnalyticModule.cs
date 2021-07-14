#if (PluginAdjust)

using System.Collections;
using System.Collections.Generic;
using com.adjust.sdk;
using UnityEngine;

namespace IEC
{
    public class AdjustAnalyticModule : IAnalyticEvent
    {
        public bool IsDisable { get; set; }

        public AdjustAnalyticModule(string androidID, string iosID)
        {
            var appID = (Application.platform == RuntimePlatform.Android) ? androidID : iosID;
            Debug.LogWarning("Adjust AppID: " + (string.IsNullOrWhiteSpace(appID) ? "null" : appID));
            
            var adjustConfig = new AdjustConfig(appID, AdjustEnvironment.Production, true);
            adjustConfig.setLogLevel(AdjustLogLevel.Suppress);
            adjustConfig.setSendInBackground(false);
            adjustConfig.setEventBufferingEnabled(false);
            adjustConfig.setLaunchDeferredDeeplink(false);
            Adjust.start(adjustConfig);
            Debug.LogWarning("Adjust Version:" + Adjust.getSdkVersion());

        }

        public void Init()
        {

        }

        public void PushCachedEvents()
        {
            if (IsDisable) return;
        }

        public void PushEvent(string eventName, float value = 1, Dictionary<string, object> param = null)
        {
            if (IsDisable) return;
        }

        public void SetUserProperty(string name, string property)
        {
            if (IsDisable) return;
        }

        public void PushEventFullAd()
        {
            return;
            if (IsDisable) return;
        }

        public void PushEventRewardAd()
        {
            return;
            if (IsDisable) return;
        }
    }
}

#endif