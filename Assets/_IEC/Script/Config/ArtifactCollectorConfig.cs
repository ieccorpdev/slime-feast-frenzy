using System;
using System.Collections.Generic;
using IEC;
using UnityEngine;
using UnityEngine.Serialization;

namespace _IEC.Script.Config
{
    [CreateAssetMenu(fileName = "ArtifactCollectorConfig", menuName = "Game/ArtifactCollectorConfig", order = 0)]
    public class ArtifactCollectorConfig : ScriptableObject
    {
        public int daysEachReset = 3;
        public int levelUnlockFeature = 15;
        public List<ArtifactLevelNodeModel> ArtifactLevels = new List<ArtifactLevelNodeModel>();
        public ArtifactLevelNodeModel grandPrizeReward;

        public bool HasWardAvaialable(PlayerSecret playerData)
        {
            var level = playerData.relicData.curRelicLevel;
            var curToken = playerData.relicData.currentRelicTokenCount;
            if (level >= ArtifactLevels.Count)
            {
                //check max level
                var levelNodeCount = 3; //fixed
                for (int i = 0; i < levelNodeCount; i++)
                {
                }
            }
            else
            {
            }

            return false;
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

}