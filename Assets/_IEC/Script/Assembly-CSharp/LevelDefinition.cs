using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "LevelDefinition", menuName = "Scriptables/HungryHole/LevelDefinition")]
public class LevelDefinition : ScriptableObject
{
	[Serializable]
	public class ItemData
	{
		public string Id;

		public int Level;

		public int Count;
	}

	[Serializable]
	public class GroundConfig
	{
		public string GroundJson;

		public string GroundEditorJson;

		public string GroundSkin;
	}

	[Header("Main Settings")]
	public int Id;

	public string Name;

	public string Guid;

	public int EditorVersion;

	public int mapSize;

	public Vector2 mapSize2D;

	public bool mapCentered;

	public Vector3 mapCenter;

	public List<GameSettings.FallingItemSetType> Sets;

	public List<ItemData> ItemList;

	[FormerlySerializedAs("RecipeList")]
	[SerializeField]
	private List<RecipeItem> _recipeList;

	[FormerlySerializedAs("DurationInSeconds")]
	[SerializeField]
	private int _durationInSeconds;

	public int DurationInSecondsSave;

	public string GridUsed;

	public bool ForceMalusNb;

	public Vector2Int ForceMalusNbValue;

	public bool ForceMalusRepartition;

	public int BombWeight;

	public int RadioactiveWeight;

	public int NegativeTimerWeight;

	[Obsolete("Use LevelJsonFile instead")]
	[Space]
	public string Json;

	
	public GroundConfig Ground;

	[Obsolete("Use GroundConfig.GroundJson instead")]
	public string GroundJson;

	[Obsolete("Use GroundConfig.GroundEditorJson instead")]
	public string GroundEditorJson;

	public string GroundSkin;

	public bool ManuallyAssignedGroundSkin;

	public List<PhysicFallingItemType> IncludedPhysicsElementType;

	[Space]
	public const string NORMAL_TYPE = "normal";

	public const string NORMALPLUS_TYPE = "normalplus";

	public const string HARD_TYPE = "hard";

	public const string EXTRAHARD_TYPE = "extrahard";

	public const string BREAKTIME_TYPE = "breaktime";

	public string Type;

	[Space]
	public int FailNumberForActivation;

	public string EasyVersionGuid;

	public string EasyVersionName;

	[Space]
	public long Seed;

	[Header("AI Calibration")]
	public int Difficulty;

	public int MinDuration;

	public int Distance;

	public int Xp;

	public int LevelMaxRecipe;

	public bool HandCrafted;

	public int AITimer;

	public int HumanTimer;

	public bool Secure;

	public int GetDurationInSeconds()
	{
		return _durationInSeconds;
	}

	public List<RecipeItem> GetListRecipeItem()
	{
		return _recipeList;
	}
	public bool IsNormal
	{
		get
		{
			return false;
		}
		set
		{
		}
	}

	public bool IsHard
	{
		get
		{
			return false;
		}
		set
		{
		}
	}

	public bool IsXtraHard
	{
		get
		{
			return false;
		}
		set
		{
		}
	}

	public bool IsBreakTime
	{
		get
		{
			return false;
		}
		set
		{
		}
	}

	public bool IsNormalPlus
	{
		get
		{
			return false;
		}
		set
		{
		}
	}

	public bool IsComposite => false;
	
}
