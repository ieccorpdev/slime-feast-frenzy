using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class PopupProgressReward : BasePopup
    {
        public ProgressRewardItem progressRewardItemPrefab;
        [Header("Progress bar")]
        public Image currentProgressRewardIcon;
        public Image progressFillImage;
        public TextMeshProUGUI progressText;
        [Header("Timer")] 
        public TextMeshProUGUI resetTimerText;
        
        [Space(10)]
        public Transform rewardItemContainer;
        public List<ProgressRewardItem> listProgressRewardItems = new();
        
        public ProgressRewardItem CurrentRewardItem { get; private set; }
        
        public override void Show()
        {
            base.Show();
            Setup();
            UpdateProgressBar();
        }

        private void Update()
        {
            UpdateTimerText();
        }

        public void Setup()
        {
            for (int i = 0; i < Config.progressRewardConfig.listRewardData.Count; i++)
            {
                if (i >= listProgressRewardItems.Count)
                {
                    listProgressRewardItems.Add(Instantiate(progressRewardItemPrefab, rewardItemContainer));
                }
                
                listProgressRewardItems[i].Setup(i + 1, Config.progressRewardConfig.GetRewardData(i));

                listProgressRewardItems[i].SetRewardClaimed(i < PlayerData.currentProgressMilestone);

                if (i == PlayerData.currentProgressMilestone)
                {
                    CurrentRewardItem = listProgressRewardItems[i];
                }
                
            }
        }

        private void UpdateProgressBar()
        {
            currentProgressRewardIcon.sprite = CurrentRewardItem.icon.sprite;

            progressFillImage.fillAmount = (float)PlayerData.currentProgress / CurrentRewardItem.ProgressRewardData.progress;
            progressText.text = $"{PlayerData.currentProgress} / {CurrentRewardItem.ProgressRewardData.progress}";
        }

        private void UpdateTimerText()
        {
            resetTimerText.text = PlayerData.GetTimeToResetProgressReward();
        }
    }
}
