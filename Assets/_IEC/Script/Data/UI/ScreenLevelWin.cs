using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class ScreenLevelWin : ScreenUI
    {
        public TextMeshProUGUI goldRewardText;
        public BonusRewardWheel bonusRewardWheel;
        public Button btnBonusReward;
        public Button btnNext;

        private int _goldRewardAmount = 10;
        [Header("Medal Leaderboard")]
        public GameObject medalObj;
        public Image medalImage;
        public TextMeshProUGUI txtMedalAmount;
        private void Start()
        {
            listButtonHideOnClick.Add(btnNext);

            SetEvent(btnNext, () =>
            {
                Services.PushEvent(new TrackingEvent.ClickBtnNext());
                //EventController.RaiseEvent(EventList.GamePlay.END_LEVEL_ADS);
                OnNoThanksButtonClick();
            });
            
            SetEvent(btnBonusReward,
                () =>
                {
                    Services.PushEvent(new TrackingEvent.ClickBtnTracking("level_win_bonus_reward"));
                    OnAdsGetx3Gold();
                });
        }

        public override void Show()
        {
            base.Show();
            bonusRewardWheel.InitWheel();
        }

        private void Update()
        {
            goldRewardText.text = bonusRewardWheel.GetFinalRewardCount(_goldRewardAmount).ToString();
        }
        
        void OnAdsGetx3Gold()
        {
            bonusRewardWheel.StopWheel();
            goldRewardText.text = bonusRewardWheel.GetFinalRewardCount(_goldRewardAmount).ToString();
            Services.ShowRewardAds(() =>
            {
                //EventController.RaiseEvent(EventList.GamePlay.ADD_GOLD, new EventParam(this, finalCoinBonus, new Vector2(-280, -1390)));
                TweenControl.GetInstance().DelayCall(transform, 0.5f, () =>
                {
                    Hide();
                    ActionCompletePopup();
                }, () =>
                {
                    Services.PushEvent(new IEC.TrackingEvent.SRApostition($"ad_win_bonus_reward"));
                });
            }, null, true);
           
        }
        
        void OnNoThanksButtonClick()
        {
            //EventController.RaiseEvent(EventList.GamePlay.ADD_GOLD, new EventParam(this, endLevelGoldGet, new Vector2(-280, -1390)));
  
            //+1 because when win level add 1
            Services.ShowFullAdsWithTime(Game.timeDelayFullAds, () =>
            {
                bonusRewardWheel.StopWheel();
                Locator.Main.PopupPiggyBank.AddGoldToPiggyBank(_goldRewardAmount);//CustomGold here
                Hide();
                ActionCompletePopup();
            }, () =>
            {
                Services.PushEvent(new IEC.TrackingEvent.SFApostition($"skip_win_bonus_reward"));
            });
        }
        
        private void ActionCompletePopup()
        {
            //Add reward //
            EventController.RaiseEvent(EventList.GamePlay.GO_MAIN);

        }
        public void SetMedalReward(int medal)
        {
            medalObj.gameObject.SetActive(Game.isHaveLeaderboard);

            if (!Game.isHaveLeaderboard) return;

            medalImage.sprite = LeaderboardConfig.GetSpirteCurrentLeague();
            txtMedalAmount.text = "" + medal;
        }
    }
}