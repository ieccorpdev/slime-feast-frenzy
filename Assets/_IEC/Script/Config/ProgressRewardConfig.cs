using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    [CreateAssetMenu(menuName = "Game/ProgressRewardConfig")]
    public class ProgressRewardConfig : ScriptableObject
    {
        public int daysEachReset = 3;
        public List<ProgressRewardSprite> listRewardSprite = new();
        public List<ProgressRewardData> listRewardData = new();

        public ProgressRewardData GetRewardData(int index)
        {
            index = Mathf.Clamp(index, 0, listRewardData.Count - 1);

            return listRewardData[index];
        }

        public Sprite GetRewardSprite(ResourceType rewardType)
        {
            return listRewardSprite.Find(x => x.rewardType == rewardType).icon;
        }
    }

    [Serializable]
    public class ProgressRewardData
    {
        public int progress;
        public Reward reward;
    }

    [Serializable]
    public class ProgressRewardSprite
    {
        public ResourceType rewardType;
        public Sprite icon;
    }
}
