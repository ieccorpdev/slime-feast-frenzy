
namespace Root
{
	public class TimeModule : ITimeService
	{
		long totalSeconds;

		int hours;
		int minutes;
		int seconds;

		string format = "00";
		string split = ":";
		string split2 = " : ";

		public void SetTotalSeconds (int seconds) {
			totalSeconds = seconds;

			this.seconds = seconds % 60;
			this.minutes = (seconds % 3600) / 60;
			this.hours = seconds / 3600;
		}

		public int GetSeconds () {
			return seconds;
		}

		public int GetMinutes () {
			return minutes;
		}

		public int GetHours () {
			return hours;
		}

		public string GetTimeString (bool withSpace = false) {
			if (withSpace) {
				return (hours.ToString (format)
				+ split2 + minutes.ToString (format)
				+ split2 + seconds.ToString (format));
			} else {
				return (hours.ToString (format) 
					+ split + minutes.ToString (format) 
					+ split + seconds.ToString (format));
			}
		}

	}
}

