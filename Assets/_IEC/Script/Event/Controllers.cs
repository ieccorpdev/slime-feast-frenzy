using System;
using System.Collections;
using System.Collections.Generic;

public static class Controllers {

	static Dictionary<Type, Object> m_Dict = new Dictionary<Type, object>();

	public static void Add<T>(T controller) where T : class
	{
		if(controller == null)
		{
			UnityEngine.Debug.LogError("add null controller");
		}
		m_Dict[typeof(T)] = controller;
	}

	public static T Get<T>() where T : class
	{
		var type = typeof(T);
		if(m_Dict.ContainsKey(type))
		{
			return (T)m_Dict[type];
		}
		return null;
	}
	
}
