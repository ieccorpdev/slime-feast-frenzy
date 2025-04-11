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
        public List<ProgressRewardData> listRewardData = new();

        public ProgressRewardData GetRewardData(int index)
        {
            index = Mathf.Clamp(index, 0, listRewardData.Count - 1);

            return listRewardData[index];
        }
    }

    [Serializable]
    public class ProgressRewardData
    {
        public int progress;
        public List<Reward> listReward = new();
    }
}
