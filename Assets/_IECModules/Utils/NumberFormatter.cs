using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using UnityEngine;
using Random = System.Random;

public static class NumberFormatter {
	public enum TimeSuffix {
		d,
		h,
		m,
		s
	}

	private static int timeSuffixCount;

	public static bool DebugNumberFormat;

	private static string[] _suffixes;

	private static int _maxNumber;

	private static Random rng = new Random();

	// public static void Shuffle<T>(this IList<T> list) {
	// 	int n = list.Count;
	// 	while (n > 1) {
	// 		n--;
	// 		int k = rng.Next(n + 1);
	// 		T value = list[k];
	// 		list[k] = list[n];
	// 		list[n] = value;
	// 	}
	// }

	static NumberFormatter() {
//		NumberFormatter.timeSuffixCount = 2;
		NumberFormatter._maxNumber = 82;
		NumberFormatter.InitSuffixes();
	}

	public static double Round(double d, int digits) {
		if (d <= 0.0) {
			return d;
		}

		double num = Math.Pow(10.0, Math.Floor(Math.Log10(Math.Abs(d))) + 1.0);
		return num * Math.Round(d / num, digits);
	}

	public static long SecondsToMinutes(this long seconds) {
		return seconds / 60L;
	}

	public static long SecondsToHours(this long seconds) {
		return seconds / 3600L;
	}

	public static string FormatPriceText(this int num) => (num - 0.01f).ToString(CultureInfo.InvariantCulture); 
	public static string MinifyFormat(this long num) => MinifyFormat((double) num);
	public static string MinifyFormat(this float num) => MinifyFormat((double) num);

	public static string FormatMoney(this long num)
	{
		var suffix = "";
		// if (num >= Mathf.Pow(10, 10)) return num / (long) Mathf.Pow(10, 9) + "B";
		if(num >= Mathf.Pow(10, 7)) return num / (long) Mathf.Pow(10, 6) + "M";
		// if(num >= Mathf.Pow(10, 4)) return num / (long) Mathf.Pow(10, 3) + "K";
		return num.ToString();
	}

	public static string MinifyFormat(this double num) {
		double num2 = NumberFormatter.Round(num, 3);
		if (NumberFormatter.DebugNumberFormat) {
			return num2.DebugNumberFormation();
		}

		return NumberFormatter.Minify(num2);
	}

	private static string DebugNumberFormation(this double num) {
		return num.ToString("E1");
	}

	public static string ToInvariantCultureString(this double num) {
		return num.ToString(CultureInfo.InvariantCulture);
	}

	private static string MinifySuperCashValue(this double num) {
		num = Math.Floor(num);
		if (num > 1000000.0) {
			return NumberFormatter.Minify(num);
		}

		if (num < 1000.0) {
			return num.ToString("##0", CultureInfo.InvariantCulture);
		}

		return num.ToString("# ##0", CultureInfo.InvariantCulture);
	}

	public static string MinifyIncomeFactor(this double num) {
		return num.ToString("0.#", CultureInfo.InvariantCulture);
	}

	public static string ConvertTimeToDayHoursMinuteSeconds(this TimeSpan time)
	{
		var returnStr = "";

		var dayString = time.Days + "d"; 
		var hourString = time.Hours + "h";
		var minuteString = time.Minutes + "m";
		var secondString = time.Seconds + "s";

		if (time.Days > 0) return $"{dayString } {hourString}";
		if (time.Hours > 0) return $"{hourString} {minuteString}";
		return $"{minuteString} {secondString}";

	}
	public static string ConvertTimeToHoursMinuteSeconds(this TimeSpan time) {
		return $"{(int) Math.Floor(time.TotalHours):D2}:{time.Minutes:D2}:{time.Seconds:D2}";
	}


	private static List<NumberFormatter.TimeSuffix> GetNeededSuffixesOrdered(TimeSpan timeSpan, int MaxSuffixCount, NumberFormatter.TimeSuffix MaxSuffix) {
		List<NumberFormatter.TimeSuffix> list = new List<NumberFormatter.TimeSuffix>();
		if ((timeSpan.Days > 0 || (MaxSuffix == NumberFormatter.TimeSuffix.d && (int) timeSpan.TotalDays > 0)) && list.Count < MaxSuffixCount && MaxSuffix <= NumberFormatter.TimeSuffix.d) {
			list.Add(NumberFormatter.TimeSuffix.d);
		}

		if ((timeSpan.Hours > 0 || (MaxSuffix == NumberFormatter.TimeSuffix.h && (int) timeSpan.TotalHours > 0)) && list.Count < MaxSuffixCount && MaxSuffix <= NumberFormatter.TimeSuffix.h) {
			list.Add(NumberFormatter.TimeSuffix.h);
		}

		if ((timeSpan.Minutes > 0 || (MaxSuffix == NumberFormatter.TimeSuffix.m && (int) timeSpan.TotalMinutes > 0)) && list.Count < MaxSuffixCount && MaxSuffix <= NumberFormatter.TimeSuffix.m) {
			list.Add(NumberFormatter.TimeSuffix.m);
		}

		if ((timeSpan.Seconds > 0 || (MaxSuffix == NumberFormatter.TimeSuffix.s && (int) timeSpan.TotalSeconds > 0)) && list.Count < MaxSuffixCount && MaxSuffix <= NumberFormatter.TimeSuffix.s) {
			list.Add(NumberFormatter.TimeSuffix.s);
		}

		if (timeSpan.Seconds == 0 && list.Count == 0) {
			list.Add(NumberFormatter.TimeSuffix.s);
		}

		return list;
	}

	private static void InitSuffixes() {
		string text = "abcdefghijklmnopqrstuvwxyz";
		NumberFormatter._suffixes = new string[NumberFormatter._maxNumber];
		NumberFormatter._suffixes[0] = "K";
		NumberFormatter._suffixes[1] = "M";
		NumberFormatter._suffixes[2] = "B";
		NumberFormatter._suffixes[3] = "T";
		for (int i = 0; i < text.Length; i++) {
			NumberFormatter._suffixes[4 + i] = "a" + text[i];
		}

		for (int j = 0; j < text.Length; j++) {
			NumberFormatter._suffixes[30 + j] = "b" + text[j];
		}

		for (int k = 0; k < text.Length; k++) {
			NumberFormatter._suffixes[56 + k] = "c" + text[k];
		}
	}

	private static string Minify(double num) {
		for (int i = NumberFormatter._maxNumber; i > 0; i--) {
			if (num >= Math.Pow(10.0, (double) (3 * i))) {
				return (num / Math.Pow(10.0, (double) (3 * i))).ToString("0.##", CultureInfo.InvariantCulture) + NumberFormatter._suffixes[i - 1];
			}
		}

		return num.ToString(CultureInfo.InvariantCulture);
	}

	public static string FormatTimeString(this double totalSeconds, bool getFull = false) {
		return FormatTimeString((long) totalSeconds, getFull);
	}

	public static string FormatTimeString(this long totalSeconds, bool getFull = false) {
		TimeSpan timeSpan = TimeSpan.FromSeconds(totalSeconds);
		int day = timeSpan.Days;
		int hour = timeSpan.Hours;
		int minute = timeSpan.Minutes;
		int second = timeSpan.Seconds;
		string result = "";
		int count = 0;
		int maxCount = 2;
		if (getFull) maxCount = 10;
		if (day > 0 && count < maxCount) {
			result += day + "d ";
			count++;
		}

		if (hour > 0 && count < maxCount) {
			result += hour + "h ";
			count++;
		}

		if (minute > 0 && count < maxCount) {
			result += minute + "m ";
			count++;
		}

		if (second > 0 && count < maxCount) {
			result += second + "s ";
			count++;
		}

		if (result.Length == 0) result = "0s";
		return result;
	}

	public static bool InRange(this int value, int minimum, int maximum) {
		return value >= minimum && value <= maximum;
	}

	public static string JsonSerialize(this object obj) {
		return JsonConvert.SerializeObject(obj);
	}
}
