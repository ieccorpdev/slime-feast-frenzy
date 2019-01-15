using System.Collections;
using System.Collections.Generic;
using IEC.Component;

namespace IEC
{
    public static class Components
    {
        static List<IComponentMap> s_ListMap;
        public static readonly ComponentMap<Empty> TestMapEmpty;
        public static readonly Empty TestStaticEmpty;


        static Components()
        {
            s_ListMap = new List<IComponentMap>();

            TestMapEmpty = NewMap(TestMapEmpty);

            TestStaticEmpty = new Empty();
        }

        static ComponentMap<T> NewMap<T>(ComponentMap<T> map) where T : new()
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
    }
}