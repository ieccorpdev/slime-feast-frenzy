using UnityEngine;

namespace IEC
{
	public class UnityJsonModule : IJsonService
	{

		public string ToJson (object obj) {
			return "";
		}

		public T FromJson<T> (string json)
		{
			return  default(T);
		}

	}
}

