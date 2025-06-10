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
        public Image infoGRBG;
        public GameObject eventTimeObj;
        public TextMeshProUGUI timeEventTMP;
        public TextMeshProUGUI infoTrophyTMP;
        public List<PTroyphyItemData> listSpriteTrophy;
        [Header("Unlocked Groups")]
        public GameObject unlockedGroups;
        public Image badgeImage;
        public List<Sprite> ListBadgeSprites;
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
                ShowCompleteGroups();
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
            mainTileTrophy.fontMaterial  = dataFont.fontMaterial;
            mainTileTrophy.colorGradientPreset = dataFont.colorGradient;
            trophyUnlockText.gameObject.SetActive(state == ItemTrophyState.New);
        }
        public void ShowInfoGroups()
        {
            infoGroups.gameObject.SetActive(true);   
            int currentTrophy = PlayerData.trophyData.GetCurrentCollectedTrophy(type);
            int index = Mathf.Clamp(currentTrophy,0, ListBadgeSprites.Count-1);
            for (int i = 0; i < listSpriteTrophy.Count; i++)
            {
                listSpriteTrophy[i].tick.gameObject.SetActive(i < index);
            }

            switch (type)
            {
                case TrophyType.GrandVoyage:
                case TrophyType.SharkFrenzyForce:
                    infoGRBG.sprite = defaultBg;
                    eventTimeObj.gameObject.SetActive(false);
                    break;
                default:
                    //Then load time
                    infoGRBG.sprite = eventTimeBg;
                    break;
            }
        }

        public void ShowCompleteGroups()
        {
            unlockedGroups.gameObject.SetActive(true);
            int currentTrophy = PlayerData.trophyData.GetCurrentCollectedTrophy(type);
            int index = Mathf.Clamp(currentTrophy,0, ListBadgeSprites.Count-1);
            badgeImage.sprite = ListBadgeSprites[index];
            trophyRoomButton.onClick.AddListener(OnTrophyRoomClick);
        }

        public void OnTrophyRoomClick()
        {
            Hide();
            Locator.Main.ScreenMain.bottomBar.GetTab(TabType.Trophy).onClick?.Invoke();
        }
        public TrophyCustomTitle GetTrophyType(int crTrophy)
        {
            int index = Mathf.Clamp(crTrophy,0, listFontMaterial.Count-1);
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
