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

    public enum ArtifactRewardType
    {
        Gold,
        LivesInMinutes,
        Booster_EvolvePillInMinutes,
        Booster_WindsurfInMinutes,
        Helper_EnlargeInAmount,
        Helper_MagnetInAmount,
        Helper_CompassInAmount,
        Helper_TimeStopInAmount,
    }
}