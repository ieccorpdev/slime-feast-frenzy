using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class PopupAddHelper : BasePopup
    {
        public Image helperIcon;

        public Button buyGoldButton;
        public Button freeAdsButton;

        public TextMeshProUGUI helperDescription;
        public TextMeshProUGUI goldAddAmountText;
        public TextMeshProUGUI goldAmountText;
        public TextMeshProUGUI freeAdsAddAmountText;

        [Header("Free Ads Count")] 
        public List<Image> listAvailableAdsImage = new();
        public Sprite availableAdsSprite;
        public Sprite unavailableAdsSprite;
        
        
        public ResourceType HelperType { get; private set; }
        
        public AddHelperData AddHelperData { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            
            SetEvent(buyGoldButton,OnBuyGold);
            SetEvent(freeAdsButton, OnFreeAds);
        }

        public void SetData(ResourceType helperType)
        {
            HelperType = helperType;

            AddHelperData = Config.popupAddHelperConfig.GetAddHelperData(HelperType);

            helperIcon.sprite = AddHelperData.helperIcon;
            helperIcon.SetNativeSize();
            
            helperDescription.text = AddHelperData.helperDescription;
            goldAmountText.text = $"{AddHelperData.goldCost}";
            goldAddAmountText.text = $"x{AddHelperData.addGoldAmount}";
            freeAdsAddAmountText.text = $"Get x{AddHelperData.addFreeAdsAmount}";
            
            SetDataAdsReward();
        }

        private void SetDataAdsReward()
        {
            if (PlayerData.lastTimeResetAdsRewardHelper != DateTime.Today)
            {
                PlayerData.ResetAdsRewardHelper();
            }

            int usedReward = PlayerData.GetUsedAdRewardAddHelper(HelperType);

            if (usedReward == AddHelperData.freeAdsLimitPerDay)
            {
                freeAdsButton.gameObject.SetActive(false);
                return;
            }
            
            freeAdsButton.gameObject.SetActive(true);

            for (int i = 0; i < AddHelperData.freeAdsLimitPerDay; i++)
            {
                if (i >= listAvailableAdsImage.Count)
                {
                    listAvailableAdsImage.Add(Instantiate(listAvailableAdsImage[0], listAvailableAdsImage[0].transform.parent.transform));
                }
                
                listAvailableAdsImage[i].gameObject.SetActive(false);
            }
            

            for (int i = 0; i < AddHelperData.freeAdsLimitPerDay; i++)
            {
                listAvailableAdsImage[i].gameObject.SetActive(true);
                if (i < AddHelperData.freeAdsLimitPerDay - usedReward)
                {
                    listAvailableAdsImage[i].sprite = availableAdsSprite;
                }
                else
                {
                    listAvailableAdsImage[i].sprite = unavailableAdsSprite;
                }
            }

        }

        public override void Show()
        {
            if (!Game.isUsePopupRefillHelper)
                return;
            
            base.Show();
        }
 
        private void OnBuyGold()
        {
            if (PlayerData.CanSpendCurrency(ResourceType.Gold, AddHelperData.goldCost))
            {
                PlayerData.AddCurrency(HelperType, AddHelperData.addGoldAmount);
                PlayerData.SubtractCurrency(ResourceType.Gold, AddHelperData.goldCost);
                Hide();
            }
            else
            {
                Locator.Main.PopupGoldOffer.Show();
            }
        }

        private void OnFreeAds()
        {
            Services.ShowRewardAds(
                () =>
                {
                    Services.PushEvent(new IEC.TrackingEvent.SRApostition($"Popup_Refill_Helper_{HelperType}"));   
                    PlayerData.UseAdRewardAddHelper(HelperType);
                    PlayerData.AddCurrency(HelperType, AddHelperData.addFreeAdsAmount);
                    Hide();
                });
        }
        
    }
}
