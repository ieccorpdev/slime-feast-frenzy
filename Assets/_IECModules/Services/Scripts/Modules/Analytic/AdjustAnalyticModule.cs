#if (PluginAdjust)

using System;
using System.Collections;
using System.Collections.Generic;
using com.adjust.sdk;
using UnityEngine;

namespace IEC
{
    public class AdjustAnalyticModule : IAnalyticEvent
    {
        public bool IsDisable { get; set; }

        public Dictionary<string, string> DictEvent;
        private List<string> m_ListEventRev = new List<string>();

        public const string ADJUST_SANDBOX = "ADJUST_SAND_BOX";

        public AdjustAnalyticModule(string androidID, string iosID, Action<string> deferredDeeplinkCallback = null)
        {
            var appID = (Application.platform == RuntimePlatform.Android) ? androidID : iosID;
            Debug.LogWarning("Adjust AppID: " + (string.IsNullOrWhiteSpace(appID) ? "null" : appID));

            var isEnableSandBox = PlayerPrefs.GetInt(ADJUST_SANDBOX, 0) == 1;
            
            var adjustConfig = new AdjustConfig(appID, (isEnableSandBox) ? AdjustEnvironment.Sandbox : AdjustEnvironment.Production, true);
#if UNITY_ANDROID
            adjustConfig.setPreinstallTrackingEnabled(true);
#endif
            adjustConfig.setLogLevel(AdjustLogLevel.Suppress);
            adjustConfig.setSendInBackground(false);
            adjustConfig.setEventBufferingEnabled(false);
            adjustConfig.setLaunchDeferredDeeplink(false);
            adjustConfig.setDeferredDeeplinkDelegate(deferredlink =>
            {
                deferredDeeplinkCallback?.Invoke(deferredlink);
            });
            Adjust.start(adjustConfig);
            Debug.LogWarning("Adjust Version:" + Adjust.getSdkVersion());
        }

        public void Init()
        {

        }

        public void SetListEventRev(List<string> listEventRev)
        {
            m_ListEventRev = listEventRev;
        }


        public void PushCachedEvents()
        {
            if (IsDisable) return;
        }

        public void PushEvent(string eventName, float value = 1, Dictionary<string, object> param = null)
        {
            if (IsDisable || DictEvent == null || !DictEvent.ContainsKey(eventName)) return;
            var adjustEvent = new AdjustEvent(DictEvent[eventName]);
            var adjustLog = $"Push Adjust Event:{eventName}__{DictEvent[eventName]}";
            if (m_ListEventRev != null && m_ListEventRev.Contains(eventName))
            {
                adjustEvent.setRevenue(value, "USD");
                adjustLog += $"__Rev:{value}";
            }
            Adjust.trackEvent(adjustEvent);
            Debug.Log(adjustLog);
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
