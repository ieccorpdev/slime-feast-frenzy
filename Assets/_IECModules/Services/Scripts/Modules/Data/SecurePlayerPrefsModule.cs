
using System;

namespace IEC
{
	public class SecurePlayerPrefsModule : IDataService
	{
		IJsonService jsonService;

        public SecurePlayerPrefsModule (IJsonService jsonService)
        {
            this.jsonService = jsonService;
        }

		public void SetInt (string key, int value) {
			SecurityPlayerPrefs.SetInt (key, value);
		}

		public void SetBool (string key, bool value) {
			int intValue = (value) ? 1 : 0;
			SetInt (key, intValue);
		}

		public void SetFloat (string key, float value) {
			SecurityPlayerPrefs.SetFloat (key, value);
		}

		public void SetString (string key, string value) {
			SecurityPlayerPrefs.SetString (key, value);
		}

		public void SetObject (string key, object value) {
			var json = jsonService.ToJson (value);
			SetString (key, json);
		}

		public int GetInt (string key, int defaultValue) {
			return SecurityPlayerPrefs.GetInt (key, defaultValue);
		}

		public bool GetBool (string key, bool defaultValue) {
			int intValue = GetInt (key, -1);
			if (intValue == -1) return defaultValue;
			return (intValue != 0);
		}

		public float GetFloat (string key, float defaultValue) {
			return SecurityPlayerPrefs.GetFloat (key, defaultValue);
		}

		public string GetString (string key, string defaultValue) {
			return SecurityPlayerPrefs.GetString (key, defaultValue);
		}

		public T GetObject<T> (string key, T defaultValue) {
			var json = GetString (key, string.Empty);
			var obj = jsonService.FromJson<T> (json);
			obj = (obj == null) ? defaultValue : obj;
			return obj;
		}

		public void Save () {
			SecurityPlayerPrefs.Save ();
		}

        public void Load(Action callback)
        {
        }

        public bool HasKey(string key)
        {
            return SecurityPlayerPrefs.HasKey(key);
        }
    }
}

