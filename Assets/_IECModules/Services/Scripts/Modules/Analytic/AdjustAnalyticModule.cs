#if (PluginAdjust)

using System;
using System.Collections;
using System.Collections.Generic;
using com.adjust.sdk;
using UnityEngine;

namespace IEC
{
    public class AdjustAnalyticModule : BaseAnalyticModule, IAnalyticEvent
    {
        public bool IsDisable { get; set; }
        public string AppID { get; private set; }

        public Dictionary<string, string> DictEvent;
        private List<string> m_ListEventRev = new List<string>();

        public const string ADJUST_SANDBOX = "ADJUST_SAND_BOX";
        private const string ADJUST_IN_APP_PURCHASE = "adjust_in_app_purchase";

        public void Init()
        {

        }

        public void Init(AdjustAnalyticEventConfig adjustAnalyticEventConfig)
        {
            InitEventConfig(adjustAnalyticEventConfig);
        }

        public void InitAdjust(string androidID, string iosID, Action<string, AdjustAttribution> onDone)
        {
            var appID = (Application.platform == RuntimePlatform.Android) ? androidID : iosID;
            AppID = appID;
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
            onDone?.Invoke(Adjust.getAdid(), Adjust.getAttribution());
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
            if (CheckPushedUniqueEvent(eventName, AnalyticDataParamType.Adjust)) return;
            var adjustEvent = new AdjustEvent(DictEvent[eventName]);
            var adjustLog = $"Push Adjust Event:{eventName}__{DictEvent[eventName]}";
            var currencyCode = "USD";
            if(param.ContainsKey("currencyCode"))
            {
                currencyCode = param["currencyCode"].ToString();
            }
            if (m_ListEventRev != null && m_ListEventRev.Contains(eventName))
            {
                adjustEvent.setRevenue(value, currencyCode);
                adjustLog += $"__Rev:{value} | Currency:{currencyCode}";
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

        public string GetIAPEventId()
        {
            DictEvent.TryGetValue(ADJUST_IN_APP_PURCHASE, out var eventId);
            return eventId;
        }
    }
}

#endif
