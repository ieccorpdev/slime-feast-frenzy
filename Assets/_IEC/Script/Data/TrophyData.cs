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
            TrophyDataConfig tdc = GetTrophyDataConfig(type);
            if (IsEventTrophy(type))
            {
                //for event, change method count
                var config = Pool.Get<GameConfig>().trophyRewardConfig.GetReward(type);
                tdc.timeCompleted -= config.listTimeToCollect[tdc.trophyCollected];
                tdc.timeCompleted = tdc.timeCompleted <= 0 ? 0 : tdc.timeCompleted;
            }
            tdc.trophyCollected += 1;
            //After collect, reduce number times collected 

        }

        public bool IsEventTrophy(TrophyType type)
        {
            return type != TrophyType.GrandVoyage && type != TrophyType.SharkFrenzyForce;
        }

        public bool IsTrophyReadyToCollect(TrophyType type)
        {
            var data = GetTrophyDataConfig(type);
            int times = data.timeCompleted;
            int trophyCol = data.trophyCollected;
            int trophyTarget = 0;
            int max = Mathf.Clamp(trophyCol, 0, 3);
            for (int i = 0; i <= max; i++)
            {
                trophyTarget += GetTargetToGetTrophy(type, i);
            }
            return times >= trophyTarget;
        }

        public (bool found,TrophyType type) GetTrophyNotiUnlocked()
        {
            foreach (var name in Enum.GetValues(typeof(TrophyType)))
            {
                var typeTemp = (TrophyType)name;
                var data = GetTrophyDataConfig(typeTemp);
                if (IsTrophyReadyToCollect(typeTemp) && data.indexTrophyWasNoti <= data.trophyCollected)
                {
                    return (true, typeTemp);
                }
            }
            return (false,TrophyType.GrandVoyage);
        }
        public bool IsTrophyCanCollect(TrophyType type)
        {
            return IsTrophyReadyToCollect(type);
        }
        public int GetTargetToGetTrophy(TrophyType type,int trophyCol)
        {
           var dataConfig = Pool.Get<GameConfig>().trophyRewardConfig.GetReward(type);
           if (trophyCol == 0)
           {
               return dataConfig.listTimeToCollect[0];
           }
           else
           {
               int index = Mathf.Clamp(trophyCol,0,dataConfig.listTimeToCollect.Count-1);
               return dataConfig.listTimeToCollect[index];
           }
           return 0;
        }
        public int GetCurrentTimeComplete(TrophyType type)
        {
            return GetTrophyDataConfig(type).timeCompleted;
        }
        public int GetMaxTrophyNoti(TrophyType type)
        {
            return GetTrophyDataConfig(type).indexTrophyWasNoti;
        }
        public int GetCurrentCollectedTrophy(TrophyType type)
        {
            return GetTrophyDataConfig(type).trophyCollected;
        }

        public Vector2 GetMinMaxProgressBar(TrophyType type)
        {
            var config = Pool.Get<GameConfig>().trophyRewardConfig.GetReward(type);
            var dataSave = GetTrophyDataConfig(type);
            int index = Mathf.Clamp(dataSave.trophyCollected, 0, 3);
            int target = config.listTimeToCollect[index];
            int start = GetCurrentTimeComplete(type);
            if (IsEventTrophy(type) || dataSave.trophyCollected == 0)
            {
                return new Vector2(dataSave.timeCompleted, target);
            }
            else
            {
                //With another, sum need be all cal
                int totalPerivousTarget = 0;
                for (int i = 0; i < config.listTimeToCollect.Count; i++)
                {
                    if (i < dataSave.trophyCollected)
                    {
                        totalPerivousTarget += config.listTimeToCollect[i];
                    }
                }
                int lastTarget = config.listTimeToCollect[dataSave.trophyCollected-1];
                return new Vector2(dataSave.timeCompleted - totalPerivousTarget, target );
                //return new Vector2(start,end);
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
        public int indexTrophyWasNoti;//Save the maximum trophy was noti for player, 1 mean was collect bronze, 2 silver, 3 gold 4 diamond
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