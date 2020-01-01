using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace IEC
{
    public class TimeServerModule : ITimeServer
    {
        public const string REQUEST_URL = "http://apigame.dtac.vn/helper/get_time_utc";

        string serverURL = REQUEST_URL;

        MonoBehaviour m_Coroutiner;
        bool m_IsHaveServerResult = false;
        DateTime m_NowToLocal;
        DateTime m_Now;
        IJsonService m_Json;

        List<Action<bool>> m_ListCallback;
        Coroutine m_RequestCorou;

        public TimeServerModule(MonoBehaviour coroutiner, IJsonService json)
        {
            m_Coroutiner = coroutiner;
            m_Json = json;
            m_ListCallback = new List<Action<bool>>();
        }

        public DateTime GetTimeNowToLocal()
        {
            if (m_IsHaveServerResult)
            {
                return m_NowToLocal;
            }
            else
            {
                return DateTime.Now;
            }
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
            m_ListCallback.Add(callback);

            if (m_RequestCorou == null)
            {
                m_IsHaveServerResult = false;
                m_RequestCorou = m_Coroutiner.StartCoroutine(IERequestTime(callback));
            }
        }

        IEnumerator IERequestTime(Action<bool> callback)
        {
            var www = UnityWebRequest.Get(serverURL);
            var time = Time.time;
            yield return www.SendWebRequest();
            Debug.Log("time run:" + (Time.time - time));
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log("Error get time server: " + www.error);
                RaiseCallbacks(false);
            }
            else
            {
                Debug.Log("TimeServer: " + www.downloadHandler.text);
                //var timeData = m_Json.FromJson<TimeData>(www.downloadHandler.text);
                //if (timeData != null)
                //{
                var timeData = www.downloadHandler.text;
                long timestamp;
                if (long.TryParse(timeData, out timestamp))
                {
                    m_NowToLocal = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                    m_NowToLocal = m_NowToLocal.AddSeconds(timestamp).ToLocalTime();
                    m_Now = m_NowToLocal.ToUniversalTime();
                    Debug.Log("Time Server now:" + m_Now.ToString());
                    Debug.Log("Time Server to Local now:" + m_NowToLocal.ToString());
                    m_IsHaveServerResult = true;
                    //callback(true);
                    RaiseCallbacks(true);

                }
                else
                {
                    Debug.Log("error to long");
                    //callback(false);
                    RaiseCallbacks(false);
                }
                //}
                //else
                //{
                //    Debug.Log("error json");
                //    RaiseCallbacks(false);
                //}
            }

            m_RequestCorou = null;
        }

        public class TimeData
        {
            public string unixtime;
        }

        void RaiseCallbacks(bool result)
        {
            for (int i = 0; i < m_ListCallback.Count; i++)
            {
                m_ListCallback[i](result);
            }
            m_ListCallback.Clear();
        }

        public void SetServerURL(string url)
        {
            serverURL = url;
            Debug.Log("Set server url:" + serverURL);
        }
    }
}

