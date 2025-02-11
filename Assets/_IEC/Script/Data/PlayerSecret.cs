using System;
using System.Collections;
using System.Collections.Generic;
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

        #region Player Avatar Profile

        public string playerAvatarSpriteName;
        public string playerName;    

        #endregion
    }
}