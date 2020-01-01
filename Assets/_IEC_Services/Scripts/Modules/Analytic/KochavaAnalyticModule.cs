#if (PluginKochava)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Root
{
    public class KochavaAnalyticModule : IAnalyticEvent
    {
        public KochavaAnalyticModule(string androidID, string iosID)
        {
            var appID = (Application.platform == RuntimePlatform.Android) ? androidID : iosID;
            Kochava.Tracker.Config.SetAppGuid(appID);
            Kochava.Tracker.Config.SetLogLevel(Kochava.DebugLogLevel.info);
            Kochava.Tracker.Config.SetRetrieveAttribution(true);
            Kochava.Tracker.Initialize();
        }

        public void Init()
        {

        }

        public void PushCachedEvents()
        {
        }

        public void PushEvent(string eventName, float value = 1, Dictionary<string, object> param = null)
        {
        }

        public void SetUserProperty(string name, string property)
        {
        }

        public void PushEventFullAd()
        {
            if (Kochava.IsInitialized())
            {
                Debug.Log("kochava show full ad");
                Kochava.Event myEvent = new Kochava.Event(Kochava.EventType.AdView);
                Kochava.Tracker.SendEvent(myEvent);
            }
        }

        public void PushEventRewardAd()
        {
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