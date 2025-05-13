using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _IECModules;
#if PluginFirebaseAnalytic
using Firebase.Analytics;
#endif
using UnityEngine;

namespace IEC
{
    public class InternalEvent
    {
        private class Param
        {
            public string key;
            public object value;
        }
        private class EventBody
        {
            public string appId => Application.identifier;
            public string eventName;
            public long timestamp => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            public string userId => Pool.Get<ServiceData>().firebasePseudoId;
            public string appVersion => Application.version;
            public List<Param> eventParameters = new List<Param>();
            public string sessionId;

        }
        const string URL = "https://event-api.haleinteractive.vn/api/events";
        private static EventBody m_Body = new EventBody();
        public static void Push(string eventName, Dictionary<string, object> eventParams = null)
        {
            m_Body.eventName = eventName;
            m_Body.eventParameters.Clear();
            if (eventParams != null)
            {
                m_Body.eventParameters = eventParams.Select(x=> new Param { key = x.Key, value = x.Value }).ToList();
            }
            
            var action = new Action(() =>
            {
                NetworkBase.Post(URL, m_Body);
            });
#if PluginFirebaseAnalytic
            Controllers.Get<ServicesController>().FirebaseAnalyticModule.RequestSessionId(s =>
            {
                m_Body.sessionId = s.ToString();
                action.Invoke();
            });
#else   
            action.Invoke();
#endif
        }
    }
}
