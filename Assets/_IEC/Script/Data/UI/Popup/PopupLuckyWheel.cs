using System;
using DG.Tweening;
using EasyUI.PickerWheelUI;
using IEC;
using IEC.TrackingEvent;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _IEC
{
    public class PopupLuckyWheel : BasePopup
    {
        [Header("Base Spin")]
        public PickerWheel mainWheel;
        public Button spinAdsBtn, noThankBtn;
        public TextMeshProUGUI availableTMP;
        [Header("RewardPopup")]
        public GameObject rewardClaimanel;

        public Button closeRewardBtn;
        public Button claimRewardBtn;
        public Image iconReward;
        public TextMeshProUGUI valueReward;
        public WheelPiece curentResult;
        private bool canClaim;
        protected override void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            mainWheel.OnSpinEnd(OnWheelSpinEnd);
            SetEvent(noThankBtn,OnNoThanksButtonCLick);
            SetEvent(spinAdsBtn,OnSpinButtonCLick);
            SetEvent(closeRewardBtn,OnNoThanksButtonCLick);
            SetEvent(claimRewardBtn,ClaimReward);
        }
        
        void OnSpinButtonCLick()
        {
            Services.ShowRewardAds(() =>
            {
                mainWheel.Spin();
                PlayerData.numberSpinWheelAdsLeft--;
                spinAdsBtn.gameObject.SetActive(false);
                noThankBtn.gameObject.SetActive(false);
                availableTMP.gameObject.SetActive(false);
            }, null);
        }
        void OnNoThanksButtonCLick()
        {
            this.Hide();
        }
       
        public override void Hide()
        {
            base.Hide();
        }

        void OnWheelSpinEnd(WheelPiece result)
        {
            //SetupReward
            iconReward.sprite = result.dataReward.icon;
            iconReward.SetNativeSize();
            iconReward.gameObject.transform.localScale = Vector3.one * 1.7f;
            if(result.dataReward.dataReward.typeReward == ResourceType.UnlimitedLive)
            {
                valueReward.text = result.dataReward.dataReward.rewardAmount + "m";
            }
            else
            {
                valueReward.text = "x" + result.dataReward.dataReward.rewardAmount;
            }
            curentResult = result; 
            TweenControl.GetInstance().DelayCall(transform,1.5f, () =>
            {
                mainWheel.gameObject.SetActive(false);
                rewardClaimanel.gameObject.SetActive(true);
                canClaim = true;
            });

        }

        void ClaimReward()  
        {
            if (!canClaim)
            {
                return;
            }
            canClaim = false;
            var itr = curentResult.dataReward.dataReward;
            switch (itr.typeReward)
            {
                case ResourceType.Gold:
                    PlayerData.AddGold(itr.rewardAmount);
                    break;
                case ResourceType.UnlimitedLive:
                    //PlayerData.AddTimeUnlimitedLife(itr.rewardAmount);
                    break;
            }
            this.Hide();
        }
        public override void Show()
        {
            base.Show();
            LoadData();
            SetupUI();
        }
        public void LoadData()
        {
            mainWheel.dataWheel = Config.dataRewardLuckyWheelConfig;
            mainWheel.SetupWheel();
        }
        void SetupUI()
        {
            mainPanel.gameObject.SetActive(true);
            mainWheel.gameObject.SetActive(true);
            rewardClaimanel.gameObject.SetActive(false);
            availableTMP.text = "AVAILABLE:" + PlayerData.numberSpinWheelAdsLeft + "/2";

            SetupStatusButton();
        }

        void SetupStatusButton()
        {
            spinAdsBtn.gameObject.SetActive(true);
            noThankBtn.gameObject.SetActive(true);
            availableTMP.gameObject.SetActive(true);
            spinAdsBtn.interactable = PlayerData.numberSpinWheelAdsLeft > 0;
        }
        
    }
}
