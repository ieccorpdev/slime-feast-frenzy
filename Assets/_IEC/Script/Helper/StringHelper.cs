using System;

namespace _IEC.Features.Common.Helper
{
    public static class StringHelper
    {
        public static string ConvertTimeInSecondToString(float seconds)
        {
            var time = TimeSpan.FromSeconds(seconds);
            if (time.Hours <= 0)
            {
                return time.ToString(@"mm\:ss");
            }
            if (time.Days <= 0)
            {
                return time.ToString(@"hh\:mm");
            }

            return time.ToString(@"dd\:hh");
        }
        
        public static string ConvertTimeInSecondToStringNoLeadingZero(float seconds)
        {
            var time = TimeSpan.FromSeconds(seconds);

            if (time.TotalHours < 1)
            {
                return $"{(int)time.Minutes}:{time.Seconds:D2}";
            }
            if (time.TotalDays < 1)
            {
                return $"{(int)time.Hours}:{time.Minutes:D2}";
            }

            return $"{(int)time.Days}:{time.Hours:D2}";
        }
    }
}