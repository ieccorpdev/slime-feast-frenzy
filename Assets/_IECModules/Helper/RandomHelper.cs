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

	// Returns a random integer number between min [inclusive] and max [exclusive] with seed
	public static int RangeWithSeed(int seed, int min, int max)
	{
		System.Random r = new System.Random(seed);
		return r.Next(min, max);
	}

	public static Vector3 RandomPointInBounds(Bounds bounds)
	{
		return new Vector3(
			Random.Range(bounds.min.x, bounds.max.x),
			Random.Range(bounds.min.y, bounds.max.y),
			Random.Range(bounds.min.z, bounds.max.z)
		);
	}

	public static Vector3 GetRandomPointInsideCollider(BoxCollider boxCollider)
	{
		Vector3 extents = boxCollider.size / 2f;
		Vector3 point = new Vector3(
			Random.Range(-extents.x, extents.x),
			Random.Range(-extents.y, extents.y),
			Random.Range(-extents.z, extents.z)
		) + boxCollider.center;
		return boxCollider.transform.TransformPoint(point);
	}
}