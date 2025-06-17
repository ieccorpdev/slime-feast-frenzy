using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _IEC.Features.Gameplay;
using UnityEngine;

namespace IEC
{
    [CreateAssetMenu(menuName = "Game/CharacterShardObjectConfig")]
    public class CharacterShardObjectConfig : ScriptableObject
    {
        public List<CharacterConfig> listCharacterConfigForShard = new();
        public List<ShardObjectSpawnData> listShardObjectSpawnData = new();
        public ItemDefinition shardObjectItemDefinition;
        public int GetSpawnAmount(int level)
        {
            if (listCharacterConfigForShard.Count == 0)
                return 0;
            
            var spawnData = listShardObjectSpawnData
                .OrderBy(data => data.levelStartSpawn)
                .LastOrDefault(data => data.levelStartSpawn <= level);
            return 1;
            return GetRandomWithZeroBias(spawnData?.dropChance ?? 0);
        }

        public CharacterConfig GetRandomCharacterConfigForShard()
        {
            if (listCharacterConfigForShard.Count == 0)
                return null;
            return listCharacterConfigForShard[Random.Range(0, listCharacterConfigForShard.Count)];
        }
        
        private int GetRandomWithZeroBias(float oneProbability)
        {
            if (Random.value <= oneProbability)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }

    [System.Serializable]
    public class ShardObjectSpawnData
    {
        public int levelStartSpawn;
        public float dropChance;
    }
}
