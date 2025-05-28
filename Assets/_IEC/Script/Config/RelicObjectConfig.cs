using System.Collections.Generic;
using _IEC.Features.Gameplay;
using UnityEngine;using System.Linq;

namespace IEC
{
    [CreateAssetMenu(menuName = "Game/RelicObjectConfig")]
    public class RelicObjectConfig : ScriptableObject
    {
        public float maxSpawnAmount = 6;
        public float minSpawnAmount = 3;
        public float maxSpawnSize = 6;
        public float minSpawnSize = 3;
        public List<RelicObjectSpawnData> listSpecialObjectSpawnData = new();
        public List<ItemDefinition> listRelicItemDefinition = new();
        
        public int GetSpawnAmount(int level)
        {
            var spawnData = listSpecialObjectSpawnData
                .OrderBy(data => data.levelStartSpawn)
                .LastOrDefault(data => data.levelStartSpawn <= level);

            return spawnData?.GetSpawnAmount() ?? 0;
        }

      

        public int GetLevelUnlockProgressReward()
        {
            return listSpecialObjectSpawnData.Min(x => x.levelStartSpawn);
        }
        public ItemDefinition GetRelicItemDefinition(int index)
        {
            if (listRelicItemDefinition.Count == 0)
                return null;
            
            index = Mathf.Clamp(index, 0, listRelicItemDefinition.Count);
            
            return listRelicItemDefinition[index];
        }

        public float GetRandomSpawnAmount()
        {
            return Random.Range(minSpawnAmount, maxSpawnAmount);
        }
        public float GetRandomSpawnSize()
        {
            return Random.Range(minSpawnSize, maxSpawnSize);
        }
    }
    [System.Serializable]
    public class RelicObjectSpawnData
    {
        public int levelStartSpawn;
        public int minSpawn, maxSpawn;

        public int GetSpawnAmount()
        {
            return Random.Range(minSpawn, maxSpawn + 1);
        }
    }
}