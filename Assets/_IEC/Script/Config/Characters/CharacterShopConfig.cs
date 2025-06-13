using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace IEC
{
    [CreateAssetMenu(menuName = "Game/CharacterShopConfig")]
    public class CharacterShopConfig : ScriptableObject
    {
        public int levelUnlock = 10;
        public List<CharacterShardSlotData> listShardSlotData = new();
        public List<CharacterShardRarity> listShardRarity = new();
        public List<int> shopRefreshPrice = new();
        public float shopRefreshTimeInHours;
        public float countdownBetweenRefresh = 10f;

        public CharacterConfig GetRandomCharacterShard(CharacterShardSlotData slotData)
        {
            var rarity = slotData.GetRandomRarityChance();

            var listCharacter = listShardRarity.Find(r => r.rarity == rarity).listCharacterConfig;

            return listCharacter[Random.Range(0, listCharacter.Count)];
        }

        public int GetShopRefreshPrice(int refreshTime)
        {
            refreshTime = Mathf.Clamp(refreshTime, 0, shopRefreshPrice.Count - 1);
            
            if (shopRefreshPrice.Count == 0)
                return 0;

            return shopRefreshPrice[refreshTime];
        }
        
    }

    [System.Serializable]
    public class CharacterShardSlotData
    {
        public int levelUnlockSlot;
        public int shardCount;
        public int goldCost;
        public List<CharacterRarityRandomChance> rarityRandomChances = new();

        public ShardRarity GetRandomRarityChance()
        {
            int totalWeight = rarityRandomChances.Sum(rarity => rarity.randomChance);
            
            int randomValue = Random.Range(0, totalWeight);
            
            int cumulative = 0;
            for (int i = 0; i < rarityRandomChances.Count; i++)
            {
                cumulative += rarityRandomChances[i].randomChance;
                if (randomValue < cumulative)
                {
                    return rarityRandomChances[i].rarity;
                }
            }
            
            return ShardRarity.I;
        }
    }

    [System.Serializable]
    public class CharacterShardRarity
    {
        public ShardRarity rarity;
        public List<CharacterConfig> listCharacterConfig = new();
    }

    [System.Serializable]
    public class CharacterRarityRandomChance
    {
        public ShardRarity rarity;
        public int randomChance;
    }

    public enum ShardRarity
    {
        I,
        II,
        III,
    }
}
