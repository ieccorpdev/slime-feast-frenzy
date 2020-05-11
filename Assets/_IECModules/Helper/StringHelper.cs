using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StringHelper {

	static Dictionary<int, string> s_MapNumber = new Dictionary<int, string>();

	public static string GetNumber(int number)
	{
		if(!s_MapNumber.ContainsKey(number))
		{
			s_MapNumber[number] = number.ToString();
			
		}
		return s_MapNumber[number];
	}
}
