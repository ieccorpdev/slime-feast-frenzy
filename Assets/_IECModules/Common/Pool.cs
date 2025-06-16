using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public static class Pool
    {
        static readonly Dictionary<Type, object> m_Dict = new Dictionary<Type, object>();
        static readonly Dictionary<Type, Func<object>> m_DictCreate = new Dictionary<Type, Func<object>>();

        static Pool()
        {
        }

        /// <summary>
        /// set single object to Get from type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        public static void Set<T>(T t) where T : class, new()
        {
            if (t == null)
            {
                Debug.LogError("Cannot set null object to Pool");
                return;
            }
            var type = typeof(T);
            m_Dict[type] = t;
        }

        public static void Set(Type type, object obj)
        {
            if (type == null)
            {
                Debug.LogError("Cannot set null object to Pool");
                return;
            }
            m_Dict[type] = obj;
        }

        /// <summary>
        /// get single object from type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Get<T>() where T : class, new()
        {
            var type = typeof(T);
            if (m_Dict.TryGetValue(type, out var result))
            {
                return result as T;
            }
            var t = new T();
            m_Dict[type] = t;
            return t;
        }
        
        /// <summary>
        /// return object from function SetCreate
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetCreate<T>() where T : class, new()
        {
            var type = typeof(T);
            if (m_DictCreate.TryGetValue(type, out var func))
            {
                return func.Invoke() as T;
            }
            return new T();
        }

        /// <summary>
        /// set function return what we get from GetCreate
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="functionCreate"></param>
        // public static void SetCreate<T>(Func<T> functionCreate) where T : class, new()
        // {
        //     var type = typeof(T);
        //     m_DictCreate[type] = functionCreate;
        // }

        public static void ClearAll()
        {
            m_Dict.Clear();
            m_DictCreate.Clear();
        }

        public static void ClearAllSingle()
        {
            m_Dict.Clear();
        }

        public static void ClearAllCreate()
        {
            m_DictCreate.Clear();
        }
    }
}