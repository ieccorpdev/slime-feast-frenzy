
namespace Root
{
	public interface ITimeService
	{

		void SetTotalSeconds (int seconds);

		int GetSeconds ();

		int GetMinutes ();

		int GetHours ();

		string GetTimeString (bool withSpace);

	}
}

