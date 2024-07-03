#if (PluginFirebaseRemote)

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Firebase.RemoteConfig;
using Firebase.Extensions;

namespace IEC
{
    public class FirebaseRemoteConfigModule : IDataService
    {
        const string PREFIX = "Remote";
        IJsonService m_JsonService;
        ISetUpSdkService m_FirebaseSetUp;
        IDataService m_LocalData;
        bool m_IsTest;

        Dictionary<string, RemoteConfigData> m_DictData = new Dictionary<string, RemoteConfigData>();
        Dictionary<string, string> m_DictValue = new Dictionary<string, string>();
        private Dictionary<string, object> m_FirebaseDefault = new Dictionary<string, object>();
        private ulong m_FetchTimeout = 60;
        public const string CHEAT_FETCH_TIME_OUT = "CHEAT_FETCH_TIME_OUT";
        private float m_FetchTime;
        public FirebaseRemoteConfigModule(IDataService mLocalData)
        {
            m_LocalData = mLocalData;
        }

        public FirebaseRemoteConfigModule(IJsonService jsonService, ISetUpSdkService firebaseSetUp, IDataService localData, bool isTest, RemoteConfigData[] data, ulong timeOut)
        {
            m_JsonService = jsonService;
            m_FirebaseSetUp = firebaseSetUp;
            m_LocalData = localData;
            m_IsTest = isTest;
            m_FetchTimeout = timeOut;
            Debug.LogWarning("Fetch TimeOut:" + timeOut);
            foreach (var d in data)
            {
                m_DictData[d.keyName] = d;
                m_FirebaseDefault[d.keyName] = GetString(d.keyName, d.defaultValue);
            }
        }

        public void Load(Action<bool> callback)
        {
            if (!IsReady())
            {
                // TODO log firebase failed event 
                return;
            }

            var timeCheat = m_LocalData.GetFloat(CHEAT_FETCH_TIME_OUT, -1);
            var config = new ConfigSettings
            {
                FetchTimeoutInMilliseconds = (ulong)((timeCheat > 0 ? timeCheat : m_FetchTimeout) * 1000)
            };
            
            var firebaseConfigInstance = FirebaseRemoteConfig.DefaultInstance;
            firebaseConfigInstance.SetDefaultsAsync(m_FirebaseDefault).ContinueWithOnMainThread(setConfig =>
            {
                firebaseConfigInstance.SetConfigSettingsAsync(config).ContinueWithOnMainThread(fetch =>
                {
                    var startTime = Time.realtimeSinceStartup;
                    Debug.LogWarning("FetchTimeout Config:" + firebaseConfigInstance.ConfigSettings.FetchTimeoutInMilliseconds);
                    firebaseConfigInstance.FetchAsync(new System.TimeSpan(0)).ContinueWithOnMainThread(taskFetch =>
                    {
                        m_FetchTime = Time.realtimeSinceStartup - startTime;
                        var info = firebaseConfigInstance.Info;
                        switch (info.LastFetchStatus)
                        {
                            case LastFetchStatus.Success:
                                firebaseConfigInstance.ActivateAsync();
                                Debug.Log(String.Format("Remote data loaded and ready (last fetch time {0}).", info.FetchTime));

                                foreach (var key in firebaseConfigInstance.Keys)
                                {
                                    var value = firebaseConfigInstance.GetValue(key);
                                    m_DictValue[key] = value.StringValue;
                                    Debug.LogWarning($"[ABRCT] Remote: [{key}, {value.StringValue}]");
                                }

                                foreach (var key in m_DictData.Keys)
                                {
                                    var remoteValue = GetRemoteValue(key);
                                    if (!string.IsNullOrEmpty(remoteValue))
                                    {
                                        m_LocalData.SetString(GetSaveKey(key), GetRemoteValue(key));
                                    }
                                }
                                Save();
                                break;
                            case LastFetchStatus.Failure:
                                switch (info.LastFetchFailureReason)
                                {
                                    // TODO log firebase failed event ?
                                    case FetchFailureReason.Error:
                                        Debug.Log("Fetch failed for unknown reason");
                                        break;
                                    case FetchFailureReason.Throttled:
                                        Debug.Log("Fetch throttled until " + info.ThrottledEndTime);
                                        break;
                                }
                                break;
                            case LastFetchStatus.Pending:
                                Debug.Log("Latest Fetch call still pending.");
                                break;
                        }
                        callback?.Invoke(info.LastFetchStatus == LastFetchStatus.Success);
                    });
                });
            });
        }

        #region Private Functions

        public bool IsReady()
        {
            return m_FirebaseSetUp.IsReady();
        }

        string GetSaveKey(string key)
        {
            return PREFIX + key;
        }

        string GetConfigValue(string key)
        {
            if (m_DictData.ContainsKey(key))
            {
                if (m_IsTest)
                {
                    return m_DictData[key].testValue;
                }

                return m_DictData[key].defaultValue;
            }
            return string.Empty;
        }

        string GetRemoteValue(string key)
        {
            if (m_DictData.ContainsKey(key))
            {
                if (m_IsTest)
                {
                    return m_DictData[key].testValue;
                }

                if (m_DictValue.ContainsKey(key))
                {
                    return m_DictValue[key];
                }
            }
            return string.Empty;
        }

        #endregion

        #region Helper Functions

        public void SetInt(string key, int value)
        {
            SetString(GetSaveKey(key), value.ToString());
        }

        public void SetBool(string key, bool value)
        {
            SetString(GetSaveKey(key), value.ToString());
        }

        public void SetFloat(string key, float value)
        {
            SetString(GetSaveKey(key), value.ToString());
        }

        public void SetString(string key, string value)
        {
            m_LocalData.SetString(GetSaveKey(key), value);
        }

        public void SetObject(string key, object value)
        {
            var json = m_JsonService.ToJson(value);
            SetString(GetSaveKey(key), json);
        }

        public int GetInt(string key, int defaultValue)
        {
            var saveKey = GetSaveKey(key);
            var localValue = m_LocalData.GetString(saveKey, string.Empty);
            var configValue = GetConfigValue(key);
            var remoteValue = GetRemoteValue(key);

            int convertValue = 0;
            if (!int.TryParse(remoteValue, out convertValue))
            {
                if (!int.TryParse(localValue, out convertValue))
                {
                    if (!int.TryParse(configValue, out convertValue))
                    {
                        return defaultValue;
                    }
                }
            }
            return convertValue;
        }

        public bool GetBool(string key, bool defaultValue)
        {
            var saveKey = GetSaveKey(key);
            var localValue = m_LocalData.GetString(saveKey, string.Empty);
            var configValue = GetConfigValue(key);
            var remoteValue = GetRemoteValue(key);

            bool convertValue = false;
            if (!bool.TryParse(remoteValue, out convertValue))
            {
                if (!bool.TryParse(localValue, out convertValue))
                {
                    if (!bool.TryParse(configValue, out convertValue))
                    {
                        return defaultValue;
                    }
                }
            }
            return convertValue;
        }

        public float GetFloat(string key, float defaultValue)
        {
            var saveKey = GetSaveKey(key);
            var localValue = m_LocalData.GetString(saveKey, string.Empty);
            var configValue = GetConfigValue(key);
            var remoteValue = GetRemoteValue(key);

            float convertValue = 0f;
            if (!float.TryParse(remoteValue, out convertValue))
            {
                if (!float.TryParse(localValue, out convertValue))
                {
                    if (!float.TryParse(configValue, out convertValue))
                    {
                        return defaultValue;
                    }
                }
            }
            return convertValue;
        }

        public string GetString(string key, string defaultValue)
        {
            var saveKey = GetSaveKey(key);
            var localValue = m_LocalData.GetString(saveKey, string.Empty);
            var configValue = GetConfigValue(key);
            var remoteValue = GetRemoteValue(key);

            if (string.IsNullOrEmpty(remoteValue))
            {
                if (string.IsNullOrEmpty(localValue))
                {
                    if (string.IsNullOrEmpty(configValue))
                    {
                        return defaultValue;
                    }
                    return configValue;
                }
                return localValue;
            }
            return remoteValue;
        }

        public T GetObject<T>(string key, T defaultValue)
        {
            var json = GetString(key, string.Empty);

            if (string.IsNullOrEmpty(json)) return defaultValue;

            var obj = m_JsonService.FromJson<T>(json);
            obj = (obj == null) ? defaultValue : obj;
            return obj;
        }

        public void DefaultOverrideRemote()
        {
            foreach (var d in m_FirebaseDefault)
            {
                SetString(GetSaveKey(d.Key), d.Value.ToString());
                m_DictValue[d.Key] = d.Value.ToString();
            }
            Save();
        }

        public void Save()
        {
            m_LocalData.Save();
        }

        public bool HasKey(string key)
        {
            return m_LocalData.HasKey(GetSaveKey(key)) || m_DictData.ContainsKey(key) || m_DictValue.ContainsKey(key);
        }

        public float FetchTime => m_FetchTime;

        #endregion
    }
}

#endif