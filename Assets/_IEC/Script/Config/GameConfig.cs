using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace IEC
{
    /// <summary>
    /// object contain references to all other configs (hard fixed data)
    /// </summary>
    [CreateAssetMenu(menuName = "Game/GameConfig")]
    public class GameConfig : BaseConfig
    {
        [Header("Game level data")] 
        public string baseLevelAssetFile;
        public string customLevelAssetFile;
        public string hardLevelAssetFile;
        public List<string> listLevelLoopOverride;
        
        [Space(10)]
        public int maxHealth = 5;
        public int levelUnlockLife = 10;
        public int timeRefillLife = 15;//By Minutes
        public int goldToRefill = 250;
        public int goldToAddOneLife = 100;
        public int goldLevelWin = 10;
        public int levelWinBackHome = 10;
        public int levelShowInter = 4;
        public _EmptyConfig emptyConfig;
        public HelperConfigData helperConfig;
        
        public HybridConfig hybridConfig;
        [Header(" Player Avatar Profile")]
        public PlayerAvatarConfig playerAvatarConfig;
        [Header("Lucky Wheel Config")]
        public DataRewardLuckyWheelConfig dataRewardLuckyWheelConfig;   
        [Header("PiggyBank Config")]
        public DataPiggyBankConfig dataPiggyBankConfig;   
        [Header("Daily Reward Config")]
        public DataDailyReward dataDailyRewardConfig;

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
        public string GetLevelDataAssetBundle(bool custom)
        {
            if (usingLevelGDV1 && custom)
            {
                return customLevelAssetFile;
            }

            return baseLevelAssetFile;
        }

        public List<string> GetListLevelLoop()
        {
            return listLevelLoopOverride;
        }

        public string GetLevelHardDataAssetBundle()
        {
            if (Pool.Get<Game>().isUsingHardLevel)
            {
                return hardLevelAssetFile;
            }
            return baseLevelAssetFile;
        }
        
    }
    [System.Serializable]
    public class RewardSprite
    {
        public ResourceType type;
        public Sprite icon;

    }
}