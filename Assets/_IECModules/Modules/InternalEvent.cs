using System;
using System.Collections;
using System.Collections.Generic;
#if PluginFirebaseAnalytic
using Firebase.Analytics;
#endif
using UnityEngine;

namespace IEC
{
    public class InternalEvent
    {
        public void Push(string eventName, Dictionary<string, object> eventParams)
        {
            var timestamp = DateTime.Now.Ticks;
            var userId = Pool.Get<ServiceData>().firebasePseudoId;
            var appVersion = Application.version;
            var appId = Application.identifier;
            var sessionId = string.Empty;
#if PluginFirebaseAnalytic
            (FirebaseAnalyticModule)Controllers.Get<ServicesController>().RequestSessionId(s =>
            {
                
            })
#endif
        }
    }
}
