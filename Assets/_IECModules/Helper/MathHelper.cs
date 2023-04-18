using System.Collections.Generic;
using UnityEngine;

public class MathHelper
{
    public static float Lerp(float from, float to, float rel)
    {
        return ((1 - rel) * from) + (rel * to);
    }

    public static float InvLerp(float from, float to, float value)
    {
        return (value - from) / (to - from);
    }

    public static float Remap(float origFrom, float origTo, float targetFrom, float targetTo, float value)
    {
        float rel = InvLerp(origFrom, origTo, value);
        return Lerp(targetFrom, targetTo, rel);
    }

    public static int ClampInt(int value, int min, int max)
    {
        value = (value < min) ? min : value;
        value = (value > max) ? max : value;
        return value;
    }

    public static Vector3 DivideVector(Vector3 a, Vector3 b)
    {
        return new Vector3(a.x / b.x, a.y / b.y, a.z / b.z);
    }

    public static Vector3 MultiplyVector(Vector3 a, Vector3 b)
    {
        return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
    }

    public static void SetRandomSeed(int seed)
    {
        UnityEngine.Random.InitState(seed);
    }

    public static int GetRandomIndexByRatioList(int[] ratio, int itemLength)
    {
        if (ratio == null || ratio.Length == 0)
        {
            return UnityEngine.Random.Range(0, itemLength);
        }

        float sum = 0;
        for (int i = 0; i < ratio.Length; i++)
        {
            sum += ratio[i];
        }
        float randValue = UnityEngine.Random.Range(0, sum);
        sum = 0;
        for (int i = 0; i < ratio.Length; i++)
        {
            if (ratio[i] <= 0)
                continue;
            sum += ratio[i];
            if (randValue <= sum)
            {
                return i;
            }
        }

        return UnityEngine.Random.Range(0, itemLength);
    }

    public static int GetRandomIndexByRatioList(float[] ratio, int itemLength)
    {
        if (ratio == null || ratio.Length == 0)
        {
            return UnityEngine.Random.Range(0, itemLength);
        }

        float sum = 0;
        for (int i = 0; i < ratio.Length; i++)
        {
            sum += ratio[i];
        }
        float randValue = UnityEngine.Random.Range(0, sum);
        sum = 0;
        for (int i = 0; i < ratio.Length; i++)
        {
            if (ratio[i] <= 0)
                continue;
            sum += ratio[i];
            if (randValue <= sum)
            {
                return i;
            }
        }

        return UnityEngine.Random.Range(0, itemLength);
    }

    private static List<int> listTemp = new List<int>();
    public static int GetRandomIndexByList(List<int> listUsed, int itemCount)
    {
        listTemp.Clear();
        for (int i = 0; i < itemCount; i++)
        {
            if (!listUsed.Contains(i))
            {
                listTemp.Add(i);
            }
        }

        return listTemp[UnityEngine.Random.Range(0, listTemp.Count)];
    }

    public static int RoundCeilToInt(float f)
    {
        var frac = f - (int)f;
        if (frac == 0.5f)
        {
            frac = 1;
            return (int)f + (int)frac;
        }
        return Mathf.RoundToInt(f);
    }

    public static int RoundCeilToIntGreaterZero(float f)
    {
        var round = RoundCeilToInt(f);
        if (round == 0) return 1;
        return round;
    }
}