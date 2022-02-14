
namespace IEC
{
	public class TimeModule : ITimeService
	{
		long totalSeconds;

        int days;
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
            this.days = hours / 24;
            hours -= days * 24;
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
            var s = (withSpace) ? split2 : split;
            if (days > 0)
            {
                return (days.ToString(format)
                    + s + hours.ToString(format)
                    + s + minutes.ToString(format)
                    + s + seconds.ToString(format));
            }
            else if (hours > 0)
            {
                return (hours.ToString(format)
                    + s + minutes.ToString(format)
                    + s + seconds.ToString(format));
            }
            else if (minutes > 0)
            {
                return (minutes.ToString(format)
                   + s + seconds.ToString(format));
            }
            return (seconds.ToString(format));
        }

        public string GetTimeStringMin3(bool withSpace = false)
        {
            var s = (withSpace) ? split2 : split;
            if (days > 0)
            {
                return (days.ToString(format)
                    + s + hours.ToString(format)
                    + s + minutes.ToString(format)
                    + s + seconds.ToString(format));
            }
            else
            {
                return (hours.ToString(format)
                    + s + minutes.ToString(format)
                    + s + seconds.ToString(format));
            }
        }
    }
}

