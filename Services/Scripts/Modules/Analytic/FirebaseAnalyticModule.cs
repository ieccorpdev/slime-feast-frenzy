#if (PluginFirebaseAnalytic)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Analytics;

namespace IEC
{
    public class FirebaseAnalyticModule : IAnalyticEvent
    {
        List<AnalyticEventData> m_ListCachedEvents = new List<AnalyticEventData>();
        ISetUpSdkService m_FirebaseSDK;

        Dictionary<string, string> m_CacheSetProperty = new Dictionary<string, string>();
        private List<string> m_ListIgnoredEvent = new List<string>();

        public bool IsDisable { get ; set; }

        public FirebaseAnalyticModule(ISetUpSdkService firebaseSDK, List<string> listIgnore)
        {
            m_FirebaseSDK = firebaseSDK;
            m_ListIgnoredEvent = listIgnore;
        }

        public void Init()
        {
            if (IsDisable)
            {
                return;
            }
        }

        public void PushCachedEvents()
        {
            if (IsDisable)
            {
                return;
            }
            if (m_ListCachedEvents.Count != 0)
            {
                Debug.Log("Push Firebase Cached Events: " + m_ListCachedEvents.Count);
                var cacheEvents = new List<AnalyticEventData>(m_ListCachedEvents);
                m_ListCachedEvents.Clear();
                cacheEvents.ForEach(evt => PushEvent(evt.eventName, evt.value, evt.param));
            }

            if (m_CacheSetProperty.Count != 0)
            {
                Debug.Log("Set Firebase Cached Property: " + m_CacheSetProperty.Count);
                var cacheDict = new Dictionary<string, string>(m_CacheSetProperty);
                m_CacheSetProperty.Clear();
                foreach (var name in cacheDict.Keys)
                {
                    SetUserProperty(name, cacheDict[name]);
                }
            }
        }

        public void PushEvent(string eventName, float value = 1, Dictionary<string, object> param = null)
        {
            if(IsDisable)
            {
                return;
            }

            if (!IsFirebaseReady())
            {
                Debug.Log("Push Firebase Event Failed: " + eventName);
                m_ListCachedEvents.Add(new AnalyticEventData { eventName = eventName, value = value, param = param });
                return;
            }
            
            if(m_ListIgnoredEvent != null && m_ListIgnoredEvent.Contains(eventName)) return;
            
            var newName = eventName.Replace(' ', '_').Replace('-', '_');

            if (param != null && param.ContainsKey("level"))
            {
                value = (int)param["level"];
            }
            var firebaseParams = new List<Parameter>
            {
                new Parameter(FirebaseAnalytics.ParameterValue, value),
                new Parameter(FirebaseAnalytics.ParameterCurrency, "USD")
            };

            if (param != null)
            {
                var keys = param.Keys;
                foreach (var key in keys)
                {
                    var typeValue = param[key].GetType();
                    Debug.Log(typeValue.ToString());
                    if (typeValue == typeof(int) || typeValue == typeof(long))
                    {
                        var isTypeInt = (typeValue == typeof(int));
                        long valueL = 0;
                        if (isTypeInt)
                        {
                            valueL = (int)param[key];
                        }
                        else
                        {
                            valueL = (long)param[key];
                        }

                        var firebaseParam = new Parameter(key, valueL);
                        firebaseParams.Add(firebaseParam);
                    }
                    else if (typeValue == typeof(float) || typeValue == typeof(double))
                    {
                        var isTypeFloat = (typeValue == typeof(float));
                        double valueD = 0.0;
                        if (isTypeFloat)
                        {
                            valueD = (float)param[key];
                        }
                        else
                        {
                            valueD = (double)param[key];
                        }
                        var firebaseParam = new Parameter(key, valueD);
                        firebaseParams.Add(firebaseParam);
                    }
                    else if (typeValue == typeof(string))
                    {
                        var firebaseParam = new Parameter(key, (string)param[key]);
                        firebaseParams.Add(firebaseParam);
                    }

                }
            }

            var firebaseParamArr = firebaseParams.ToArray();
            Debug.Log("Push Firebase Event: " + eventName + ", " + value);
            FirebaseAnalytics.LogEvent(newName, firebaseParamArr);
        }

        public void SetUserProperty(string name, string property)
        {
            if (IsDisable)
            {
                return;
            }

            if (IsFirebaseReady())
            {
                FirebaseAnalytics.SetUserProperty(name, property);
            }
            else
            {
                m_CacheSetProperty[name] = property;
            }
        }

        bool IsFirebaseReady()
        {
            return m_FirebaseSDK.IsReady();
        }
    }

}

#endif