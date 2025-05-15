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
        public Button ClaimButton;
        public Image rewardIcon;
        public Image bg;
        public TextMeshProUGUI dayText;
        public string id;



        public Action<RewardButton> onClaim;
        
        public List<Reward> rewards;

        public RewardButtonState dailyRewardState { get; protected set; } = RewardButtonState.Available;

        public bool RewardClaimed { get; private set; } = false;
        public Sprite iconClaimable;
        public Sprite iconClaimed;

        public Sprite bgClaimable;
        public Sprite bgNormal;
        [Header("Button State Groups")]
        public GameObject availableGroup;
        public GameObject unavailableGroup;
        private void Start()
        {
            ClaimButton.onClick.RemoveAllListeners();
            ClaimButton.onClick.AddListener(ClaimReward);
        }

        public void SetDataButton( List<Reward> rewardData)
        {
            dayText.text = "Day " + id;
            this.rewards = rewardData;
        }

        public virtual void SetButtonState(RewardButtonState buttonState)
        {
            dailyRewardState = buttonState;
            
            availableGroup.SetActive(dailyRewardState == RewardButtonState.Available);
            unavailableGroup.SetActive(dailyRewardState == RewardButtonState.Unavailable);

            ClaimButton.gameObject.SetActive(dailyRewardState != RewardButtonState.Claimed);
            ClaimButton.interactable = dailyRewardState == RewardButtonState.Available;

            if (!RewardClaimed)
                RewardClaimed = dailyRewardState == RewardButtonState.Claimed;
            
            bg.sprite = dailyRewardState == RewardButtonState.Claimed ? bgNormal: bgClaimable;
            bg.SetNativeSize();
            rewardIcon.sprite = dailyRewardState == RewardButtonState.Claimed ? iconClaimed: iconClaimable;
            rewardIcon.SetNativeSize();
        }

        public virtual void ClaimReward()
        {
            HandleRewardClaim();
            SetButtonState(RewardButtonState.Claimed);

            //Locator.Main.HomeScreenTab.UpdateMainHome();
            onClaim?.Invoke(this);
        }


        void HandleRewardClaim()
        {
            foreach (var reward in rewards)
            {
                PlayerData.AddCurrency(reward.typeReward,reward.rewardAmount, false);
            }
        }

        public void ResetButton()
        {
            SetButtonState(RewardButtonState.Available);
            RewardClaimed = false;
        }
        

    }

    public enum RewardButtonState
    {
        Available,
        Unavailable,
        Claimed
    }

}
