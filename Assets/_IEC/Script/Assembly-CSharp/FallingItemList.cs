using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;

[CreateAssetMenu(fileName = "FallingItemList", menuName = "Scriptables/HungryHole/FallingItemList")]
public class FallingItemList : ScriptableObject
{
	
	[SerializeField]
	private List<FallingItemDefinition> itemDefinitions;

	private Dictionary<string, FallingItemDefinition> _fallingItemDefinitionsMap;

	public IReadOnlyList<FallingItemDefinition> ItemDefinitions => itemDefinitions;

	public int ItemsCount => 0;

	private void OnEnable()
	{
	}

	private void MapItemDefinitions()
	{
	}

	[PublicAPI]
	public FallingItemDefinition GetDefinitionAt(int index)
	{
		return null;
	}

	[PublicAPI]
	public FallingItemDefinition GetDefinitionWithID(string id)
	{
		return null;
	}
	
}
