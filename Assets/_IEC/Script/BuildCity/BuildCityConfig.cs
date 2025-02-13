using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    [CreateAssetMenu(menuName = "Game/BuildCityConfig")]
    public class BuildCityConfig : ScriptableObject
    {
        public Material islandActive;
        public Material islandActive_2;
        public Material islandOff;
        public int screwAddEndgame = 10;
        public List<BuildCityDataConfig> dataCity;
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


