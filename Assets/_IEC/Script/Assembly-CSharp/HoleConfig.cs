using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HoleConfig", menuName = "Scriptables/HungryHole/HoleConfig")]
public class HoleConfig : ScriptableObject
{
	[Header("Level")]
	public int[] RequiredXpPerLevel;

	public int[] RequiredXpPerLevelSlow;

	public int[] RequiredXpPerLevelCoins;

	public bool UseSlowXp;

	[Header("Hole")]
	public float BaseSpeed;

	public float SpeedIncreasePerLevel;

	public float BaseSize;

	public float SizeIncreasePerLevel;

	public int EnlargeAdditionalLevel;

	public List<float> EaseMechanicProgressiveCoef;

	[Header("Camera")]
	public float BaseCameraDistance;

	public float CameraDistanceIncreasePerLevel;
	public float magnetForce;

	public int GetRequiredXp(int level, bool isCoinLevel)
	{
		int index;
		var requiredXpPerLevel = UseSlowXp ? RequiredXpPerLevelSlow : RequiredXpPerLevel;
		if (level < requiredXpPerLevel.Length)
		{
			index = level - 1;
		}
		else
		{
			index = requiredXpPerLevel.Length - 1;
		}

		if (index == -1)
		{
			index = requiredXpPerLevel.Length - 2;
		}
		return requiredXpPerLevel[index];
	}
}
