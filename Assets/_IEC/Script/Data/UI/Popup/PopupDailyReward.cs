using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IEC;
using TMPro;

namespace _IEC
{
    public class PopupDailyReward : BasePopup
    {
        //public DailyRewardProgressBar progressBar;
        public RewardButton[] dailyReward;

        public TextMeshProUGUI dailyCountdownText;

        public int CurrentDayReward { get; private set; } = 0;

        public int CurrentGoldClaimed { get; private set; } = 0;

        private Dictionary<ResourceType, int> listRewardByType = new();

        public bool RewardAvailable
        {
            get
            {
                // for (int i = 0; i < dailyReward.Length; i++)
                // {
                //     if (!PlayerData.dailyRewardsClaimed.ContainsKey(i))
                //         return true;
                // }
                // return false;

                return !PlayerData.dailyRewardsClaimed[PlayerData.currentDailyAdReward];
            }
        }
        private void OnEnable()
        {
            foreach (var btn in dailyReward)
            {
                btn.onClaim += OnRewardClaim;
            }

        }

        private void OnDisable()
        {
            foreach (var btn in dailyReward)
            {
                btn.onClaim -= OnRewardClaim;
            }
            
        }

        private void Update()
        {
            if (!IsShow)
                return;
            //UpdateDailyRewardCountdown();
        }

        public override void Show()
        {
            base.Show();
            PlayerData.CheckResetDailyReward();
            UpdateUI();

            CurrentGoldClaimed = 0;
        }

        public override void Hide()
        {
            base.Hide();
            //UpdatePlayerGold();
            Locator.Main.HomeScreenTab.UpdateMainHome();
            //SpawnClaimRewardEffect();
        }

        private void SpawnClaimRewardEffect()
        {
            foreach (var reward in listRewardByType)
            {
                Locator.Main.ScreenMain.AddRewardClaimEffect(reward.Key, reward.Value);
            }
            Locator.Main.ScreenMain.DoAnimAllAvailableReward();
            listRewardByType.Clear();
        }


        void UpdateUI()
        {
            foreach (var btn in dailyReward)
            {
                btn.ResetButton();
            }
            CurrentDayReward = PlayerData.currentDailyAdReward;
            UpdateRewards();
            //progressBar.UpdateProgressBar();
        }

        void UpdateRewards()
        {
            for (int i = 0; i < dailyReward.Length; i++)
            {
                //multiple reward
                dailyReward[i].id = (i+1);
                dailyReward[i].SetDataButton(Config.dataDailyRewardConfig.dailyRewards[i].rewards);
                if (PlayerData.dailyRewardsClaimed[i] && i <= CurrentDayReward)
                {
                    dailyReward[i].SetButtonState(RewardButtonState.Claimed);
                    continue;
                }
                
                if (i <= CurrentDayReward && CanClaimReward())
                {
                    dailyReward[i].SetButtonState(RewardButtonState.Available);
                }
                else
                {
                    dailyReward[i].SetButtonState(RewardButtonState.Unavailable);
                }
            }
        }
        void OnRewardClaim(RewardButton btn)
        {
            PlayerData.dailyRewardsClaimed[CurrentDayReward] = true;
            CurrentDayReward++;
            PlayerData.currentDailyAdReward = CurrentDayReward;
            PlayerData.lastTimeOpenDailyReward = DateTime.Now;
            UpdateRewards();
            EventController.RaiseEvent(EventList.SAVE_DATA);
        }
        void UpdateDailyRewardCountdown()
        {
            string endTime = "";
            PlayerData.GetEndTimeDay(out endTime);
            dailyCountdownText.text = $"Reset in " + endTime;
        }

        public bool CanClaimReward()
        {
            // next day or next moth can claim
            return PlayerData.lastTimeOpenDailyReward.Day != DateTime.Now.Day 
                   || PlayerData.lastTimeOpenDailyReward.Month != DateTime.Now.Month
                   || PlayerData.lastTimeOpenDailyReward.Year != DateTime.Now.Year;
        }

    }
    
}
