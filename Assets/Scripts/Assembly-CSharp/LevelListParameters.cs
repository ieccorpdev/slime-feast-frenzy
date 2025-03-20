using UnityEngine;

[CreateAssetMenu(fileName = "LevelListParameters", menuName = "Scriptables/HungryHole/LevelListParameters")]
public class LevelListParameters : ScriptableObject
{
	public LevelDefinition[] Levels;
}
