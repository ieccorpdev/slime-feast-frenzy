using System;

public class CompareVersion
{
    public static bool IsHigher(string version, string compareVersion)
    {
        return new Version(version).CompareTo(new Version(compareVersion)) > 0;
    }

    public static bool IsLower(string version, string compareVersion)
    {
        return new Version(version).CompareTo(new Version(compareVersion)) < 0;
    }

    public static bool IsEqual(string version, string compareVersion)
    {
        return new Version(version).CompareTo(new Version(compareVersion)) == 0;
    }

    public static bool IsHigherOrEqual(string version, string compareVersion)
    {
        return new Version(version).CompareTo(new Version(compareVersion)) >= 0;
    }
    
    public static bool IsLowerOrEqual(string version, string compareVersion)
    {
        return new Version(version).CompareTo(new Version(compareVersion)) <= 0;
    }
}