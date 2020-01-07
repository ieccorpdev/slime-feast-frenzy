using System.Collections;
using System.Collections.Generic;

namespace IEC
{
    public class ComponentPool<TComponent> where TComponent : new()
    {
		private Stack<TComponent> m_Pool;

		public ComponentPool()
		{
			m_Pool = new Stack<TComponent>();
		}

		public TComponent Get()
		{
			if(m_Pool.Count == 0)
			{
				return new TComponent();
			}
			else
			{
				return m_Pool.Pop();
			}
		}

		public void Free(TComponent component)
		{
			m_Pool.Push(component);
		}
    }
}