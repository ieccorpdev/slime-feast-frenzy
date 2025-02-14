using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace IEC
{
    /// <summary>
    /// object contain references to all other configs (hard fixed data)
    /// </summary>
    [CreateAssetMenu(menuName = "Game/GameConfig")]
    public class GameConfig : BaseConfig
    {
        public int maxHealth = 5;
        public int goldToRefill = 250;
        public _EmptyConfig emptyConfig;
        public HybridConfig hybridConfig;
        [Header(" Player Avatar Profile")]
        public PlayerAvatarConfig playerAvatarConfig;
        [Header("Lucky Wheel Config")]
        public DataRewardLuckyWheelConfig dataRewardLuckyWheelConfig;   
        [Header("PiggyBank Config")]
        public DataPiggyBank dataPiggyBank;   
        [Header("Daily Reward Config")]
        public DataDailyReward dataDailyRewardConfig;
        [Header("City Build")] 
        public BuildCityConfig buildCityConfig;
        [Header("LeaderBoard")]
        public List<RewardSprite> rewardSprites;
        public List<Sprite> goldSprites = new List<Sprite>();
        public Sprite GetSpriteByReward(ResourceType type)
        {
            return rewardSprites.Find(d => d.type == type).icon;
        }
        public Sprite GetGoldSprite(int gold)
        {
            if (gold >= 1500) return goldSprites[3];
            if (gold >= 1000) return goldSprites[2];
            if (gold >= 500) return goldSprites[1];
            return goldSprites[0];
        }
    }
    [System.Serializable]
    public class RewardSprite
    {
        public ResourceType type;
        public Sprite icon;

    }
}