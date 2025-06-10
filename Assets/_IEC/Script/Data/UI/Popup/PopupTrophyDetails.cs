using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using IEC.Common.Event;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class PopupTrophyDetails : BasePopup
    {
        public TrophyType type;
        public ItemTrophyState state;
        public List<TrophyCustomTitle> listFontMaterial;
        public Image mainSpriteTrophy;
        public Sprite defaultBg;
        public Sprite eventTimeBg;
        public TextMeshProUGUI mainTileTrophy;
        public TextMeshProUGUI trophyUnlockText;
        [Header("Idle Groups")]
        public GameObject infoGroups;
        public GameObject eventTimeObj;
        public TextMeshProUGUI timeEventTMP;
        public TextMeshProUGUI infoTrophyTMP;
        public List<PTroyphyItemData> listSpriteTrophy;
        [Header("Unlocked Groups")]
        public GameObject unlockedGroups;
        public Image badgeImage;
        public Button trophyRoomButton;

        public void ShowTrophy(TrophyType type,ItemTrophyState state)
        {
            base.Show();
            this.type = type;
            this.state = state;
            infoGroups.gameObject.SetActive(false);
            unlockedGroups.gameObject.SetActive(false);
            var crState = Locator.Main.ScreenMain.trophyTab.GetStateTrophy(type);
            var data = PlayerData.trophyData.GetTrophyDataConfig(type);
            LoadDefaultData();
            if (state == ItemTrophyState.Complete || state == ItemTrophyState.New)
            {
                
            }
            else
            {
                ShowInfoGroups();
            }
        }

        public void LoadDefaultData()
        {
            int crTrophy = PlayerData.trophyData.GetCurrentCollectedTrophy(type);
            var dataFont = GetTrophyType(crTrophy);
            mainTileTrophy.text = Locator.Main.ScreenMain.trophyTab.GetRealNameTrophy(type);
            mainTileTrophy.material = dataFont.fontMaterial;
            mainTileTrophy.colorGradientPreset = dataFont.colorGradient;
            trophyUnlockText.gameObject.SetActive(state == ItemTrophyState.New);
        }
        public void ShowInfoGroups()
        {
            infoGroups.gameObject.SetActive(true);   
            //TOdo
        }

        public TrophyCustomTitle GetTrophyType(int crTrophy)
        {
            int index = Mathf.Clamp(crTrophy,0, listFontMaterial.Count);
            return listFontMaterial[index];
        }
    }
    
    [Serializable]
    public class TrophyCustomTitle
    {
        public Material fontMaterial;
        public TMP_ColorGradient colorGradient;
    }
    [Serializable]public class PTroyphyItemData
    {
        public Image trophy;
        public Image tick;
    }
}
