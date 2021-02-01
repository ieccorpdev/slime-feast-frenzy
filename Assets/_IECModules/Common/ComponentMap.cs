using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public class ComponentMap<TComponent> : IComponentMap where TComponent : class, new()
    {
		private Dictionary<int, TComponent> m_Map = new Dictionary<int, TComponent>();

		private int[] m_ArrayObjID;

		private List<int> m_ListObjID;

		private bool m_IsArrayChanged = true;
		private bool m_IsListChanged = true;

		private ComponentPool<TComponent> m_Pool = new ComponentPool<TComponent>();

		public TComponent AddNew(int objID)
		{
			var comp = m_Pool.Get();
			Add(objID, comp);
			return comp;
		}

		public int[] GetArrayObjID()
		{
			if(m_IsArrayChanged)
			{
				m_ArrayObjID = new int[m_Map.Count];
				int i = 0;
				foreach(var member in m_Map)
				{
					m_ArrayObjID[i] = member.Key;
					i++;
				}
				m_IsArrayChanged = false;
			}
			return m_ArrayObjID;
		}

		public List<int> GetListObjID()
		{
			if(m_IsListChanged)
			{
				if(m_ListObjID == null)
				{
					m_ListObjID = new List<int>();
				}
				m_ListObjID.Clear();

				int i = 0;
				foreach(var member in m_Map)
				{
					m_ListObjID.Add(member.Key);
					i++;
				}
				m_IsListChanged = false;
			}
			return m_ListObjID;
		}

		public bool IsContain(int objID)
		{
			return m_Map.ContainsKey(objID);
		}

		public void Add(int objID, TComponent component)
		{
			m_Map.Add(objID, component);
			m_IsArrayChanged = true;
			m_IsListChanged = true;
		}

		public TComponent Get(int objID)
		{
			return m_Map[objID];
		}

		public void Remove(int objID)
		{
			if(IsContain(objID))
			{
				m_Pool.Free(m_Map[objID]);
				m_Map.Remove(objID);
				m_IsArrayChanged = true;
				m_IsListChanged = true;
			}
		}

        public void RemoveAll()
        {
            var list = GetListObjID();
            int count = list.Count;
            for(int i = 0; i < count; i++)
            {
                int objID = list[i];
                m_Pool.Free(m_Map[objID]);
            }

            m_Map.Clear();
            m_IsArrayChanged = true;
            m_IsListChanged = true;
        }

    }
}