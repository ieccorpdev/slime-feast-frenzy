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
                for (int i = 0; i < dailyReward.Length; i++)
                {
                    if (!PlayerData.dailyRewardsClaimed.ContainsKey(i))
                        return true;
                }
                return false;
            }
        }


        private void Update()
        {
            if (!IsShow)
                return;
            UpdateDailyRewardCountdown();
        }

        public override void Show()
        {
            base.Show();
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
            CurrentDayReward = PlayerData.currentDailyAdReward;
            UpdateRewards();
            //progressBar.UpdateProgressBar();
        }

        void UpdateRewards()
        {
            for (int i = 0; i < dailyReward.Length; i++)
            {
                //multiple reward
                dailyReward[i].id = (i+1).ToString();
                dailyReward[i].SetDataButton(Config.dataDailyRewardConfig.dailyRewards[i].rewards);
                if (PlayerData.dailyRewardsClaimed.ContainsKey(i) && PlayerData.dailyRewardsClaimed[i] && i <= CurrentDayReward)
                {
                    dailyReward[i].SetButtonState(RewardButtonState.Claimed);
                    continue;
                }
                
                if (i <= CurrentDayReward)
                {
                    dailyReward[i].SetButtonState(RewardButtonState.Available);
                }
                else
                {
                    dailyReward[i].SetButtonState(RewardButtonState.Unavailable);
                }
            }
        }

        void UpdateDailyRewardCountdown()
        {
            string endTime = "";
            PlayerData.GetEndTimeDay(out endTime);
            dailyCountdownText.text = $"Reset in " + endTime;
            
            if(PlayerData.lastTimeOpenDailyReward != DateTime.Today)
                OnResetDailyReward();
        }

        void OnResetDailyReward()
        {
            var dayDiff = (PlayerData.lastTimeOpenDailyReward - DateTime.Today).Days;
            if (dayDiff >= Config.dataDailyRewardConfig.dailyRewards.Count)
            {
                //Out of day reward, reset 
                PlayerData.ResetDailyReward();
            }
            
            PlayerData.lastTimeOpenDailyReward = DateTime.Today;
            EventController.RaiseEvent(EventList.SAVE_DATA);
            foreach (var btn in dailyReward)
            {
                btn.ResetButton();
            }
            UpdateUI();
        }
    }
    
}
