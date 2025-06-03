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

        public void CollectTrophy(TrophyType type)
        {
            Debug.LogError("Collect Trophy:" + type);
            var tdc = GetTrophyDataConfig(type);
           tdc.timeCompleted += 1;
        }

        public void CheckLevelTrophy(int level)
        {
            var levelTrophy = GetTrophyDataConfig(TrophyType.GrandVoyage);
            if (level < 50)
            {
                levelTrophy.timeCompleted = 0;
            } 
            else if (level < 100)
            {
                levelTrophy.timeCompleted = 1;
            }
            else if (level < 500)
            {
                levelTrophy.timeCompleted = 2;
            }
            else if (level < 1000)
            {
                levelTrophy.timeCompleted = 3;
            }
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