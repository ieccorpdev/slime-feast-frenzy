using System.Collections.Generic;
using System.Linq;
using _IEC.Features.Booster;
using _IEC.Features.Common.Config;
using _IEC.Features.Gameplay;
using UnityEngine;

namespace IEC
{
    /// <summary>
    /// object contain references to all other configs (hard fixed data)
    /// </summary>
    [CreateAssetMenu(menuName = "Game/GameConfig")]
    public class GameConfig : ScriptableObject
    {
        [Space(10)]
        public int maxHealth = 5;
        public int levelUnlockLife = 10;
        public int timeRefillLife = 15;//By Minutes
        public int goldToRefill = 250;
        public int goldToAddOneLife = 100;
        public int goldLevelWin = 10;
        public int levelWinBackHome = 10;
        
        [Header("SwallowHole")]
        public GamePlayConfig gamePlayConfig;
        public GameplayItemPrefabConfig itemPrefabConfig;
        public LevelConfig levelConfig;
        public ThemeConfig themeConfig;
        public PrefabConfig prefabConfig;
        //public HomeConfig homeConfig;
        public MapUnlockConfig mapUnlockConfig;
        //public SoundConfig soundConfig;
        //public BoosterConfig boosterConfig;
        //public MagicPouchConfig magicPouchConfig;
        //public RatingConfig ratingConfig;
        //public DailyRewardConfig dailyRewardConfig;
        //public TreasureHuntProgressConfig treasureHuntProgressConfig;
        //public StarProgressRewardConfig starProgressRewardConfig;
        //public LevelProgressRewardConfig levelProgressRewardConfig;
        public GameplayDebugConfig gameplayDebugConfig;
        //public PiggyBankConfig piggyBankConfig;
        //public ProfileConfig profileConfig;
        public Material waveWaterMaterial;
        public HybridConfig hybridConfig;

        [Header("Special Object Spawn Config")]
        public SpecialObjectConfig specialObjectConfig;
        [Header(" Player Avatar Profile")]
        public PlayerAvatarConfig playerAvatarConfig;
        [Header("Lucky Wheel Config")]
        public DataRewardLuckyWheelConfig dataRewardLuckyWheelConfig;   
        [Header("PiggyBank Config")]
        public DataPiggyBankConfig dataPiggyBankConfig;   
        [Header("Daily Reward Config")]
        public DataDailyReward dataDailyRewardConfig;
        [Header("Progress Reward Config")]
        public ProgressRewardConfig progressRewardConfig;
        [Header("City Build")] 
        public int brickWinReward = 15;
        public BuildCityConfig buildCityConfig;
        [Header("LeaderBoard")]
        public List<RewardSprite> defaultRewardSprites;
        public List<Sprite> goldSprites = new List<Sprite>();
        [Header("IapConfig")] 
        public List<int> levelBeginnerPackWin;
        public List<int> levelNoAdsPackWin;
        public List<PackConfig> listAllIapPackConfig;
        [Header("Level Data Desgin")] 
        public bool usingLevelGDV1;
        [Header("Hard Level")]
        public int levelStartHardLevel = 76;
        public int loseBeforeAllowSkipHardLevel = 1;
        public int stepBetweenHardLevels = 10;
        public float amountGoldRewardMultiply = 2f;
        [Header("Popup Add Helper")] 
        public PopupAddHelperConfig popupAddHelperConfig;
        [Space(10)]
        public EffectRewardClaimConfig dataEffectRewardClaimConfig;

        [Header("Chest")] 
        public AllChestConfig allChestConfig;

        public Sprite GetGoldSprite(int gold)
        {
            if (gold >= 150) return goldSprites[3];
            if (gold >= 100) return goldSprites[2];
            if (gold >= 50) return goldSprites[1];
            return goldSprites[0];
        }
        public Sprite GetSpriteByReward(ResourceType type)
        {
            return defaultRewardSprites.Find(d => d.type == type)?.icon;
        }
        public Sprite GetRewardSprite(Reward rewardData)
        {
            switch (rewardData.typeReward)
            {
                case ResourceType.Gold:
                   return GetGoldSprite(rewardData.rewardAmount);
                default:
                    return GetSpriteByReward(rewardData.typeReward);
            }
        }

        public PackConfig GetConfigPackIap(string id)
        {
            return listAllIapPackConfig.FirstOrDefault(x => x.bundleId == id);
        }
        [System.Serializable]
        public class RewardSprite
        {
            public ResourceType type;
            public Sprite icon;

        }
    }
}