using System;
using UnityEngine;

namespace IEC
{
	public class PlayerPrefsModule : IDataService
	{
		IJsonService jsonService;

        public PlayerPrefsModule(IJsonService jsonService)
        {
            this.jsonService = jsonService;
        }

		public void SetInt (string key, int value) {
			PlayerPrefs.SetInt (key, value);
		}

		public void SetBool (string key, bool value) {
			int intValue = (value) ? 1 : 0;
			SetInt (key, intValue);
		}

		public void SetFloat (string key, float value) {
			PlayerPrefs.SetFloat (key, value);
		}

		public void SetString (string key, string value) {
			PlayerPrefs.SetString (key, value);
		}

		public void SetObject (string key, object value) {
			var json = jsonService.ToJson (value);
			SetString (key, json);
		}

		public int GetInt (string key, int defaultValue) {
			return PlayerPrefs.GetInt (key, defaultValue);
		}

		public bool GetBool (string key, bool defaultValue) {
			int intValue = GetInt (key, -1);
			if (intValue == -1) return defaultValue;
			return (intValue != 0);
		}

		public float GetFloat (string key, float defaultValue) {
			return PlayerPrefs.GetFloat (key, defaultValue);
		}

		public string GetString (string key, string defaultValue) {
			return PlayerPrefs.GetString (key, defaultValue);
		}

		public T GetObject<T> (string key, T defaultValue) {
			var json = GetString (key, string.Empty);

            if (string.IsNullOrEmpty(json)) return defaultValue;

			var obj = jsonService.FromJson<T> (json);
			obj = (obj == null) ? defaultValue : obj;
			return obj;
		}

		public void Save () {
			PlayerPrefs.Save ();
		}

        public void Load(Action callback)
        {
        }

        public bool HasKey(string key)
        {
            return PlayerPrefs.HasKey(key);
        }
    }
}

