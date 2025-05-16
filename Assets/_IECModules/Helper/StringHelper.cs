using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class StringHelper
{

    static Dictionary<int, string> s_MapNumber = new Dictionary<int, string>();

    public static string GetNumber(int number)
    {
        if (!s_MapNumber.ContainsKey(number))
        {
            s_MapNumber[number] = number.ToString();

        }
        return s_MapNumber[number];
    }


    public static string RemoveBetweenParenthesis(this string original)
    {
        return RemoveBetweenString(original, "(", ")");
    }
    public static string RemoveBetweenString(this string original, string start, string end)
    {
        var pFrom = original.IndexOf(start, StringComparison.Ordinal) + start.Length;
        var pTo = original.IndexOf(end, pFrom, StringComparison.Ordinal);

        if (pFrom == -1 || pTo == -1 || pFrom >= pTo) return original;
        return original.Remove(pFrom - start.Length, pTo - pFrom + start.Length + end.Length);
    }

    public static string GetBetweenString(this string original, string start, string end,
        bool includeStartEndString = true)
    {
        var pFrom = original.IndexOf(start, StringComparison.Ordinal);
        var pTo = original.IndexOf(end, pFrom + start.Length, StringComparison.Ordinal);
        if (end == string.Empty || end == "") pTo = original.Length;

        if (pFrom == -1 || pTo == -1) return "";
        return includeStartEndString
            ? original.Substring(pFrom, pTo - pFrom + end.Length)
            : original.Substring(pFrom + start.Length, pTo - pFrom - start.Length);
    }

    public static string TrimStringToMaxBytes(string input, int maxBytes)
    {
        var totalBytes = Encoding.UTF8.GetByteCount(input);
        if (totalBytes <= maxBytes)
        {
            return input;
        }

        var low = 0;
        var high = input.Length;
        var bestIndex = input.Length;

        while (low <= high)
        {
            var mid = (low + high) / 2;
            var substring = input.Substring(mid);
            var byteCount = Encoding.UTF8.GetByteCount(substring);

            if (byteCount <= maxBytes)
            {
                bestIndex = mid;
                high = mid - 1;
            }
            else
            {
                low = mid + 1;
            }
        }
        return input.Substring(bestIndex);
    }

    public static string GetFileSizeStr(int byteCount)
    {
        return byteCount switch
        {
            < 1024 => byteCount + " B",
            < 1024 * 1024 => (1f * byteCount / 1024).ToString("0.0") + " KB",
            _ => (1f * byteCount / 1024 / 1024).ToString("0.0") + " MB"
        };
    }
}