using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC.Linq
{
    public static class LinqHelper
    {
        public static void Foreach<T>(this IList<T> items, System.Action<T> action)
        {
            for (int i = 0; i < items.Count; i++)
            {
                action(items[i]);
            }
        }

        public static List<T> Where<T>(this List<T> items, List<T> results, System.Func<T, bool> func)
        {
            if (results == null)
            {
                results = new List<T>();
            }
            results.Clear();
            for (int i = 0; i < items.Count; i++)
            {
                T item = items[i];
                if (func(item))
                {
                    results.Add(item);
                }
            }
            return results;
        }

        public static T Aggregate<T>(this IList<T> items, T seed, System.Func<T, T, T> accumulator)
        {
            T value = seed;
            for (int i = 0; i < items.Count; i++)
            {
                value = accumulator(value, items[i]);
            }
            return value;
        }

        public static bool All<T>(this IList<T> items, System.Func<T, bool> func)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (!func(items[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool Any<T>(this IList<T> items, System.Func<T, bool> func)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (func(items[i]))
                {
                    return true;
                }
            }
            return false;
        }

        public static int FindIndex<T>(this IList<T> items, System.Func<T, bool> func)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (func(items[i]))
                {
                    return i;
                }
            }
            return -1;
        }

        public static T Find<T>(this IList<T> items, System.Func<T, bool> func)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (func(items[i]))
                {
                    return items[i];
                }
            }
            return default(T);
        }

        public static float Min<T>(this IList<T> items, System.Func<T, float> func)
        {
            float min = func(items[0]);
            for (int i = 0; i < items.Count; i++)
            {
                if (func(items[i]) < min)
                {
                    min = func(items[i]);
                }
            }
            return min;
        }

        public static float Max<T>(this IList<T> items, System.Func<T, float> func)
        {
            float max = func(items[0]);
            for (int i = 0; i < items.Count; i++)
            {
                if (func(items[i]) > max)
                {
                    max = func(items[i]);
                }
            }
            return max;
        }

        public static int Min<T>(this IList<T> items, System.Func<T, int> func)
        {
            int min = func(items[0]);
            for (int i = 0; i < items.Count; i++)
            {
                if (func(items[i]) < min)
                {
                    min = func(items[i]);
                }
            }
            return min;
        }

        public static int Max<T>(this IList<T> items, System.Func<T, int> func)
        {
            int max = func(items[0]);
            for (int i = 0; i < items.Count; i++)
            {
                if (func(items[i]) > max)
                {
                    max = func(items[i]);
                }
            }
            return max;
        }

        public static List<T> OrderBy<T>(this List<T> items, Func<T, float> func)
        {
            int count = items.Count;
            for (int i = 0; i < count; i++)
            {
                float valueI = func(items[i]);
                for (int j = i + 1; j < count; j++)
                {
                    float valueJ = func(items[j]);
                    if (valueI > valueJ)
                    {
                        T t = items[i];
                        items[i] = items[j];
                        items[j] = t;
                        valueI = valueJ;
                    }
                }
            }
            return items;
        }

        public static float Sum<T>(this List<T> items, Func<T, float> func)
        {
            float sum = 0f;
            int count = items.Count;
            for (int i = 0; i < count; i++)
            {
                sum += func(items[i]);
            }
            return sum;
        }

        public static List<TSource> ToList<TSource>(this IEnumerable<TSource> source, List<TSource> results)
        {
            if (results == null)
            {
                results = new List<TSource>();
            }
            results.Clear();
            results.AddRange(source);
            return results;
        }

        public static T First<T>(this IList<T> items)
        {
            return items[0];
        }

        public static T Last<T>(this IList<T> items)
        {
            return items[items.Count - 1];
        }

        public static List<Result> Select<T, Result>(this List<T> items, List<Result> results, System.Func<T, Result> func)
        {
            if (results == null)
            {
                results = new List<Result>();
            }
            results.Clear();
            for (int i = 0; i < items.Count; i++)
            {
                results.Add(func(items[i]));
            }
            return results;
        }

        public static Dictionary<Key, T> ToDictionary<T, Key>(this List<T> items, Dictionary<Key, T> dictionary, System.Func<T, Key> keySelector)
        {
            if (dictionary == null)
            {
                dictionary = new Dictionary<Key, T>();
            }
            dictionary.Clear();
            for (int i = 0; i < items.Count; i++)
            {
                T item = items[i];
                dictionary.Add(keySelector(item), item);
            }
            return dictionary;
        }

    }
}