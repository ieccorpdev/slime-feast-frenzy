using UnityEngine;

namespace IEC
{
	public class UnityJsonModule : IJsonService
	{

		public string ToJson (object obj) {
			return JsonUtility.ToJson (obj);
		}

		public T FromJson<T> (string json) {
			return JsonUtility.FromJson<T> (json);
		}

	}
}

