using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    [CreateAssetMenu(menuName = "Game/BuildCityConfig")]
    public class BuildCityConfig : ScriptableObject
    {
        public int levelUnlock = 10;
        public Material islandActive;
        public Material islandActive_2;
        public Material islandOff;
        public int screwAddEndgame = 15;
        public float speedRotateIsland = 1;
        public bool useCustomScrewIsland;
        public List<BuildCityDataConfig> dataCity;

        public int GetBaseScrewCompleteIsland(int idIsland)
        {
           return idIsland == 0 ? 150 : (200 + (idIsland) * 150);
        }
    }

    [Serializable]
    public class BuildCityDataConfig
    {
        public int idCity;
        public string cityName;
        public List<int> numberScrewNeedUnlockItem;
        public List<CityReward> listReward;
    }
}


