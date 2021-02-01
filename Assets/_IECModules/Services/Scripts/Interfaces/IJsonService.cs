
namespace IEC
{
	public interface IJsonService
	{

		string ToJson (object obj);

		T FromJson<T> (string json);

	}
}

