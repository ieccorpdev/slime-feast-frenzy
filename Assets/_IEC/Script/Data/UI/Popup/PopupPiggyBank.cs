using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using IEC;
using UnityEngine.PlayerLoop;

namespace _IEC
{
    public class PopupPiggyBank : BasePopup
    {
        public Button buyButton;
        public Image progressFillBar;
        public Image fullIcon;
        public TextMeshProUGUI fullGoldText;
        public TextMeshProUGUI guideText;
        public TextMeshProUGUI goldText;
        public BundlePackUI bundlePackUI;
        public bool IsFull => PlayerData.piggyBankGold >= Config.dataPiggyBank.maxGoldCapByPiggyBank;
        
        public float PiggyBankFillAmount => (float)PlayerData.piggyBankGold / Config.dataPiggyBank.maxGoldCapByPiggyBank;

        public void Start()
        {
            bundlePackUI.OnPurchaseSuccess += BuySuccessPiggybank;
        }

        public void BuySuccessPiggybank()
        {
            PlayerData.gold += PlayerData.piggyBankGold;
            Hide();
            ResetPiggyBank();
            Locator.Main.HomeScreenTab.UpdateMainHome();
            EventController.RaiseEvent(EventList.SAVE_DATA);
        }
        public override void Show()
        {
            base.Show();
            UpdatePopup();
        }

        public void UpdatePopup()
        {
            SetFullState();
            UpdateGoldText();
            UpdateProgressBar();
            UpdateBuyButton();
        }

        void SetFullState()
        {
            //PlayerData.piggyBankGold = Mathf.Min(PlayerData.piggyBankGold, maxGoldCount);
            fullIcon.gameObject.SetActive(IsFull);
            fullGoldText.gameObject.SetActive(IsFull);
            guideText.gameObject.SetActive(!IsFull);
        }

        void UpdateGoldText()
        {
            goldText.text = PlayerData.piggyBankGold.ToString();
        }

        void UpdateProgressBar()
        {
            progressFillBar.fillAmount = PiggyBankFillAmount;
        }

        void UpdateBuyButton()
        {
            buyButton.interactable = PlayerData.piggyBankGold != 0;
        }

        public void AddGoldToPiggyBank(int gold)
        {
            PlayerData.piggyBankGold += gold;
            PlayerData.piggyBankGold = Mathf.Min(PlayerData.piggyBankGold, Config.dataPiggyBank.maxGoldCapByPiggyBank);
            EventController.RaiseEvent(EventList.SAVE_DATA);
        }

        public void ResetPiggyBank()
        {
            PlayerData.piggyBankGold = 0;
            EventController.RaiseEvent(EventList.SAVE_DATA);
        }
    }
}
