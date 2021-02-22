using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace IEC
{
    public class AnalyticEventMock : IAnalyticEvent
    {
        public bool IsDisable { get; set; }
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
            Debug.Log($"AnalyticMockEvent {eventName}: {JsonConvert.SerializeObject(param)}");
            // if (param != null)
            // {
            //     foreach(var key in param.Keys)
            //     {
            //         Debug.Log("Param key: " + key);
            //         Debug.Log("Param value: " + param[key]);
            //     }
            // }
        }

        public void SetUserProperty(string name, string property)
        {
            if (IsDisable) return;
            Debug.Log("AnalyticMock SetUserProperty: " + name + "; " + property);
        }
    }

}