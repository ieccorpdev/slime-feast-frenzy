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
        public int levelFirstTime = 1;
        public int countPlayThisLevel;

        public bool isSoundOn, isVibrateOn, isFirstSetup;

        public bool haveAlreadyRate;
        public List<int> levelShowRate;

        public int themeSelected;
        public List<int> listThemeUnlocked;

        public int backgroundSelected;
        public List<int> listBbackgroundUnlocked;
        public List<int> listBackgroundUnlocked;

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
                if(listBbackgroundUnlocked.Count > 0)
                {
                    UnlockBackground(listBbackgroundUnlocked[0]);
                }
                else
                {
                    UnlockBackground(bgDefault);
                }
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

        public void SelectShopItem(ItemShopType type, int index)
        {
            if(type == ItemShopType.Theme)
            {
                if (listThemeUnlocked.Contains(index))
                {
                    themeSelected = index;
                }
            }
            else if(type == ItemShopType.Background)
            {
                if (listThemeUnlocked.Contains(index))
                {
                    backgroundSelected = index;
                }
            }
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