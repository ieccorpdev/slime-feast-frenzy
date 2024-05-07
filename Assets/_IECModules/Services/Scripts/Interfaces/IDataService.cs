
using System;

namespace IEC
{
	public enum DataType { Normal, Secure }

	public interface IDataService
	{

		void SetInt (string key, int value);

		void SetBool (string key, bool value);

		void SetFloat (string key, float value);

		void SetString (string key, string value);

		void SetObject (string key, object value);

		int GetInt (string key, int defaultValue);

		bool GetBool (string key, bool defaultValue);

		float GetFloat (string key, float defaultValue);

		string GetString (string key, string defaultValue);

		T GetObject<T> (string key, T defaultValue);

		void Save ();

        void Load(System.Action<bool> callback);

        bool HasKey(string key);
	}

}

