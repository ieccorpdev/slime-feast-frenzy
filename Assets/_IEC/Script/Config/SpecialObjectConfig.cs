using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _IEC.Features.Gameplay;
using UnityEngine;
using Random = UnityEngine.Random;

namespace IEC
{
    [CreateAssetMenu(menuName = "Game/SpecialObjectConfig")]
    public class SpecialObjectConfig : ScriptableObject
    {
        public float minSpawnSize, maxSpawnSize;
        public float minSpawnSizeKingdom, maxSpawnSizeKingdom;
        public List<SpecialObjectSpawnData> listSpecialObjectSpawnData = new();
        public List<ItemDefinition> listSpecialItemDefinition = new();
        [Header("Gold Level")] 
        public ItemDefinition goldSpecialObjective;
        [Header("Kingdom Special Level")]
        public ItemDefinition kingdomSpecialObjective;
        public SpecialObjectSpawnData kingdomSpecialObjectSpawnData;

        public int GetSpawnAmount(int level)
        {
            var spawnData = listSpecialObjectSpawnData
                .OrderBy(data => data.levelStartSpawn)
                .LastOrDefault(data => data.levelStartSpawn <= level);

            return spawnData?.GetSpawnAmount() ?? 0;
        }

        public int GetSpawnAmount()
        {
            return kingdomSpecialObjectSpawnData.GetSpawnAmount();
        }

        public int GetLevelUnlockProgressReward()
        {
            return listSpecialObjectSpawnData.Min(x => x.levelStartSpawn);
        }
        public ItemDefinition GetSpecialItemDefinition(int index)
        {
            if (listSpecialItemDefinition.Count == 0)
                return null;
            
            index = Mathf.Clamp(index, 0, listSpecialItemDefinition.Count);
            
            return listSpecialItemDefinition[index];
        }

        public float GetRandomSpawnSize(string itemId)
        {
            if (itemId.Equals(EventList.GamePlay.Kingdom_Special))
            {
                return Random.Range(minSpawnSizeKingdom, maxSpawnSizeKingdom);
            }
            return Random.Range(minSpawnSize, maxSpawnSize);
        }
    }

    [Serializable]
    public class SpecialObjectSpawnData
    {
        public int levelStartSpawn;
        public int minSpawn, maxSpawn;

        public int GetSpawnAmount()
        {
            return Random.Range(minSpawn, maxSpawn + 1);
        }
    }
}
