using System.Collections.Generic;
using UnityEngine;

public static class RandomUtils
{
    private static System.Random rng = new System.Random();  

    public static T SampleWithSeed<T>(this List<T> list, int seed)
    {
        rng = new System.Random(seed);
        return list[rng.Next(list.Count)];
    }
    public static T Sample<T>(this List<T> list)
    {
        return list[Random.Range(0, list.Count)];
    }
    public static void Shuffle<T>(this IList<T> list)  
    {  
        var n = list.Count;  
        while (n > 1) {  
            n--;  
            var k = rng.Next(n + 1);  
            var value = list[k];  
            list[k] = list[n];  
            list[n] = value;  
        }  
    }
}