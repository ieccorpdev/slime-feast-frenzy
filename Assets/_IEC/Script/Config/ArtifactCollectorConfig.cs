using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace _IEC.Script.Config
{
    [CreateAssetMenu(fileName = "ArtifactCollectorConfig", menuName = "Game/ArtifactCollectorConfig", order = 0)]
    public class ArtifactCollectorConfig : ScriptableObject
    {
        public int daysEachReset = 3;
        public List<ArtifactLevelNodeModel> ArtifactLevels = new List<ArtifactLevelNodeModel>();

        // private void OnValidate()
        // {
        //     for (int i = 0; i < ArtifactNodeModels.Count; i++)
        //     {
        //         ArtifactNodeModels[i].id = i + 1;
        //     }
        // }
    }
    [System.Serializable]
    public class ArtifactLevelNodeModel
    {
        public List<ArtifactNodeModel> artifactLevelRewards = new List<ArtifactNodeModel>();
    }
    [System.Serializable]
    public class ArtifactNodeModel
    {
      //  public int id;
        public int tokenCost;
        public List<ArtifactRewardData> artifactRewards = new List<ArtifactRewardData>();
    }

    [System.Serializable]
    public class ArtifactRewardData
    {
        public ArtifactRewardType artifactRewardType;
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