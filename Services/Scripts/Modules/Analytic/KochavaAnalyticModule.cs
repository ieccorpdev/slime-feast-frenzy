#if (PluginKochava)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public class KochavaAnalyticModule : IAnalyticEvent
    {
        public bool IsDisable { get; set; }

        public KochavaAnalyticModule(string androidID, string iosID)
        {
            var appID = (Application.platform == RuntimePlatform.Android) ? androidID : iosID;
            Debug.LogError("Kochava AppID: " + (string.IsNullOrWhiteSpace(appID) ? "null" : appID));
            Kochava.Tracker.Config.SetAppGuid(appID);
            Kochava.Tracker.Config.SetLogLevel(Kochava.DebugLogLevel.info);
            Kochava.Tracker.Config.SetRetrieveAttribution(true);
            Kochava.Tracker.Initialize();
            var kochavaVersion = Kochava.Tracker.GetVersion();
            Debug.LogError("Kochava Version:" + kochavaVersion);

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
            if (Kochava.IsInitialized())
            {
                Debug.Log("kochava show full ad");
                Kochava.Event myEvent = new Kochava.Event(Kochava.EventType.AdView);
                Kochava.Tracker.SendEvent(myEvent);
            }
        }

        public void PushEventRewardAd()
        {
            return;
            if (IsDisable) return;
            if (Kochava.IsInitialized())
            {
                Debug.Log("kochava show reward ad");
                Kochava.Event myEvent = new Kochava.Event(Kochava.EventType.Achievement);
                Kochava.Tracker.SendEvent(myEvent);
            }
        }
    }
}

#endif