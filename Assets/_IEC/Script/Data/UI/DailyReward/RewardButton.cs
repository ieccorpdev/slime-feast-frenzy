using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using IEC;
using IEC.TrackingEvent;
using TMPro;
using UnityEngine.Serialization;

namespace _IEC
{
    public class RewardButton : _BaseBehaviour
    {
        public ButtonClaimType claimType;
        public Button ClaimButton;
        public Image rewardIcon;
        public TextMeshProUGUI amountText;
        public string id;
        [Header("Cooldown Timer")]
        public bool haveCooldown;
        public int cooldownTimeInMinutes = 60;
        public TextMeshProUGUI cooldownText;
        
        [Header("Button State Groups")]
        public GameObject availableGroup;
        public GameObject unavailableGroup;
        public GameObject claimedGroup;

        public Action<RewardButton> onClaim;
        
        public ResourceType Reward { get; private set; }
        public int RewardAmount { get; private set; }
        
        public RewardButtonState ButtonState { get; protected set; } = RewardButtonState.Available;

        public bool RewardClaimed { get; private set; } = false;

        private void Start()
        {
            ClaimButton.onClick.RemoveAllListeners();
            ClaimButton.onClick.AddListener(TryClaimReward);
        }

        public void UpdateButton(Reward rewardData)
        {
            //rewardIcon.sprite = Config.GetRewardSprite(rewardData);
            //rewardIcon.SetNativeSize();
            amountText.text = "x" + rewardData.rewardAmount.ToString();

            Reward = rewardData.typeReward;
            RewardAmount = rewardData.rewardAmount;
        }

        private void Update()
        {
            if (!Locator.Main.PopupDailyReward.IsShow)
                return;
            
            UpdateCooldownTimer();
        }

        public virtual void SetButtonState(RewardButtonState buttonState)
        {
            ButtonState = buttonState;
            
            availableGroup.SetActive(ButtonState == RewardButtonState.Available);
            unavailableGroup.SetActive(ButtonState == RewardButtonState.Unavailable);
            claimedGroup.SetActive(ButtonState == RewardButtonState.Claimed);
            
            ClaimButton.gameObject.SetActive(ButtonState != RewardButtonState.Claimed);
            ClaimButton.interactable = ButtonState == RewardButtonState.Available;

            if (!RewardClaimed)
                RewardClaimed = ButtonState == RewardButtonState.Claimed;
        }

        public virtual void ClaimReward()
        {
            HandleRewardClaim();
            SetButtonState(RewardButtonState.Claimed);

            if (haveCooldown)
            {
                PlayerData.mainRewardCooldownTickTime = DateTime.Now.AddMinutes(cooldownTimeInMinutes).Ticks;
                EventController.RaiseEvent(EventList.SAVE_DATA);
            }
            //Locator.Main.HomeScreenTab.UpdateMainHome();
            onClaim?.Invoke(this);
        }

        public void TryClaimReward()
        {
            if (claimType == ButtonClaimType.Free)
            {
                ClaimReward();
            }
            else if (claimType == ButtonClaimType.Ad)
            {
                //Services.ShowRewardAds(ClaimReward, null, true);
                
                Services.ShowRewardAds(() =>
                {
                    Services.PushEvent(new SRApostition("claimDailyRewardWithAd_"+id));
                    ClaimReward();
                }, null, true);
            }
        }

        void HandleRewardClaim()
        {
            PlayerData.AddCurrency(Reward,RewardAmount, false);
        }

        public void ResetButton()
        {
            SetButtonState(RewardButtonState.Available);
            RewardClaimed = false;
        }

        void UpdateCooldownTimer()
        {
            if (!haveCooldown || !cooldownText)
                return;

            var currentTick = PlayerData.mainRewardCooldownTickTime - DateTime.Now.Ticks;

            var cooldownTime = new TimeSpan(currentTick);
            string minutes = cooldownTime.Minutes < 10 ? $"0{cooldownTime.Minutes}" : cooldownTime.Minutes.ToString();
            string seconds = cooldownTime.Seconds < 10 ? $"0{cooldownTime.Seconds}" : cooldownTime.Seconds.ToString();
            cooldownText.text = $"{minutes}m {seconds}s";

            if (currentTick <= 0)
            {
                ResetButton();
            }
        }

    }

    public enum RewardButtonState
    {
        Available,
        Unavailable,
        Claimed
    }

    public enum ButtonClaimType
    {
        Free,
        Ad
    }
}
