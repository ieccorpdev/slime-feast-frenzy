#if (PluginFirebaseAnalytic)

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public class FirebaseAnalyticModule : BaseAnalyticModule, IAnalyticEvent
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

        public void RequestPseudoId(Action<string> onDone)
        {
           
        }

        public void Init(AdjustAnalyticEventConfig adjustAnalyticEventConfig)
        {
            InitEventConfig(adjustAnalyticEventConfig);
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
            if (CheckPushedUniqueEvent(eventName, AnalyticDataParamType.Firebase)) return;
            
            var newName = eventName.Replace(' ', '_').Replace('-', '_');

            if (param != null && param.ContainsKey("level"))
            {
                value = (int)param["level"];
            }
         

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
                       
                    }
                    else if (typeValue == typeof(string))
                    {
                        
                    }
                    else if (typeValue == typeof(bool))
                    {
                       
                    }
                    else
                    {
                        Debug.LogWarning("Firebase doesn't support this type");
                    }

                }
            }

            Debug.Log("Push Firebase Event: " + eventName + ", " + value);
        }

        public void SetUserProperty(string name, string property)
        {
            if (IsDisable)
            {
                return;
            }

            if (IsFirebaseReady())
            {
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