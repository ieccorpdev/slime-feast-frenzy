using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        public bool IsCompleteReward(int currentIndex)
        {
            return currentIndex >= listRewardData.Count;
        }

        public Sprite GetRewardSprite(ResourceType rewardType)
        {
            return listRewardSprite.FirstOrDefault(x => x.rewardType == rewardType)?.icon;
        }
    }

    [Serializable]
    public class ProgressRewardData
    {
        public int progress;
        public ProgressRewardType progressRewardType;
        public Reward reward;

        public string GetDisplayText()
        {
            string result = reward.rewardAmount.ToString();

            switch (progressRewardType)
            {
                case ProgressRewardType.Gold:
                    break;
                case ProgressRewardType.Helper:
                    result = "x" + result;
                    break;
                case ProgressRewardType.TimeEffect:
                    result += "min";
                    break;
            }
            return result;
        }
    }

    [Serializable]
    public class ProgressRewardSprite
    {
        public ResourceType rewardType;
        public Sprite icon;
    }

    public enum ProgressRewardType
    {
        Gold,
        Helper,
        TimeEffect,
    }
}
