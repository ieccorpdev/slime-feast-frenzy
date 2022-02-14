
using System;
using UnityEngine;

namespace IEC
{
	public class SecurePlayerPrefsUtilModule : IDataService
	{
		IJsonService jsonService;

        public SecurePlayerPrefsUtilModule (IJsonService jsonService)
        {
            this.jsonService = jsonService;
        }

		public void SetInt (string key, int value) {
            PlayerPrefsUtility.SetEncryptedInt (key, value);
		}

		public void SetBool (string key, bool value) {
			int intValue = (value) ? 1 : 0;
			SetInt (key, intValue);
		}

		public void SetFloat (string key, float value) {
            PlayerPrefsUtility.SetEncryptedFloat (key, value);
        }

		public void SetString (string key, string value) {
            PlayerPrefsUtility.SetEncryptedString (key, value);
        }

		public void SetObject (string key, object value) {
			var json = jsonService.ToJson (value);
			SetString (key, json);
		}

		public int GetInt (string key, int defaultValue) {
            return PlayerPrefsUtility.GetEncryptedInt (key, defaultValue);
		}

		public bool GetBool (string key, bool defaultValue) {
			int intValue = GetInt (key, -1);
			if (intValue == -1) return defaultValue;
			return (intValue != 0);
		}

		public float GetFloat (string key, float defaultValue) {
            return PlayerPrefsUtility.GetEncryptedFloat (key, defaultValue);
		}

		public string GetString (string key, string defaultValue) {
            return PlayerPrefsUtility.GetEncryptedString (key, defaultValue);
        }

		public T GetObject<T> (string key, T defaultValue) {
			var json = GetString (key, string.Empty);
			var obj = jsonService.FromJson<T> (json);
			obj = (obj == null) ? defaultValue : obj;
			return obj;
		}

		public void Save () {
            PlayerPrefs.Save();
        }

        public void Load(Action callback)
        {
        }

        public bool HasKey(string key)
        {
            return PlayerPrefsUtility.IsHasEncryptedKey(key);
        }
    }
}

