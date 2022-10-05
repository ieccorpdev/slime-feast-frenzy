using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class RandomUtils
{
    private static System.Random rng = new System.Random();

    public static T SampleWithSeed<T>(this List<T> list, int seed)
    {
        rng = new System.Random(seed);
        return list[rng.Next(list.Count)];
    }

    public static int WeightRandom(List<float> listWeight)
    {
        var random = Random.Range(0, listWeight.Sum());
        var sum = 0f;
        for (var i = 0; i < listWeight.Count; i++)
        {
            sum += listWeight[i];
            if (random < sum) return i;
        }
        return 0;
    }

    public static T Sample<T>(this List<T> list)
    {
        return list[Random.Range(0, list.Count)];
    }

    public static void Shuffle<T>(this IList<T> list)
    {
        var n = list.Count;
        while (n > 1)
        {
            n--;
            var k = rng.Next(n + 1);
            var value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}

namespace IEC
{
    public class RandomPro
    {
        private System.Random m_RNG;
        private List<int> m_ListIndex = new List<int>();

        public RandomPro()
        {
            m_RNG = new System.Random();
        }

        public RandomPro(int seed)
        {
            m_RNG = new System.Random(seed);
        }

        public bool BinaryChoice()
        {
            return RangeFromTo(0, 1) == 1;
        }

        public int RangeFromTo(int min, int max)
        {
            return Range(min, max + 1);
        }

        public int Range(int min, int max)
        {
            return m_RNG.Next(min, max);
        }

        public float Range(float min, float max)
        {
            float rand = (float) m_RNG.NextDouble();
            float range = max - min;
            return min + range * rand;
        }

        public T Sample<T>(List<T> list)
        {
            return list[Range(0, list.Count)];
        }

        public void Shuffle<T>(IList<T> list)
        {
            var n = list.Count;
            while (n > 1)
            {
                n--;
                var k = m_RNG.Next(n + 1);
                var value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public Vector3 InsideUnitSphere()
        {
            var rs = new Vector3();
            rs.x = Range(-1f, 1f);
            rs.y = Range(-1f, 1f);
            rs.z = Range(-1f, 1f);
            return rs;
        }

        public int WeightRandom(List<float> listWeight)
        {
            var random = Range(0, listWeight.Sum());
            var sum = 0f;
            for (var i = 0; i < listWeight.Count; i++)
            {
                sum += listWeight[i];
                if (random < sum) return i;
            }
            return 0;
        }

        public KeyValuePair<T, float> WeightRandom<T>(Dictionary<T, float> dictWeight)
        {
            var listWeight = dictWeight.Values.ToList();
            var index = WeightRandom(listWeight);
            return dictWeight.ElementAt(index);
        }

        public T SampleWhere<T>(IList<T> list, System.Func<T, bool> filter)
        {
            m_ListIndex.Clear();
            var count = list.Count;
            for (int i = 0; i < count; i++)
            {
                m_ListIndex.Add(i);
            }
            Shuffle(m_ListIndex);
            for (int i = 0; i < count; i++)
            {
                var index = m_ListIndex[i];
                if (filter(list[index]))
                {
                    return list[index];
                }
            }
            return default(T);
        }
    }
}
