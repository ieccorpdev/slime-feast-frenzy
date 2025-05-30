using System;
using System.Collections.Generic;
using System.Linq;
using _IEC.Features.Gameplay;
using IEC;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _IEC.Script.Config
{
    [CreateAssetMenu(fileName = "ArtifactCollectorConfig", menuName = "Game/ArtifactCollectorConfig", order = 0)]
    public class ArtifactCollectorConfig : ScriptableObject
    {
        
        public int daysEachReset = 3;
        public int levelUnlockFeature = 15;
        public List<ArtifactLevelNodeModel> ArtifactLevels = new List<ArtifactLevelNodeModel>();
        public ArtifactLevelNodeModel grandPrizeReward;
        
        [Header("Spawn Config")]
        public int maxSpawnAmount = 5;
        public int minSpawnAmount = 3;
        public float maxSpawnSize = 1.5f;
        public float minSpawnSize = 0.8f;
        public List<RelicObjectSpawnData> listSpecialObjectSpawnData = new();
        public List<ItemDefinition> listRelicItemDefinition = new();
        public int eventDurationInHours = 48;

        public int GetSpawnAmount(int level)
        {
            var spawnData = listSpecialObjectSpawnData
                .OrderBy(data => data.levelStartSpawn)
                .LastOrDefault(data => data.levelStartSpawn <= level);

            return spawnData?.GetSpawnAmount() ?? 0;
        }

      

        public ItemDefinition GetRelicItemDefinition(int index)
        {
            if (listRelicItemDefinition.Count == 0)
                return null;
            
            index = Mathf.Clamp(index, 0, listRelicItemDefinition.Count);
            
            return listRelicItemDefinition[index];
        }

        public int GetRandomSpawnAmount()
        {
            return Random.Range(minSpawnAmount, maxSpawnAmount + 1);
        }
        public float GetRandomSpawnSize()
        {
            return Random.Range(minSpawnSize, maxSpawnSize);
        }
        public int GetStackTokenCost(int id)
        {
            int rs = 0;
            var level = ArtifactLevels.Find(x => x.artifactLevelRewards.Exists(y => y.id == id));
            var curRwIndex = level.artifactLevelRewards.FindIndex(x => x.id == id);
            for (int i = 0; i <= curRwIndex; i++)
            {
                rs += level.artifactLevelRewards[i].tokenCost;
            }
            return rs;
        }

        private void OnValidate()
        {
            int index = 0;
            foreach (var levelNode in ArtifactLevels)
            {
                foreach (var t in levelNode.artifactLevelRewards)
                {
                    t.id = index;
                    index++;
                }
            }
        }
    }

    [System.Serializable]
    public class ArtifactLevelNodeModel
    {
        public List<ArtifactNodeModel> artifactLevelRewards = new List<ArtifactNodeModel>();
    }

    [System.Serializable]
    public class ArtifactNodeModel
    {
        public int id;
        public int tokenCost;
        public List<ArtifactRewardData> artifactRewards = new List<ArtifactRewardData>();
    }

    [System.Serializable]
    public class ArtifactRewardData
    {
        public ResourceType artifactRewardType;
        public int value;
    }
    [System.Serializable]
    public class RelicObjectSpawnData
    {
        public int levelStartSpawn;
        public int minSpawn, maxSpawn;

        public int GetSpawnAmount()
        {
            var rs = Random.Range(minSpawn, maxSpawn + 1);
            Debug.Log($"GetSpawnAmount {rs}");
            return rs;
        }
    }
    
}