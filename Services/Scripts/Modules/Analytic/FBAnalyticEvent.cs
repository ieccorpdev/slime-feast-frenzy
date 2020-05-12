#if (PluginFacebook)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;
namespace IEC
{
    public class FBAnalyticEvent : IAnalyticEvent
    {
        List<AnalyticEventData> m_ListCachedEvents = new List<AnalyticEventData>();

        public void Init()
        {

        }

        public void PushCachedEvents()
        {
            if (FB.IsInitialized)
            {
                if (m_ListCachedEvents.Count == 0) return;
                Debug.Log("Push Facebook CachedEvents: " + m_ListCachedEvents.Count);
                m_ListCachedEvents.ForEach(evt => PushEvent(evt.eventName, evt.value, evt.param));
                m_ListCachedEvents.Clear();
            }
        }

        public void PushEvent(string eventName, float value = 1, Dictionary<string, object> param = null)
        {
            if (FB.IsInitialized)
            {
                Debug.Log("Push Facebook Event: " + eventName + ", " + value);
                FB.LogAppEvent(eventName, value, param);
            }
            else
            {
                Debug.Log("Push Facebook Event Failed: " + eventName);
                m_ListCachedEvents.Add(new AnalyticEventData { eventName = eventName, value = value, param = param });
            }
        }

        public void SetUserProperty(string name, string property)
        {
        }
    }
}

#endif