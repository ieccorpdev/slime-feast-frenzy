using System;
using System.Collections.Generic;
using System.Linq;
using _IEC.Script.Config;
using UnityEngine;

namespace IEC
{
    [Serializable]
    public class TrophyData
    {
        public List<TrophyDataConfig> data = new List<TrophyDataConfig>();
        public TrophyRewardConfig trophyRewardConfig => Pool.Get<GameConfig>().trophyRewardConfig;
        public void CollectTrophy(TrophyType type)
        {
            Debug.LogError("Collect Trophy:" + type);
            var tdc = GetTrophyDataConfig(type);
            tdc.timeCompleted += 1;
        }
        public void CollectTrophyReward(TrophyType type)
        {
            Debug.LogError("CollectTrophyReward:" + type);
            var tdc = GetTrophyDataConfig(type);
            tdc.trophyCollected += 1;
        }

        public bool IsTrophyCanCollect(TrophyType type)
        {
            int times = GetCurrentTimeComplete(type);
            int trophyCol = GetCurrentCollectedTrophy(type);
            int trophyTarget = GetTrophyByTimeComplete(type, times);
            return trophyTarget > trophyCol;
        }

        public int GetTrophyByTimeComplete(TrophyType type,int times)
        {
           var dataConfig = trophyRewardConfig.listTrophyReward.Find(x => x.type == type);
            for (int i = 0; i < dataConfig.listTimeToCollect.Count; i++)
            {
                if (dataConfig.listTimeToCollect[i] >= times)
                {
                    return i + 1;
                }
            }

            return 0;
        }
        public int GetCurrentTimeComplete(TrophyType type)
        {
            return GetTrophyDataConfig(type).timeCompleted;
        }
        public int GetCurrentCollectedTrophy(TrophyType type)
        {
            return GetTrophyDataConfig(type).trophyCollected;
        }

        public Vector2 GetMinMaxProgressBar(TrophyType type)
        {
            var config = trophyRewardConfig.GetReward(type);
            var dataSave = GetTrophyDataConfig(type);
            int start = config.listTimeToCollect[dataSave.trophyCollected];
            if (dataSave.timeCompleted == 0)
            {
                return new Vector2(dataSave.timeCompleted, start);
            }
            else
            {
                int end = start;
                if (dataSave.trophyCollected < config.listTimeToCollect.Count)
                {
                    end = config.listTimeToCollect[dataSave.trophyCollected+1];
                }
                return new Vector2(dataSave.timeCompleted - start, end - start);
            }
        }
        public void CheckLevelTrophy(int level)
        {
            var levelTrophy = GetTrophyDataConfig(TrophyType.GrandVoyage);
            levelTrophy.timeCompleted = level;
        }
        public TrophyDataConfig GetTrophyDataConfig(TrophyType type)
        {
            var result = data.Find(x => x.typeTrophy == type);
            if (result == null)
            {
                //Init config if not found TrophyData;
                TrophyDataConfig newConfig = new TrophyDataConfig();
                newConfig.typeTrophy = type;
                data.Add(newConfig);
                return newConfig;
            }
            else
            {
                return result;
            }
        }
    }
    [Serializable]public class TrophyDataConfig
    {
        public TrophyType typeTrophy;
        public int timeCompleted;
        public int trophyCollected = 0;// 1 mean was collect bronze, 2 silver, 3 gold 4 diamond
    }
    public enum TrophyType{
        GrandVoyage,
        SharkFrenzyForce,
        UnderseaVoyage,
        SharkRace,
        ChompOff,
        OceanGift,
        StarRace,
        RelicHunt,
        //
        //
    }


}