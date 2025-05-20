using _IEC.Features.Gameplay;
using UnityEngine;

namespace IEC
{
    /// <summary>
    /// object contain game session data (no save)
    /// </summary>
    [System.Serializable]
    public class Game : IData
    {
        #region Hybrid

        public HybridGame hybrid = new HybridGame();

        #endregion
        public bool isDebugReset;
        public int emptyData;
        public int abVersion;
        public bool levelCreative;
        public bool isHaveBooster = false;
        public float timeDelayFullAds = 120f;
        public float timeDelayFullAds2050 = 105;
        public float timeDelayFullAds50Plus = 90f;
        public int levelStartShowFullAds = 8;
        public int levelVersion = 1;
        public bool enableLevelProgress;
        public bool isUsingHardLevel;
        public bool enableTreasureHunt;
        public bool isImmortal;

        public bool isUsingGroundExtend = true ;
        public int itemVersion;
        public int levelStartInterAds = 4;
        public bool isUsingBtnAdsMoreLife = true;
        public bool usingWaterTheme = true;

        public float GetTimeDelayFullAds()
        {
            int level = Pool.Get<PlayerSecret>().level;
            if (level >= 50)
            {
                return timeDelayFullAds50Plus;
            } else if (level >= 20)
            {
                return timeDelayFullAds2050;
            }
            return timeDelayFullAds;
        }
        public bool ShowFullAdsCondition
        {
            get
            {
#if UNITY_ANDROID
                return Pool.Get<PlayerSecret>().level > levelStartShowFullAds;
#endif

#if UNITY_IOS
                return Pool.Get<PlayerSecret>().level > levelStartShowFullAds;
#endif
            }
        }

        public System.DateTime timeStartLevel;
        public System.DateTime timeEndLevel;
        public bool enablePiggyBank;
        
        public bool isHaveProfileAndLeaderboard;
        
        public int TimeFinishLevel => (int)(timeEndLevel - timeStartLevel).TotalSeconds;

        public void SetTimeStartLevel()
        {
            timeStartLevel = System.DateTime.Now;
        }

        public void SetTimeEndLevel()
        {
            timeEndLevel = System.DateTime.Now;
        }
        public bool isOverrideHardLevelConfig;
        public bool isAllowSkipHardLevelAds;
        public int levelStartHardLevelOverride = 31;
        public int loseBeforeAllowSkipHardLevelOverride = 2;

        public bool dropSplashWater = true;
        public bool enableComboCount = true;
        public bool haveDailyReward = true;
        public bool isOverrideDataRewardLuckyWheel;
        [Header("SFF")] public bool usingNewLogicPull = false;
        [Header("Tracking")]
        public LevelType crLevelType = LevelType.NONE;
        [Header("IAP")] 
        public int timeSkipAdsStartPackage = 20;//Minutes
        [Header("Tutorial")] 
        public bool inTutorial;
        public bool inTutorialLevel1;
        public bool inTutorialLevel2;
        public bool inTutorialStack;
        public int levelTutorialStack = 3;
        public bool isHaveLeaderboard = true;
        public bool isHaveAnimMedal;
    }
}