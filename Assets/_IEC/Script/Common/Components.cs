using System;
using System.Collections;
using System.Collections.Generic;
using IEC.Component;

namespace IEC
{
    public static class Components
    {
        static List<IComponentMap> s_ListMap = new List<IComponentMap>();

        static Dictionary<Type, Object> m_Dict = new Dictionary<Type, object>();
        static Dictionary<Type, Object> m_DictMap = new Dictionary<Type, object>();

        static Components()
        {
        }

        public static T Get<T>() where T : class, new()
        {
            var type = typeof(T);
            if (!m_Dict.ContainsKey(type))
            {
                m_Dict[type] = new T();
            }
            return m_Dict[type] as T;
        }

        public static ComponentMap<T> GetMap<T>() where T : class, new()
        {
            var type = typeof(T);
            if (!m_DictMap.ContainsKey(type))
            {
                var map = new ComponentMap<T>();
                s_ListMap.Add(map);
                m_Dict[type] = map;
            }
            return m_Dict[type] as ComponentMap<T>;
        }

        static ComponentMap<T> NewMap<T>(ComponentMap<T> map) where T : class, new()
        {
            map = new ComponentMap<T>();
            s_ListMap.Add(map);
            return map;
        }

        public static void Remove(int objID)
        {
            for (int i = 0; i < s_ListMap.Count; i++)
            {
                s_ListMap[i].Remove(objID);
            }
        }

        public static void RemoveAll()
        {
            for (int i = 0; i < s_ListMap.Count; i++)
            {
                s_ListMap[i].RemoveAll();
            }
        }
    }
}