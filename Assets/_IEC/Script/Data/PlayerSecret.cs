using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    /// <summary>
    /// object contain player secret data (need save and secure)
    /// </summary>
    [System.Serializable]
    public class PlayerSecret
    {
        public string lastVersion;
        public bool isNoAds;
        public int gold;

        public int level = 1;
        public int levelFirstTime = 1;
        public int countPlayThisLevel;

        public bool isSoundOn, isVibrateOn, isFirstSetup;

        public bool haveAlreadyRate;
        public List<int> levelShowRate;

        public void AfterInit()
        {
            if (levelShowRate == null) levelShowRate = new List<int>();
        }
    }
}