using System;
using System.Collections.Generic;
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
        public Button spinAdsBtn,spinFreeBtn,spinGoldBtn, backBtn,backBtn2;
        public TextMeshProUGUI availableTMP;
        public TextMeshProUGUI goldSpinCostTMP;
        public SpineAnimOT mascotLuckyWheel;
        // [Header("RewardPopup")]
        // public ClaimBonusRewardPanel rewardClaimPanel;
        public WheelPiece curentResult;
        private bool canClaim;
        private int goldCost => Config.dataRewardLuckyWheelConfig.GoldCostToSpin(PlayerData.luckWheelGoldTimeSpin);
        
        private List<Reward> listDataReward = new List<Reward>();
        
        protected override void Awake()
        {
            base.Awake();
            AudioEventOpen = EventList.Audio.LUCKY_WHEEL_POPUP_OPEN;
        }

        private void Start()
        {
            mainWheel.OnSpinEnd(OnWheelSpinEnd);
            SetEvent(backBtn,OnNoThanksButtonCLick);
            SetEvent(backBtn2,OnNoThanksButtonCLick);
            SetEvent(spinAdsBtn,OnSpinAdsButtonCLick);
            SetEvent(spinFreeBtn,OnSpinFreeButtonCLick);
            SetEvent(spinGoldBtn,OnSpinGoldButtonCLick);
        }
        
        void OnSpinAdsButtonCLick()
        {
            Services.ShowRewardAds(()=>
            {
                Services.PushEvent(new IEC.TrackingEvent.SRApostition("LuckyWheel_Spin"));   
                PlayerData.luckWheelDailyAdsSpin += 1;
                SpinWheel();
            });
        }
        void OnSpinFreeButtonCLick()
        {
            PlayerData.luckWheelDailyFreeSpin = false;
            PlayerData.firstSpinTutorial = true;
            SpinWheel();
            Services.PushEvent(new IEC.TrackingEvent.ClickBtnTracking("lucky_wheel_spin_free"));
        }
        void OnSpinGoldButtonCLick()
        {
            if (PlayerData.CanSpendCurrency(ResourceType.Gold, goldCost))
            {
                if (PlayerData.SubtractCurrency(ResourceType.Gold, goldCost))
                {
                    Services.PushEvent(new IEC.TrackingEvent.ClickSpinLuckyWheel(goldCost.ToString()));//Need push before count spin times;
                    PlayerData.luckWheelGoldTimeSpin += 1;
                    SpinWheel();
                }
            }
            else
            {
                Locator.Main.PopupGoldOffer.Show();
            }

        }
        void OnNoThanksButtonCLick()
        {
            this.Hide();
        }

        public void SpinWheel(int debugSpinNum = -1)
        {
            mascotLuckyWheel.SetAnimation("2-Capy_Spin",false, () =>
            {
                mascotLuckyWheel.SetAnimation("3-Capy_After_Spin", true);
            });
            mainWheel.Spin(debugSpinNum);
            DeactiveAllSpinButton();
            SetStatusBackButton(false);
        }
        public override void Hide()
        {
            base.Hide();
            
            Locator.Main.ScreenMain.DoAnimAllAvailableReward();
            
            EventController.RaiseEvent(EventList.GamePlay.SET_UNMUTE_BACKGROUND_MUSIC_HOME);
        }

        void OnWheelSpinEnd(WheelPiece result)
        {
            curentResult = result;
            listDataReward.Add(result.dataReward.dataReward);
            Locator.Main.ClaimBonusRewardPanel.SetupRewardPanel(listDataReward,CloseComplete,false);
            TweenControl.GetInstance().DelayCall(transform,0.2f, () =>
            {
                Locator.Main.ClaimBonusRewardPanel.ShowingRewardPanel();
                canClaim = true;
                LoadData();
            });
            Locator.Main.HomeScreenTab.UpdateMainHome();
            
        }

        public void CloseComplete()
        {
            
            LoadData();
            SetupUI();

            foreach (var reward in listDataReward)
            {
                if(reward.typeReward == ResourceType.Gold)
                    continue;
                
                Locator.Main.ScreenMain.AddRewardClaimEffect(reward.typeReward, reward.rewardAmount);
                
                // if(reward.typeReward == ResourceType.Gold)
                //     Locator.Main.ScreenMain.DoAnimLatestAvailableReward();
            }
            
            // if (listDataReward.Find(reward => reward.typeReward == ResourceType.UnlimitedLive) != null)
            // {
            //     Hide();
            //     return;
            // }
            
            listDataReward.Clear();
        }
        public override void Show() 
        {
            base.Show();
            LoadData();
            SetupUI();
            EventController.RaiseEvent(EventList.GamePlay.SET_MUTE_BACKGROUND_MUSIC);
        }
        public void LoadData()
        {
            UpdateConfigRewardLuckyWheel();
            mainWheel.dataWheel = Config.dataRewardLuckyWheelConfig;
            mainWheel.SetupWheel();
            mainWheel.LoadDataWheel();
        }

        private void UpdateConfigRewardLuckyWheel()
        {
            if (!PlayerData.firstSpinTutorial)
            {
                if(Game.isOverrideDataRewardLuckyWheel)
                    mainWheel.listRewardLuckyWheel = Config.dataRewardLuckyWheelConfig.listRewardTutorialOverrideABTest;
                else
                    mainWheel.listRewardLuckyWheel = Config.dataRewardLuckyWheelConfig.listRewardTutorial;
            }
            else
            {
                if(Game.isOverrideDataRewardLuckyWheel)
                    mainWheel.listRewardLuckyWheel = Config.dataRewardLuckyWheelConfig.listRewardOverrideABTest;
                else
                    mainWheel.listRewardLuckyWheel = Config.dataRewardLuckyWheelConfig.listRewardLuckyWheel;
            }
        }

        void SetupUI()
        {
            mainPanel.gameObject.SetActive(true);
            mainWheel.gameObject.SetActive(true);
            //rewardClaimPanel.gameObject.SetActive(false);
            //availableTMP.text = "AVAILABLE:" + PlayerData.numberSpinWheelAdsLeft + "/2";
            SetupStatusButton();
            mascotLuckyWheel.SetAnimation("1-Capy_Idle",true);
            EventController.RaiseEvent(EventList.GamePlay.UPDATE_RESOURCE_UI);
        }

        public void SetStatusBackButton(bool stt)
        {
            backBtn.gameObject.SetActive(stt);
            backBtn2.gameObject.SetActive(stt);
        }
        void SetupStatusButton()
        {
            SetStatusBackButton(PlayerData.firstSpinTutorial);
            DeactiveAllSpinButton();
            if (PlayerData.luckWheelDailyFreeSpin || !PlayerData.firstSpinTutorial)
            {
                spinFreeBtn.gameObject.SetActive(true);
            }
            else
            {
                spinAdsBtn.gameObject.SetActive(PlayerData.luckWheelDailyAdsSpin < Config.dataRewardLuckyWheelConfig.numberSpinAdsEachDay);
                spinGoldBtn.gameObject.SetActive(true);
                SetSpinGoldStatus();
            }
        }

        public void SetSpinGoldStatus()
        {
            goldSpinCostTMP.text = goldCost.ToString();

        }
        public void DeactiveAllSpinButton()
        {
            spinAdsBtn.gameObject.SetActive(false);
            spinFreeBtn.gameObject.SetActive(false);
            spinGoldBtn.gameObject.SetActive(false);
        }


    }
}
