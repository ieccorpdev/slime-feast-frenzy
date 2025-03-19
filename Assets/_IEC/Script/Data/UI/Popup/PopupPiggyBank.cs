using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using IEC;
using UnityEngine.PlayerLoop;

namespace IEC
{
    public class PopupPiggyBank : BasePopup
    {
        public Button buyButton;
        public Button keepPlayingButton;
        public Image progressFillBar;
        public List<Transform> listGoldMilestoneDisplay = new();
        public Image fullIcon;
        public TextMeshProUGUI fullGoldText;
        public TextMeshProUGUI guideText;
        public TextMeshProUGUI goldText;
        public BundlePackUI bundlePackUI;
        public SpineAnimOT spineAnimPig;
        public bool IsFull => PlayerData.piggyBankGold >= Config.dataPiggyBankConfig.MaxGoldCapByPiggyBank;
        
        public float PiggyBankFillAmount => (float)PlayerData.piggyBankGold / Config.dataPiggyBankConfig.MaxGoldCapByPiggyBank;

        protected override void Awake()
        {
            base.Awake();
            AudioEventOpen = EventList.Audio.PIGGY_BANK_POPUP_OPEN;
        }

        public void Start()
        {
            bundlePackUI.OnPurchaseSuccess += BuySuccessPiggyBank;
            SetEvent(keepPlayingButton,OnKeepPlayingButtonClick);
        }

        public void OnKeepPlayingButtonClick()
        {
            Hide();
        }
        public void BuySuccessPiggyBank()
        {
            PlayerData.AddCurrency(ResourceType.Gold,PlayerData.piggyBankGold, false);
            if(Locator.Main.ScreenMain) Locator.Main.ScreenMain.AddRewardClaimEffect(ResourceType.Gold,PlayerData.piggyBankGold);
            //Locator.Main.ScreenMain.DoAnimAllAvailableReward();
            Hide();
            ResetPiggyBank();
            if(Locator.Main.HomeScreenTab) Locator.Main.HomeScreenTab.UpdateMainHome();
            EventController.RaiseEvent(EventList.SAVE_DATA);
        }
        public override void Show()
        {
            base.Show();
            UpdatePopup();
            PlayerData.dailyShowPiggyBank = true;// Reset daily noti each 24h
            Locator.Main.HomeScreenTab.UpdateMainHome();
        }

        public override void Hide()
        {
            base.Hide();
        }

        public void UpdatePopup()
        {
            SetFullState();
            UpdateGoldText();
            UpdateProgressBar();
            UpdateBuyButton();
            UpdateButtonBottom();
            SetAnim();
        }

        public void SetAnim()
        {
            spineAnimPig.SetAnimation(IsFull ? "1.2_start_Full" : "1.1_start_Regular", false, () =>
            {
                spineAnimPig.SetAnimation(IsFull ? "2.2_idle_Full" : "2.1_idle_Regular", true);
            });
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

            var progressBarRect = progressFillBar.GetComponent<RectTransform>();
            
            // Get the width of the progress bar
            float barWidth = progressBarRect.rect.width;

            // Calculate min and max local X positions (relative to the progress bar)
            float minX = -barWidth * 0.5f;
            float maxX = barWidth * 0.5f;

            for (int i = 0; i < Config.dataPiggyBankConfig.listGoldMilestone.Count; i++)
            {
                if (i >= listGoldMilestoneDisplay.Count)
                {
                    listGoldMilestoneDisplay.Add(Instantiate(listGoldMilestoneDisplay[0], listGoldMilestoneDisplay[0].transform.parent));
                }

                var displayObj = listGoldMilestoneDisplay[i].GetComponent<RectTransform>();
                float newX = Mathf.Lerp(minX, maxX, 
                    (float)Config.dataPiggyBankConfig.listGoldMilestone[i] / Config.dataPiggyBankConfig.MaxGoldCapByPiggyBank);

                displayObj.localPosition = new Vector2(newX, displayObj.anchoredPosition.y);

                var amountText = displayObj.GetComponentInChildren<TextMeshProUGUI>();
                if (amountText != null)
                {
                    amountText.text = Config.dataPiggyBankConfig.listGoldMilestone[i].ToString();
                }

            }
        }

        public void UpdateButtonBottom()
        {
            bool canBuy = Config.dataPiggyBankConfig.PassMileStonePiggyBank(PlayerData.piggyBankGold);
            buyButton.gameObject.SetActive(canBuy);
            keepPlayingButton.gameObject.SetActive(!canBuy);
        }
        void UpdateBuyButton()
        {
            //buyButton.interactable = PlayerData.piggyBankGold != 0;
            buyButton.interactable = PlayerData.piggyBankGold >= Config.dataPiggyBankConfig.listGoldMilestone[0];
        }

        public void AddGoldToPiggyBank(int gold)
        {
            PlayerData.AddGoldToPiggyBank(gold);
            PlayerData.piggyBankGold = Mathf.Min(PlayerData.piggyBankGold, Config.dataPiggyBankConfig.MaxGoldCapByPiggyBank);
            EventController.RaiseEvent(EventList.SAVE_DATA);
        }

        public void ResetPiggyBank()
        {
            PlayerData.piggyBankGold = 0;
            EventController.RaiseEvent(EventList.SAVE_DATA);
        }
    }
}
