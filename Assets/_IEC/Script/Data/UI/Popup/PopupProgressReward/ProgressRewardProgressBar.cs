using System;
using System.Collections;
using System.Collections.Generic;
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
        public Image progressFillImage;
        public TextMeshProUGUI progressText;
        [Header("Timer")] public TextMeshProUGUI resetTimerText;
        
        public ProgressRewardData ProgressRewardData { get; private set; }

        public void Setup()
        {
            ProgressRewardData = PlayConfig.progressRewardConfig.GetRewardData(PlayerData.currentProgressMilestone);
            UpdateProgressBar();
        }

        private void Update()
        {
            UpdateTimerText();
        }

        private void UpdateProgressBar()
        {
            currentSpecialObjectItem.sprite = PlayConfig.specialObjectConfig
                .GetSpecialItemDefinition(PlayerData.currentSpecialItemIndex).GetIcon();
            currentProgressRewardIcon.sprite = PlayConfig.progressRewardConfig.GetRewardSprite(ProgressRewardData.reward.typeReward);
            currentProgressRewardIcon.SetNativeSize();
            progressRewardAmountText.text = ProgressRewardData.GetDisplayText();
            progressFillImage.fillAmount = (float)PlayerData.currentProgress / ProgressRewardData.progress;
            progressText.text = $"{PlayerData.currentProgress}/{ProgressRewardData.progress}";
        }

        private void UpdateTimerText()
        {
            if (resetTimerText == null)
                return;
            
            resetTimerText.text = PlayerData.GetTimeToResetProgressReward();
        }
    }
}
