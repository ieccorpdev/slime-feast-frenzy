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

        private Dictionary<ResourceType, int> listRewardByType = new();

        public bool RewardAvailable
        {
            get
            {
                for (int i = 0; i < adRewards.Length; i++)
                {
                    if (!PlayerData.adRewardsClaimed.ContainsKey(i))
                        return true;
                }
                return !PlayerData.claimMainReward;
                //if (PlayerData.mainRewardCooldownTickTime - DateTime.Now.Ticks <= 0)
                //    return true;
                ///return false;
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

            if (!listRewardByType.TryAdd(btn.Reward, btn.RewardAmount))
            {
                listRewardByType[btn.Reward] += btn.RewardAmount;
            }
            
            EventController.RaiseEvent(EventList.SAVE_DATA);
        }

        void OnMainRewardClaim(RewardButton btn)
        {
            PlayerData.claimMainReward = true;
            if (!listRewardByType.TryAdd(btn.Reward, btn.RewardAmount))
            {
                listRewardByType[btn.Reward] += btn.RewardAmount;
            }
            EventController.RaiseEvent(EventList.SAVE_DATA);
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

            SpawnClaimRewardEffect();
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
                adRewards[i].id = (i+1).ToString();
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
            mainReward.SetButtonState(PlayerData.claimMainReward ? RewardButtonState.Claimed : RewardButtonState.Available);
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
            PlayerData.currentDailyAdReward = 0;
            PlayerData.adRewardsClaimed.Clear();
            PlayerData.claimMainReward = false;
            
            PlayerData.lastTimeOpenDailyReward = DateTime.Today;
            EventController.RaiseEvent(EventList.SAVE_DATA);
            
            mainReward.ResetButton();
            foreach (var btn in adRewards)
            {
                btn.ResetButton();
            }
            mainReward.ResetButton();
            UpdateUI();
        }
    }
    
}
