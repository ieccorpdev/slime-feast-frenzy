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
        public bool isShowJourneyTutorial;
        public void CollectTrophy(TrophyType type,int value = 1)
        {
            Debug.LogError("Collect Trophy:" + type);
            var tdc = GetTrophyDataConfig(type);
            tdc.timeCompleted += value;
        }
        public void SetTrophyTimes(TrophyType type,int value)
        {
            var tdc = GetTrophyDataConfig(type);
            tdc.timeCompleted = value;
        }
        public void SetTrophyCollected(TrophyType type,int value)
        {
            var tdc = GetTrophyDataConfig(type);
            tdc.trophyCollected = value;
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
            int trophyTarget = GetTargetToGetTrophy(type, times,trophyCol);//Target
            return times >= trophyTarget;
        }

        public int GetTargetToGetTrophy(TrophyType type,int times,int trophyCol)
        {
           var dataConfig = Pool.Get<GameConfig>().trophyRewardConfig.GetReward(type);
           if (trophyCol == 0)
           {
               return dataConfig.listTimeToCollect[0];
           }
           else
           {
               return dataConfig.listTimeToCollect[trophyCol];
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
            var config = Pool.Get<GameConfig>().trophyRewardConfig.GetReward(type);
            var dataSave = GetTrophyDataConfig(type);
            int start = config.listTimeToCollect[dataSave.trophyCollected];
            if (dataSave.trophyCollected == 0)
            {
                return new Vector2(dataSave.timeCompleted, start);
            }
            else
            {

                int end = start;
                start = config.listTimeToCollect[dataSave.trophyCollected-1];
                return new Vector2(dataSave.timeCompleted - start, end - start);
            }
        }
        public void SetLevelTrophy(int level)
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