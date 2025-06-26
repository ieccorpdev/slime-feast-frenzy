using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class ProgressRewardProgressBar : _BaseBehaviour
    {
        [Header("Progress bar")] 
        public Image currentSpecialObjectItem;
        public Image currentProgressRewardIcon;
        public TextMeshProUGUI progressRewardAmountText;
        public ProgressBarFillEffect progressFillBar;
        [SerializeField] GameObject iconDoubleTag;
        [Header("Timer")] public TextMeshProUGUI resetTimerText;
        
        public ProgressRewardData ProgressRewardData { get; private set; }

        public void Setup()
        {
            ProgressRewardData = Config.progressRewardConfig.GetRewardData(PlayerData.currentProgressMilestone);
            if (PlayerData.level < Config.specialObjectConfig.GetLevelUnlockProgressReward())
            {
                gameObject.SetActive(false);
            }
            else
            {
                gameObject.SetActive(true);
                CheckReward();
                UpdateProgressBar();
                SetAtiveIcon();
            }

        }

        private void SetAtiveIcon()
        {
            bool isOnDouble = PlayerData.IsUnlimitedBooster(ResourceType.DoubleSpecialObjectCount);
            //MeoMew need call duy fix here
            if(iconDoubleTag) iconDoubleTag.gameObject.SetActive(isOnDouble);
        }

        private void CheckReward()
        {
            var reward = Config.progressRewardConfig.GetRewardData(PlayerData.currentProgressMilestone);
            if (PlayerData.CheckClaimProgressReward(reward))
            {
                System.Collections.Generic.List<Reward> listReward = new() {reward.reward};
                Locator.Main.ClaimBonusRewardPanel.SetupRewardPanel(listReward, ()=>
                {
                    CheckShowTrophy();
                },true);
                Locator.Main.ClaimBonusRewardPanel.ShowingRewardPanel(ClaimBonusRewardSkin.Chest_1);
                reward = Config.progressRewardConfig.GetRewardData(PlayerData.currentProgressMilestone);
            }
            Locator.Main.HomeScreenTab.progressRewardBar.DoAnimProgressBar();
        }

        public void CheckShowTrophy()
        {
            if (PlayerData.currentProgressMilestone >= Config.progressRewardConfig.listRewardData.Count)
            {
                Locator.Main.HomeScreenTab.CheckShowTrophyPopup();
                Locator.Main.HomeScreenTab.ShowNextPopup();
            }
        }

        private void Update()
        {
            UpdateTimerText();
        }

        private void UpdateProgressBar()
        {
            currentSpecialObjectItem.sprite = Config.specialObjectConfig
                .GetSpecialItemDefinition(PlayerData.currentSpecialItemIndex).GetIcon();
            currentProgressRewardIcon.sprite = Config.progressRewardConfig.GetRewardSprite(ProgressRewardData.reward.typeReward);
            //currentProgressRewardIcon.SetNativeSize();
            progressRewardAmountText.text = ProgressRewardData.GetDisplayText();
            
            progressFillBar.SetProgressText($"{PlayerData.currentProgress}/{ProgressRewardData.progress}");
        }

        public void DoAnimProgressBar()
        {
            progressFillBar.DoFillAnim((float)PlayerData.currentProgress / ProgressRewardData.progress);
        }

        // public void SetProgressBarFill(float fillAmount)
        // {
        //     
        // }

        private void UpdateTimerText()
        {
            if (resetTimerText == null)
                return;
            
            resetTimerText.text = PlayerData.GetTimeToResetProgressReward();
        }
    }
}
