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
        public DailyRewardProgressBar progressBar;
        public RewardButton mainReward;
        public RewardButton[] adRewards;

        public TextMeshProUGUI dailyCountdownText;

        public int CurrentAdReward { get; private set; } = 0;

        public int CurrentGoldClaimed { get; private set; } = 0;

        public bool RewardAvailable
        {
            get
            {
                for (int i = 0; i < adRewards.Length; i++)
                {
                    if (!PlayerData.adRewardsClaimed.ContainsKey(i))
                        return true;
                }

                if (PlayerData.mainRewardCooldownTickTime - DateTime.Now.Ticks <= 0)
                    return true;

                return false;
            }
        }

        private void OnEnable()
        {
            foreach (var btn in adRewards)
            {
                btn.onClaim += OnAdRewardClaim;
            }

            mainReward.onClaim += OnMainRewardClaim;
        }
        
        

        private void OnDisable()
        {
            foreach (var btn in adRewards)
            {
                btn.onClaim -= OnAdRewardClaim;
            }
            
            mainReward.onClaim -= OnMainRewardClaim;
        }

        private void Update()
        {
            if (!IsShow)
                return;
            
            UpdateDailyRewardCountdown();
        }

        void OnAdRewardClaim(RewardButton btn)
        {
            PlayerData.adRewardsClaimed[CurrentAdReward] = true;
            CurrentAdReward++;
            PlayerData.currentDailyAdReward = CurrentAdReward;
            UpdateAdRewards();
            progressBar.UpdateProgressBar();
            
            if(btn.Reward == ResourceType.Gold)
                CurrentGoldClaimed += btn.RewardAmount;
            
            EventController.RaiseEvent(EventList.SAVE_DATA);
        }

        void OnMainRewardClaim(RewardButton btn)
        {
            CurrentGoldClaimed += btn.RewardAmount;
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
            UpdatePlayerGold();
            Locator.Main.HomeScreenTab.UpdateMainHome();
            Locator.Main.HomeScreenTab.buildCityUI.ShowScale();
        }

        void UpdatePlayerGold()
        {
            Vector2 pos = new Vector2(0, -1250);
            //GoldGroup goldGroup = UI.GetGoldGroup();
            //goldGroup.EffectCoinClaim(pos, CurrentGoldClaimed, PlayerData.gold, () =>
            //{
            //    goldGroup.UpdateGold();
            //});
        }

        void UpdateUI()
        {
            CurrentAdReward = PlayerData.currentDailyAdReward;
            //mainReward.SetButtonState(RewardButtonState.Available);
            mainReward.UpdateButton(Config.dataDailyRewardConfig.mainReward);
            UpdateAdRewards();
            progressBar.UpdateProgressBar();
        }

        void UpdateAdRewards()
        {
            for (int i = 0; i < adRewards.Length; i++)
            {
                //if (adRewards[i].RewardClaimed)
                adRewards[i].UpdateButton(Config.dataDailyRewardConfig.dailyRewards[i]);
                if (PlayerData.adRewardsClaimed.ContainsKey(i) && PlayerData.adRewardsClaimed[i] && i <= CurrentAdReward)
                {
                    adRewards[i].SetButtonState(RewardButtonState.Claimed);
                    continue;
                }
                    
                if (i <= CurrentAdReward)
                {
                    adRewards[i].SetButtonState(RewardButtonState.Available);
                }
                else
                {
                    adRewards[i].SetButtonState(RewardButtonState.Unavailable);
                }
            }
        }

        void UpdateDailyRewardCountdown()
        {
            string endTime = "";
            PlayerData.GetEndTimeDay(out endTime);
            dailyCountdownText.text = $"RESETS IN " + endTime;
            
            if(PlayerData.lastTimeOpenDailyReward != DateTime.Today)
                OnResetDailyReward();
        }

        void OnResetDailyReward()
        {
            PlayerData.currentDailyAdReward = 0;
            PlayerData.adRewardsClaimed.Clear();
            PlayerData.lastTimeOpenDailyReward = DateTime.Today;
            EventController.RaiseEvent(EventList.SAVE_DATA);
            
            mainReward.ResetButton();
            foreach (var btn in adRewards)
            {
                btn.ResetButton();
            }
            
            UpdateUI();
        }
    }
    
}
