using System;

namespace _IEC.Features.Common.Extension
{
    public static class StringExtensions
    {
        public static string AutoConvert(this TimeSpan timeSpan)
        {
            if (timeSpan.Days > 0)
            {
                return timeSpan.Days + "d" + timeSpan.Hours + "h";
            }else
            {
                if (timeSpan.Hours > 0)
                    return timeSpan.Hours + "h" + timeSpan.Minutes + "mn";
                else return timeSpan.Minutes + "mn" + timeSpan.Seconds + "s";
            }
        }
    }
}