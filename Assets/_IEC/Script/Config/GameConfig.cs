using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace IEC
{
    /// <summary>
    /// object contain references to all other configs (hard fixed data)
    /// </summary>
    [CreateAssetMenu(menuName = "Game/GameConfig")]
    public class GameConfig : BaseConfig
    {
        public _EmptyConfig emptyConfig;
        public HybridConfig hybridConfig;
        [Header(" Player Avatar Profile")]
        public PlayerAvatarConfig playerAvatarConfig;
        [Header("Lucky Wheel Config")]
        public DataRewardLuckyWheelConfig dataRewardLuckyWheelConfig;   
        [Header("PiggyBank Config")]
        public DataPiggyBank dataPiggyBank;   
        [Header("Daily Reward Config")]
        public DataDailyReward dataDailyRewardConfig;
        [Header("City Build")] 
        public BuildCityConfig buildCityConfig;

    }
}