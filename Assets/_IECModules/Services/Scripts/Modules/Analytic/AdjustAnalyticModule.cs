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

        public Dictionary<string, string> DictEvent;
        private List<string> m_ListEventRev = new List<string>();

        public const string ADJUST_SANDBOX = "ADJUST_SAND_BOX";

        public AdjustAnalyticModule(string androidID, string iosID)
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
            Adjust.start(adjustConfig);
            EnableThirdPartySharing();
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

        public void PushEvent(string eventName, float value = 1, Dictionary<string, object> param = null, string isoCurrencyCode = "USD")
        {
            if (IsDisable || DictEvent == null || !DictEvent.ContainsKey(eventName)) return;
            var adjustEvent = new AdjustEvent(DictEvent[eventName]);
            var adjustLog = $"Push Adjust Event:{eventName}__{DictEvent[eventName]}";
            if (m_ListEventRev != null && m_ListEventRev.Contains(eventName))
            {
                adjustEvent.setRevenue(value, isoCurrencyCode);
                adjustLog += $"__Rev:{value} | Currency:{isoCurrencyCode}";
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

        public void EnableThirdPartySharing()
        {
            AdjustThirdPartySharing adjustThirdPartySharing = new AdjustThirdPartySharing(true);
            Adjust.trackThirdPartySharing(adjustThirdPartySharing);
            Debug.LogWarning("Adjust enable third party sharing");
        }
    }
}

#endif
