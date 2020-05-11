using System.Collections.Generic;
using UnityEngine;

public class RandomHelper
{
	// Returns a random integer number between min [inclusive] and max [exclusive]
    public static int Range(int min, int max)
    {
		return Random.Range(min, max);
    }

	//Returns a random float number between and min [inclusive] and max [inclusive]
	public static float Range(float min, float max)
	{
		return Random.Range(min, max);
	}
}