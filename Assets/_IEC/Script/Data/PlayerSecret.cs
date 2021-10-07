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
    public class PlayerSecret
    {
        public string lastVersion;
        public bool isNoAds;
        public int gold;

        public int level = 1;
        public int levelFirstTime = 0;
        public int countPlayThisLevel;

        public bool isSoundOn, isVibrateOn, isFirstSetup;

        public bool haveAlreadyRate;
        public List<int> levelShowRate;

        public int themeSelected;
        public List<int> listThemeUnlocked;

        public int backgroundSelected;
        public List<int> listBackgroundUnlocked;

        public void AddGold(int goldAdd)
        {
            gold += goldAdd;
        }

        public void AfterInit()
        {
            if (levelShowRate == null) levelShowRate = new List<int>();
            if (listThemeUnlocked == null) 
            {
                listThemeUnlocked = new List<int>();
            }

            if (listBackgroundUnlocked == null)
            {
                listBackgroundUnlocked = new List<int>();
            }
        }

        public void InitTheme(int themeDefault, int bgDefault)
        {
            if(listThemeUnlocked.Count == 0)
            {
                UnlockTheme(themeDefault);
            }

            if (listBackgroundUnlocked.Count == 0)
            {
                UnlockBackground(bgDefault);
            }
        }

        public void UnlockTheme(int index)
        {
            if (!listThemeUnlocked.Contains(index))
            {
                listThemeUnlocked.Add(index);
            }
            themeSelected = index;
        }

        public void UnlockBackground(int index)
        {
            if (!listBackgroundUnlocked.Contains(index))
            {
                listBackgroundUnlocked.Add(index);
            }
            backgroundSelected = index;
        }

        public bool SelectShopItem(ItemShopType type, int index)
        {
            if(type == ItemShopType.Theme)
            {
                if (index == themeSelected) return false;

                if (listThemeUnlocked.Contains(index))
                {
                    themeSelected = index;

                    return true;
                }
            }
            else if(type == ItemShopType.Background)
            {
                if (index == backgroundSelected) return false;
                if (listBackgroundUnlocked.Contains(index))
                {
                    backgroundSelected = index;

                    return true;
                }
            }

            return false;
        }

        public int numberInterShow;
        public DateTime currentInterShowDate;

        public void IncreaseNumberInterShow(int value = 1)
        {
            CheckResetNumberInterShow();
            numberInterShow += value;
        }

        public int GetNumberInterShow()
        {
            CheckResetNumberInterShow();
            return numberInterShow;
        }

        public void CheckResetNumberInterShow()
        {
            if (currentInterShowDate == DateTime.Now.Date) return;
            currentInterShowDate = DateTime.Now.Date;
            numberInterShow = 0;
        }
    }
}