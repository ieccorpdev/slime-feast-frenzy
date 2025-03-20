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
    }
}