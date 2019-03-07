using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Root
{
    public class TimeServerModule : ITimeServer
    {
        const string REQUEST_URL = "http://worldtimeapi.org/api/ip";

        MonoBehaviour m_Coroutiner;
        bool m_IsHaveServerResult = false;
        DateTime m_Now;

        IJsonService m_Json;

        public TimeServerModule(MonoBehaviour coroutiner, IJsonService json)
        {
            m_Coroutiner = coroutiner;
            m_Json = json;
        }

        public DateTime GetTimeNow()
        {
            if (m_IsHaveServerResult)
            {
                return m_Now;
            }
            else
            {
                return DateTime.Now;
            }
        }

        public bool IsHaveTimeNow()
        {
            return m_IsHaveServerResult;
        }

        public void RequestTimeNow(Action<bool> callback)
        {
            m_IsHaveServerResult = false;
            m_Coroutiner.StartCoroutine(IERequestTime(callback));
        }

        IEnumerator IERequestTime(Action<bool> callback)
        {
            var www = new WWW(REQUEST_URL);
            yield return www;
            if (string.IsNullOrEmpty(www.error))
            {
                Debug.Log("TimeServer: " + www.text);
                var timeData = m_Json.FromJson<TimeData>(www.text);
                if (timeData != null)
                {
                    long timestamp;
                    if (long.TryParse(timeData.unixtime, out timestamp))
                    {
                        m_Now = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                        m_Now = m_Now.AddSeconds(timestamp).ToLocalTime();
                        m_IsHaveServerResult = true;
                        callback(true);
                    }
                    else
                    {
                        Debug.Log("error to long");
                        callback(false);
                    }
                }
                else
                {
                    Debug.Log("error json");
                    callback(false);
                }
            }
            else
            {
                Debug.Log("Error get time server: " + www.error);
                callback(false);
            }
        }

        public class TimeData
        {
            public string unixtime;
        }
    }
}

