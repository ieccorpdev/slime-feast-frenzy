using System;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "GameSettings", menuName = "Scriptables/HungryHole/GameSettings")]
public class GameSettings : ScriptableObject
{
	public enum RewardType
	{
		UnlimitedLives = 0,
		UnlimitedBonus = 1,
		Bonus_Compass = 2,
		Bonus_Enlarge = 3,
		Bonus_Magnet = 4,
		Bonus_TimeFreeze = 5,
		Bonus_Chrono = 6,
		Bonus_Lightning = 7,
		Coin = 8,
		Key = 9,
		WeeklyItem = 10,
		SpecialItem_x2 = 11,
		UnlimitedBonusChrono = 12,
		UnlimitedBonusLightning = 13,
		FreeLife = 14,
		LifeRefill = 15
	}

	[Serializable]
	public class RewardInfos
	{
		public string Id;

		public RewardType Type;

		public Sprite Icon;

		
	}

	[Serializable]
	public class GameRewardInfos
	{
		public RewardType Type;

		public Sprite Icon;
	}

	[Serializable]
	public class RewardSettings
	{
		public RewardType Type;

		public int Quantity;
	}

	public class ServerRewardSettings
	{
		public string Type;

		public int Quantity;
	}

	[Serializable]
	public class WeeklyItemRewardSettings : RewardSettings
	{
		public int RequiredItems;
	}

	[Serializable]
	public class SeasonRewardSettings
	{
		public List<RewardSettings> Rewards;

		public int MinRank;

		public int MaxRank;
	}

	[Serializable]
	public class ExtraTimeSetting
	{
		public int Price;

		public int Time;

		public int AdditionalTime;

		public List<RewardSettings> Rewards;
	}

	[Serializable]
	public class ChestRewardsConfig
	{
		public List<ChestRewardWeightedCount> NbRewards;

		public int NbForcedReward;

		public List<ChestRewardsConfigForcedDraws> ForcedDraws;

		public List<ChestRewardConfigReward> ForcedRewards;

		public List<ChestRewardConfigReward> Rewards;
	}

	[Serializable]
	public class ChestRewardsConfigForcedDraws
	{
		public List<RewardSettings> Rewards;
	}

	[Serializable]
	public class ChestRewardWeightedCount
	{
		public int Weight;

		public int NbRewards;
	}

	[Serializable]
	public class ChestRewardConfigReward
	{
		public int Weight;

		public RewardType Type;

		public int Quantity;
	}

	public enum FallingItemSetType
	{
		ASIAN_FOOD = 0,
		CAKES = 1,
		FRUITS = 2,
		TOYS = 3,
		VEGETABLES = 4,
		SCHOOL_SUPPLIES = 5,
		DRINKS = 6,
		FASTFOOD = 7,
		COIN = 100,
		OTHER_ITEMS = 101,
		ALL = 102
	}

	

	[Header("Life")]
	public int MaxLives;

	public int SecondsPerLiveRegen;

	[Header("Levels")]
	public int TutorialLevelCount;

	public LevelListParameters Levels;

	public LevelListParameters EasyLevels;

	public int MaxLevelHomeSkip;

	[Header("Bonus")]
	public int BonusNewPlayerGiftCount;

	public int BonusPerPurchase;

	public int BonusChronoTimeHome;

	public int BonusChronoTimeInGame;

	public int BonusLightningXp;

	[Header("Malus")]
	public float BonusDwarfismScale;

	public float MalusGigantismDistanceScale;

	public float MalusGigantismScale;

	public float MalusGigantismDuration;

	public int MalusAntiChronoTimeInGame;

	[Header("Extra Time")]
	public List<ExtraTimeSetting> ExtraTimeSettings;

	[Header("Rewards")]
	public List<GameRewardInfos> GameRewardInfosData;

	public List<RewardInfos> RewardInfosData;

	[Header("Weekly Item")]
	public WeeklyItemRewardSettings[] WeeklyItemRewards;

	[Header("Star Chest")]
	public int StarsGoal;

	public float[] StarThresholds;

	public float[] StarThresholdsItems;

	public ChestRewardsConfig StarChestRewardSettings;

	[Header("Level Chest")]
	public int LevelGoal;

	public ChestRewardsConfig LevelChestRewardSettings;

	[Header("Teams")]
	public int MaxTeamSize;

	public int TeamLifeRequestCooldownInSeconds;

	[Header("Unlock Levels")]
	

	public int UnlockStartGamePopup;

	public int UnlockWeeklyItemLevel;

	public int UnlockInGameLightningSpecialLevel;

	public int UnlockInGameChronoSpecialLevel;

	public int UnlockCrownLevel;

	public int UnlockInGameCoinsLevel;

	[Space]
	public int MandatoryProfileOnboarding;



	[Header("Facebook")]
	public int FacebookGroupLikeAppearanceLevel;

	public string FacebookGroupLinkApp;

	public string FacebookGroupLinkWeb;

	public int FacebookCoinReward;

	public RewardInfos GetRewardWithId(string id)
	{
		return null;
	}

	public RewardInfos GetRewardWithType(RewardType type)
	{
		return null;
	}

	public Sprite GetRewardSprite(RewardType type)
	{
		return null;
	}

	public int BreakTimeCoinsFormula(int coins)
	{
		return 0;
	}
}
