using System;

namespace IEC
{
    public static class DateTimeHelper
    {
        public static DateTime TicksToDateTime(long ticks)
        {
            return new DateTime(ticks);
        }

        public static long TicksAddMinutes(long ticks, int minutes)
        {
            var time = new DateTime(ticks);
            return time.AddMinutes(minutes).Ticks;
        }

        public static TimeSpan SafeAdd(TimeSpan t1, TimeSpan t2)
        {
            try
            {
                return t1 + t2;
            }
            catch (OverflowException)
            {
                // If overflow occurs, return maximum possible value
                return (t1.Ticks > 0 && t2.Ticks > 0) ? TimeSpan.MaxValue : TimeSpan.MinValue;
            }
        }

        public static float TicksToSeconds(long ticks)
        {
            return (float)ticks / TimeSpan.TicksPerSecond;
        }

        /// <summary>
        /// {D}d {H}h {M}m
        /// </summary>
        /// <param name="timeSpan"></param>
        /// <returns></returns>
        public static string GetFormattedTimeDHM(TimeSpan timeSpan)
        {
            var day = timeSpan.Days;
            var hour = timeSpan.Hours;
            string days = day < 10 ? $"0{day}" : day.ToString();
            string hours = hour < 10 ? $"0{hour}" : hour.ToString();
            string minutes = timeSpan.Minutes < 10 ? $"0{timeSpan.Minutes}" : timeSpan.Minutes.ToString();
            return $"{days}d {hours}h {minutes}m";
        }

        public static string GetFormattedTimeHMS(TimeSpan timeSpan)
        {
            string hours = timeSpan.Hours < 10 ? $"0{timeSpan.Hours}" : timeSpan.Hours.ToString();
            string minutes = timeSpan.Minutes < 10 ? $"0{timeSpan.Minutes}" : timeSpan.Minutes.ToString();
            string seconds = timeSpan.Seconds < 10 ? $"0{timeSpan.Seconds}" : timeSpan.Seconds.ToString();
            return $"{hours}:{minutes}:{seconds}";
        }
    }
}