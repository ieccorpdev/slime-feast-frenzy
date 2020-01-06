#if (PluginFirebaseRemote)

using System;
using System.Collections.Generic;
using UnityEngine;
using Firebase.RemoteConfig;
using Firebase.Extensions;

namespace Root
{
    public class FirebaseRemoteConfigModule : IDataService
    {
        const string PREFIX = "Remote";
        IJsonService m_JsonService;
        ISetUpSdkService m_FirebaseSetUp;
        IDataService m_LocalData;
        bool m_IsTest;

        Dictionary<string, RemoteConfigData> m_DictData = new Dictionary<string, RemoteConfigData>();
        Dictionary<string, ConfigValue> m_DictValue = new Dictionary<string, ConfigValue>();

        public FirebaseRemoteConfigModule(IJsonService jsonService, ISetUpSdkService firebaseSetUp, IDataService localData, bool isTest, RemoteConfigData[] data)
        {
            m_JsonService = jsonService;
            m_FirebaseSetUp = firebaseSetUp;
            m_LocalData = localData;
            m_IsTest = isTest;

            foreach (var d in data)
            {
                m_DictData[d.keyName] = d;
            }
        }

        public void Load(Action callback)
        {
            FirebaseRemoteConfig.FetchAsync(new System.TimeSpan(0)).ContinueWithOnMainThread(taskFetch =>
            {
                var info = FirebaseRemoteConfig.Info;
                switch (info.LastFetchStatus)
                {
                    case LastFetchStatus.Success:
                        FirebaseRemoteConfig.ActivateFetched();
                        Debug.Log(String.Format("Remote data loaded and ready (last fetch time {0}).", info.FetchTime));

                        foreach (var key in FirebaseRemoteConfig.Keys)
                        {
                            var value = FirebaseRemoteConfig.GetValue(key);
                            m_DictValue[key] = value;
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
                callback?.Invoke();
            });
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

                var config = m_DictData[key];
                var remoteKey = (Application.platform == RuntimePlatform.Android) ? m_DictData[key].remoteKeyAndroid : m_DictData[key].remoteKeyIOS;
                key = string.IsNullOrEmpty(remoteKey) ? key : remoteKey;
            }

            return m_DictValue.ContainsKey(key) ? m_DictValue[key].StringValue : string.Empty;
        }

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

        public void Save()
        {
            m_LocalData.Save();
        }

        public bool HasKey(string key)
        {
            return m_LocalData.HasKey(GetSaveKey(key)) || m_DictData.ContainsKey(key) || m_DictValue.ContainsKey(key);
        }
    }
}

#endif