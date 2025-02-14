using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.NiceVibrations;
using UnityEngine;

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
        public int gold;
        public int ticket;
        public int level = 1;
        public int levelTracking = 1;
        public int levelFirstTime = 0;
        public int countPlayLevel = 0;
        public bool isSound = true;
        public bool isMusic = true;
        public bool isVibrate = true;

        public int sessionStartLevel;
        public int LevelSessionCount => session - sessionStartLevel + 1;

        public Dictionary<string, string> dictUserVersion = new Dictionary<string, string>();
        
        public HybridData hybridData = new HybridData();
        
        private GameConfig Config => Pool.Get<GameConfig>();

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
            SetDataLuckyWheel();
            SetupNewBuildCityData();
        }
        
        public void Vibrate(HapticTypes hapticType)
        {
            if (!isVibrate) return;
            MMVibrationManager.Haptic(hapticType);
        }
        
        public void AddReward(ResourceType type, int amount)
        {
            switch (type)
            {
                case ResourceType.Gold:
                    AddGold(amount);
                    break;
            }
        }
        #region Player Life
        public int playerLife = 5;
        public DateTime timeStartCountAddLife = DateTime.Now;
        public DateTime timeStartUnlimitedLife = DateTime.MinValue;
        public DateTime timeEndUnlimitedLife = DateTime.MinValue;
        public float realTimeUnlimidtedLife;
        public string playerProfileName = "";
        public int playerProfileIconId;
        public bool levelPending;//check player quit in game
        public bool isKilledAppInGamePlay;
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
            }
        }

        public bool IsInUnlimitedLife()
        {
            return (DateTime.Now > timeStartUnlimitedLife && DateTime.Now < timeEndUnlimitedLife && realTimeUnlimidtedLife > 0);
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

        public string GetTimeLeftUnlimidtedLife()
        {
            TimeSpan diffTime =timeEndUnlimitedLife - DateTime.Now;
            if (diffTime.TotalSeconds < 0 || realTimeUnlimidtedLife < 0)
            {
                //cheat player
                return "00:00";
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
                if(!IsInUnlimitedLife()) playerLife--;
                Debug.LogWarning("Decrease life by quit in game Play");
            }
            while (playerLife < Config.maxHealth)
            {
                diffTime = DateTime.Now - timeStartCountAddLife;
                if ((diffTime.Minutes + diffTime.Hours * 60 + diffTime.Days * 60 * 24) >= 30)
                {
                    timeStartCountAddLife = timeStartCountAddLife.AddMinutes(30);
                    AddPlayerLife();
                }
                else
                {
                    break;
                }
            }
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
                TimeSpan timeLeft = new TimeSpan(0, 0, 30, 0) - diffTime;
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

        public string playerAvatarSpriteName;
        public string playerName;    

        #endregion
        
        #region LuckyWheel

        public DateTime lastTimeOpenWheel;
        public int numberSpinWheelAdsLeft;

        public void SetDataLuckyWheel()
        {
            if (lastTimeOpenWheel.Day != DateTime.Now.Day)
            {
                lastTimeOpenWheel = DateTime.Now;
                numberSpinWheelAdsLeft = Pool.Get<GameConfig>().dataRewardLuckyWheelConfig.numberSpinAdsEachDay;
            }
        }

        #region Resource Currency

        public void AddGold(int value)
        {
            gold += value;
            EventController.RaiseEvent(EventList.GamePlay.UPDATE_RESOURCE_UI);
        }
        //Custome another type currency here

        #endregion
        #endregion

        #region Started Pack IAP

        public bool isBoughtStarterPack;

        #endregion

        #region PiggyBank

        public int piggyBankGold;

        #endregion

        #region Daily Reward

        [Header("Daily Reward")] 
        public int currentDailyAdReward = 0;
        public DateTime lastTimeOpenDailyReward;
        public long mainRewardCooldownTickTime;
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

    }
}