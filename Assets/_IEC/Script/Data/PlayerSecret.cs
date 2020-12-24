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

        public int themeSelected;
        public List<int> listThemeUnlocked;

        public int backgroundSelected;
        public List<int> listBbackgroundUnlocked;

        public void AfterInit()
        {
            if (levelShowRate == null) levelShowRate = new List<int>();
            if (listThemeUnlocked == null) 
            {
                listThemeUnlocked = new List<int>();
                UnlockTheme(2);
            }

            if (listBbackgroundUnlocked == null) 
            {
                listBbackgroundUnlocked = new List<int>();
                UnlockBackground(2);
            } 
        }

        public void UnlockTheme(int index)
        {
            if (!listThemeUnlocked.Contains(index))
            {
                listThemeUnlocked.Add(index);
                themeSelected = index;
            }
        }

        public void UnlockBackground(int index)
        {
            if (!listBbackgroundUnlocked.Contains(index))
            {
                listBbackgroundUnlocked.Add(index);
                backgroundSelected = index;
            }
        }
    }
}