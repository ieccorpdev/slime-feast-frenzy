using nickeltin.SDF.Runtime;
using UnityEngine;
public class FallingItemDefinition : ScriptableObject
{
	[Header("Configuration")]
	[SerializeField]
	private string id;

	[SerializeField]
	private SpecialItemID itemId;

	[SerializeField]
	private Sprite icon;

	[SerializeField]
	private GameObject icon3DPrefab;
	[SerializeField]
	private SDFImage iconSDF;

	[SerializeField]
	private int level;

	[SerializeField]
	private int overrideLevel;

	[SerializeField]
	private int xp;

	[SerializeField]
	[Header("Settings")]
	private bool startAsKinematic;

	[SerializeField]
	private bool canBeGoal;

	[SerializeField]
	private bool ignoreInCalculations;

	[SerializeField]
	private GameSettings.FallingItemSetType setType;

	[SerializeField]
	private PhysicFallingItemType physicsType;
}
