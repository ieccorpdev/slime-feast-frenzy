using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        private bool isMultiCard = false;
        public float timeDelayFullAds;
        public int levelStartInterAds = 4;

        public bool isUsingHardLevel;
        public bool isUsingExtraLvlFollow;
        public bool isUsingLuckySpinV2Flow;
        public bool isUsingBuildV2Flow;
        public bool isUsingConnectedBox;
        public bool isUsingCustomLoopLevelFlow;

        public bool isOverrideHardLevelConfig;
        public bool isAllowSkipHardLevelAds;
        public int levelStartHardLevelOverride = 31;
        public int loseBeforeAllowSkipHardLevelOverride = 2;

        public bool isUsePopupRefillHelper;

        public bool isOverrideHelperCostData;

        public bool isOverrideDataRewardLuckyWheel;

        [Header("Adjust")]
        public bool isLevelSFAAdjustment;
        [HideInInspector]public int deaultLevelSFA = 5;
        [HideInInspector]public int levelSFAAdjust = 8;
        
        public System.DateTime timeStartLevel;
        public System.DateTime timeEndLevel;
        public int TimeFinishLevel => (int)(timeEndLevel - timeStartLevel).TotalSeconds;

        public bool IsMultiCard
        {
            get
            {
                return isMultiCard;
            }
            set => isMultiCard = value;
        }
        

        public void SetTimeStartLevel()
        {
            timeStartLevel = System.DateTime.Now;
        }

        public void SetTimeEndLevel()
        {
            timeEndLevel = System.DateTime.Now;
        }

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

    public enum LevelType
    {
        NONE,
        NORMAL,
        NEW_NORMAL,
        HARD,
    }
}