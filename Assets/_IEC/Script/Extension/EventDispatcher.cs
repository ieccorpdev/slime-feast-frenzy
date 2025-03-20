using System;
using System.Collections.Generic;
using UnityEngine;

namespace _IEC.Features.Common.Extension
{
    [ExecuteAlways]
    public static class EventDispatcher
    {
        private static readonly Dictionary<string, Action<object>> EventDictionary = new Dictionary<string, Action<object>>();
    
        public static void RegisterListener(string eventName, Action<object> listener)
        {
            if (EventDictionary.TryGetValue(eventName, out Action<object> thisEvent))
            {
                thisEvent += listener;
                EventDictionary[eventName] = thisEvent;
            }
            else
            {
                thisEvent += listener;
                EventDictionary.Add(eventName, thisEvent);
            }
        }
        
        public static void RegisterListener(this object registerer,string eventName, Action<object> listener)
        {
            if (EventDictionary.TryGetValue(eventName, out Action<object> thisEvent))
            {
                thisEvent += listener;
                EventDictionary[eventName] = thisEvent;
            }
            else
            {
                thisEvent += listener;
                EventDictionary.Add(eventName, thisEvent);
            }
        }

        public static void UnregisterListener(string eventName, Action<object> listener)
        {
            if (EventDictionary.TryGetValue(eventName, out Action<object> thisEvent))
            {
                thisEvent -= listener;
                if (thisEvent == null)
                {
                    EventDictionary.Remove(eventName);
                }
                else
                {
                    EventDictionary[eventName] = thisEvent;
                }
            }
        }
        
        public static void UnregisterListener(this object unRegisterer,string eventName, Action<object> listener)
        {
            if (EventDictionary.TryGetValue(eventName, out Action<object> thisEvent))
            {
                thisEvent -= listener;
                if (thisEvent == null)
                {
                    EventDictionary.Remove(eventName);
                }
                else
                {
                    EventDictionary[eventName] = thisEvent;
                }
            }
        }

        public static void RaiseEvent(string eventName, object eventData = null)
        {
            if (EventDictionary.TryGetValue(eventName, out Action<object> thisEvent))
            {
                thisEvent.Invoke(eventData);
            }
        }  
        public static void RaiseEvent(this object raiser,string eventName, object eventData = null)
        {
            if (EventDictionary.TryGetValue(eventName, out Action<object> thisEvent))
            {
                thisEvent.Invoke(eventData);
            }
        }  
    }
}