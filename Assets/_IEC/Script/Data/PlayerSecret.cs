using System;
using System.Collections;
using System.Collections.Generic;
using _IEC.Features.Booster;
using _IEC.Features.Gameplay;
using MoreMountains.NiceVibrations;
using UnityEngine;
using UnityEngine.Serialization;

namespace IEC
{
    /// <summary>
    /// object contain player secret data (need save and secure)
    /// </summary>
    [System.Serializable]
    public class PlayerSecret : IData
    {
        public long lastTickPlayTime;
        public long startTickPlayTime;
        public string lastVersion;
        public int session;
        public bool IsFirstTime => session == 1;

        public bool isNoAds => Pool.Get<ServiceData>().isNoAds;
        public int ticket;
        public int level = 1;
        public int lastFeatureCount = 0;
        public int levelTracking = 1;
        public int levelFirstTime = 0;
        public Dictionary<LevelType, int> levelFirstTimeTracking;
        public int countPlayLevel = 0;

        public bool isPlayLevelHardFirstTime = true;
        public int currentHardLevel = 0;
        public int currentCountLoseHardLevel = 0; // To check for showing skip option
        public int lastHardLevel = 0;

        public bool isDiscountedPackBought;
        public bool isClickedRate;
        
        public DateTime lastTimeCollectFreeAdsGoldPack;

        public int sessionStartLevel;
        public int LevelSessionCount => session - sessionStartLevel + 1;
        public Dictionary<string, string> dictUserVersion = new Dictionary<string, string>();
        [Header("CCJ")]
        public bool isMusicOn = true;
        public bool isSoundOn = true;
        public bool isVibrateOn = true;
        [Header("Tutorial")]
        public bool passHelperStack;
        public bool passHelperClear;
        
        public HybridData hybridData = new HybridData();

        private GameConfig Config => Pool.Get<GameConfig>();

        private Game Game => Pool.Get<Game>();

        public void AddUserVersion(string version, string abVersion, bool isForceSet = false)
        {
            if (!dictUserVersion.ContainsKey(version) && !isForceSet) 
            {
                dictUserVersion[version] = abVersion;
            }
            if (isForceSet)
            {
                dictUserVersion[version] = abVersion;
            }
        }

        public void AddLevel()
        {
            level += 1;
        }

        public void SaveLastPlayTime()
        {
            lastTickPlayTime = Math.Max(lastTickPlayTime, startTickPlayTime);
            startTickPlayTime = Math.Max(lastTickPlayTime, DateTime.Now.Ticks);
        }

        public DateTime GetLastPlayDateTime()
        {
            if (lastTickPlayTime == 0) return DateTime.Now;
            return new DateTime(lastTickPlayTime);
        }

        public void AfterInit()
        {
            InitCurrency();
            ResetDataDay();
            SetupNewBuildCityData();
            if (IsFirstTime || levelFirstTimeTracking == null || levelFirstTimeTracking .Count < 0)
            {
                ClearAndInitTrackingLevelType();
            }
        }

        public void ClearAndInitTrackingLevelType()
        {
            levelFirstTimeTracking = new Dictionary<LevelType, int>();
            foreach (LevelType type in (LevelType[]) Enum.GetValues(typeof(LevelType)))
            {
                levelFirstTimeTracking.Add(type,0);
            }
        }
        public void Vibrate(HapticTypes hapticType)
        {
            if (!isVibrateOn) return;
            MMVibrationManager.Haptic(hapticType);
        }
        
        public void AddReward(ResourceType type, int amount)
        {
            switch (type)
            {
                case ResourceType.Gold:
                    AddCurrency(ResourceType.Gold,amount);
                    break;
            }
        }
        #region GamePlay
        public PlayerHelperData helperData;
        public BoosterData boosterData;
        #endregion
        public bool IsHardLevel()
        {
            int levelStart = Game.isOverrideHardLevelConfig
                ? Game.levelStartHardLevelOverride
                : Config.levelStartHardLevel;
            
            if (Game.isUsingHardLevel && level >= levelStart)
            {
                // if (level == Config.levelStartHardLevel && isFirstTimePlayHardLevel)
                //     return true;
                
                if (level - lastHardLevel >= Config.stepBetweenHardLevels)
                    return true;
            }

            return false;
        }
        
        #region Player Life
        [Header("Player Life")]
        public int playerLife = 5;
        public DateTime timeStartCountAddLife = DateTime.Now;
        public DateTime timeStartUnlimitedLife = DateTime.MinValue;
        public DateTime timeEndUnlimitedLife = DateTime.MinValue;
        public float realTimeUnlimidtedLife;
        public string playerProfileName = "";
        public int playerProfileIconId;
        public bool levelPending;//check player quit in game
        public bool isKilledAppGamePlay;

        public bool dailyShowLifeOfferPack;
        public void DecreasePlayerLife()
        {
            if (playerLife == Config.maxHealth)
            {
                if (timeStartCountAddLife < DateTime.Now)
                {
                    timeStartCountAddLife = DateTime.Now;
                }
            }

            if (!IsInUnlimitedLife())
            {
                playerLife -= 1;
                playerLife = playerLife < 0 ? 0 : playerLife;
            }
        }

        public bool IsInUnlimitedLife()
        {
            bool passLevelLife = level < Config.levelUnlockLife;
            bool inUnlimitedLife = DateTime.Now > timeStartUnlimitedLife && DateTime.Now < timeEndUnlimitedLife && realTimeUnlimidtedLife > 0;
            return passLevelLife || inUnlimitedLife;
        }
        public void AddTimeUnlimitedLife(int minutes)
        {

            if (DateTime.Now > timeStartUnlimitedLife && DateTime.Now < timeEndUnlimitedLife)
            {
                //Add more time 
                timeEndUnlimitedLife = timeEndUnlimitedLife.AddMinutes(minutes);
            }
            else
            {
                //Start new 
                timeStartUnlimitedLife = DateTime.Now;

                timeEndUnlimitedLife = timeStartUnlimitedLife.AddMinutes(minutes);
            }
            if(realTimeUnlimidtedLife < 0) realTimeUnlimidtedLife = 0;
            realTimeUnlimidtedLife += minutes * 60;
        }
        public void SetTimeUnlimitedLife(int minutes)
        {
            timeStartUnlimitedLife = DateTime.Now; 
            timeEndUnlimitedLife = timeStartUnlimitedLife.AddMinutes(minutes);
        }
        public string GetTimeLeftUnlimitedLife()
        {
            TimeSpan diffTime =timeEndUnlimitedLife - DateTime.Now;
            if (diffTime.TotalSeconds < 0 || realTimeUnlimidtedLife < 0)
            {
                //cheat player
                return "MAX";
            }
            TimeSpan time = timeEndUnlimitedLife - DateTime.Now;
            if (realTimeUnlimidtedLife < diffTime.TotalSeconds)
            {
                time = DateTime.Now.AddSeconds((int)realTimeUnlimidtedLife) - DateTime.Now;
            }
            string outputString = "";
            
            var hour = time.Hours;
            var minute = time.Minutes;
            var seconds = time.Seconds;
            string hours = hour < 10 ? $"0{hour}" : hour.ToString();
            string minutes = minute < 10 ? $"0{minute}" :minute.ToString();
            string second = seconds.ToString();
            if (hour > 0)
            {
                outputString = $"{hours}h {minutes}m";
            }
            else
            {
                outputString = $"{minutes}m {second}s";
            }
            return outputString;
        }

        public void CheckPlayerAddLifeByTime()
        {
            TimeSpan diffTime = DateTime.Now - timeStartCountAddLife;
            if (levelPending && playerLife > 0 )
            {
                //ReducePlayer life when quit in time
                levelPending = false;
                if(!IsInUnlimitedLife()) DecreasePlayerLife();
                Debug.LogWarning("Decrease life by quit in game Play");
            }
            while (playerLife < Config.maxHealth)
            {
                diffTime = DateTime.Now - timeStartCountAddLife;
                if ((diffTime.Minutes + diffTime.Hours * 60 + diffTime.Days * 60 * 24) >= Config.timeRefillLife)
                {
                    timeStartCountAddLife = timeStartCountAddLife.AddMinutes(Config.timeRefillLife);
                    AddPlayerLife();
                }
                else
                {
                    break;
                }
            }
        }
        public void SetTimeToNextLife(int minues)
        {
            timeStartCountAddLife = DateTime.Now.AddMinutes(-minues);
        }

        public void AddPlayerLife()
        {
            playerLife++;
        }
        public string GetTimeToNextLife()
        {
            CheckPlayerAddLifeByTime();
            if (playerLife >= Config.maxHealth)
            {
                return "MAX";
            }
            else
            {
                TimeSpan diffTime = DateTime.Now - timeStartCountAddLife;
                if (diffTime.Ticks < 0)
                {
                    //cheat player
                    return "00:00";
                }
                TimeSpan timeLeft = new TimeSpan(0, 0, Config.timeRefillLife, 0) - diffTime;
                if (timeLeft.Ticks < 0) timeLeft = TimeSpan.Zero;
                return new DateTime(timeLeft.Ticks).ToString("mm:ss");
            }
        }

        public void RaiseToMaxHealth()
        {
            playerLife = Config.maxHealth;
        }

        #endregion
        
        #region City build

        [Header("City Build")] 
        public bool shownFirstTimeBuildCityTut;
        public int currentCityBuild;
        public int screwCityBuild = 0;
        public Dictionary<int, CityData> buildCityData = new Dictionary<int, CityData>();
        
        public class CityData
        {
            public bool isDone;
            public Dictionary<int, int> itemCity = new Dictionary<int, int>();
        }
        
        public void AddScrewBuildCity(int value)
        {
            screwCityBuild += value;
        }
        public void SetupNewBuildCityData()
        {
            if (buildCityData.Count > 0) return;
            for (int i = 0; i <= 99; i++)
            {
                CityData itemCity = new CityData();
                for (int j = 0; j <= 10; j++)
                {
                    itemCity.itemCity[j] = 0;
                }

                buildCityData[i] = itemCity;
            }
        }
        
        #endregion
        
        #region Player Avatar Profile
        [Header("Player Avatar Profile")]
        public string playerAvatarSpriteName;
        public string playerName;    

        #endregion
        
        #region LuckyWheel
        [Header("LuckyWheel")]
        public DateTime lastTimeOpenWheel;
        //public int numberSpinWheelAdsLeft;//Disable, pay by ads or gold 
        public bool firstSpinTutorial;
        public bool luckWheelDailyFreeSpin;
        public int luckWheelDailyAdsSpin;
        public int luckWheelGoldTimeSpin;
        public void ResetDataDay()
        {
            //Another param need reset each 24 day come here
            if (lastTimeOpenWheel.Day != DateTime.Now.Day)
            {
                lastTimeOpenWheel = DateTime.Now;
                luckWheelDailyFreeSpin = true;
                luckWheelDailyAdsSpin = 0;
                dailyShowPiggyBank = false;
                dailyShowLifeOfferPack = false;
            }
        }


        #endregion

        #region Currency

        private Func<int, int, int> add = (x, y) => x + y;
        
        //[ShowInInspector]
        //Using different
        public Dictionary<ResourceType, int> playerCurrencies = new Dictionary<ResourceType, int>();

        public void InitCurrency()
        {
            if (playerCurrencies == null)
            {
                playerCurrencies = new Dictionary<ResourceType, int>();
            }

            foreach (var obj in Enum.GetValues(typeof(ResourceType)))
            {
                if (!playerCurrencies.ContainsKey((ResourceType)obj))
                {
                    playerCurrencies[(ResourceType)obj] = 0;
                }
            }
        }

        public int GetPlayerCurrencyCount(ResourceType currencyType)
        {
            return playerCurrencies.TryGetValue(currencyType, out int value) ? value : -1;
        }

        public void AddCurrency(ResourceType currencyType, int amount, bool isUpdateUI = true)
        {
            if (currencyType == ResourceType.UnlimitedLive)
            {
                AddTimeUnlimitedLife(amount);
            }
            else if (playerCurrencies.ContainsKey(currencyType))
            {
                playerCurrencies[currencyType] += amount;
            }
            
            if(isUpdateUI)
                EventController.RaiseEvent(EventList.GamePlay.UPDATE_RESOURCE_UI);
        }

        public bool SubtractCurrency(ResourceType currencyType, int amount)
        {
            if (playerCurrencies.ContainsKey(currencyType))
            {
                if (playerCurrencies[currencyType] < amount)
                {
                    return false;
                }

                playerCurrencies[currencyType] -= amount;
                switch (currencyType)
                {
                    case ResourceType.Gold:
                        //AddAchievementProgress(AchievementType.SPEND_DIAMOND, amount);
                        break;
                }
            }
            EventController.RaiseEvent(EventList.GamePlay.UPDATE_RESOURCE_UI);
            return true;
        }

        public bool CanSpendCurrency(ResourceType currencyType, int amount)
        {
            if (playerCurrencies.ContainsKey(currencyType))
            {
                return playerCurrencies[currencyType] >= amount;
            }

            return false;
        }
        public void SetCurrency(ResourceType currencyType, int amount)
        {
            if (playerCurrencies.ContainsKey(currencyType))
            {
                playerCurrencies[currencyType] = amount;
            }
        }

        #endregion
        
        #region IAP

        public bool isBoughtStarterPack;
        public DateTime timeEndStaredPack = DateTime.MinValue;
        public List<string> listBundlePurchased = new List<string>();

        public bool WasBuyIapPackNonConsume(string bundleId)
        {
            return listBundlePurchased.Contains(bundleId);
        }
        public void AddBundleIAPPurchased(string bundleId)
        {
            if (!WasBuyIapPackNonConsume(bundleId))
            {
                listBundlePurchased.Add(bundleId);
            }
            //Cache bundle buy once to 
        }
        #endregion

        #region PiggyBank

        public int piggyBankGold;
        public bool wasShowPiggyBankFirstTime;
        public bool dailyShowPiggyBank;
        public void AddGoldToPiggyBank(int value)
        {
            piggyBankGold += value;
        }
        public void CollectPiggyBankGold()
        {
            AddCurrency(ResourceType.Gold, piggyBankGold);
            piggyBankGold = 0;
        }

        #endregion

        #region Daily Reward

        [Header("Daily Reward")] 
        public int currentDailyAdReward = 0;
        public DateTime lastTimeOpenDailyReward;
        public long mainRewardCooldownTickTime;
        public bool claimMainReward;
        public Dictionary<int, bool> adRewardsClaimed = new Dictionary<int, bool>();
        public double GetEndTimeDay(out string outputString)
        {
            outputString = "";
            DateTime now = DateTime.Now;
            DateTime endOfDay = DateTime.Today.AddDays(1);
            TimeSpan time = endOfDay - now;
            var hour = time.Hours;
            string hours = hour < 10 ? $"0{hour}" : hour.ToString();
            string minutes = time.Minutes < 10 ? $"0{time.Minutes}" : time.Minutes.ToString();
            outputString = $"{hours}h {minutes}m";
            return time.TotalSeconds;
        }
        #endregion
        
        #region Add Helper Popup

        public Dictionary<ResourceType, int> adRewardUsedAddHelper = new();

        public DateTime lastTimeResetAdsRewardHelper;

        public void ResetAdsRewardHelper()
        {
            adRewardUsedAddHelper.Clear();
            
            lastTimeResetAdsRewardHelper = DateTime.Today;
        }

        public void UseAdRewardAddHelper(ResourceType helperType)
        {
            if (!adRewardUsedAddHelper.TryAdd(helperType, 0))
            {
                adRewardUsedAddHelper[helperType]++;
            }
        }

        public int GetUsedAdRewardAddHelper(ResourceType helperType)
        {
            adRewardUsedAddHelper.TryAdd(helperType, 0);

            return adRewardUsedAddHelper[helperType];
        }

        #endregion
    }
}