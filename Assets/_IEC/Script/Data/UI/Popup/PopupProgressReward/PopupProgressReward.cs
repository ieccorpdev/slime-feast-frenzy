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
        public ProgressRewardProgressBar progressBar;
        
        [Space(10)]
        public Transform rewardItemContainer;
        public List<ProgressRewardItem> listProgressRewardItems = new();
        
        public ProgressRewardItem CurrentRewardItem { get; private set; }
        
        public override void Show()
        {
            base.Show();
            Setup();
        }

        public void Setup()
        {
            for (int i = 0; i < PlayConfig.progressRewardConfig.listRewardData.Count; i++)
            {
                if (i >= listProgressRewardItems.Count)
                {
                    listProgressRewardItems.Add(Instantiate(progressRewardItemPrefab, rewardItemContainer));
                }
                
                listProgressRewardItems[i].Setup(i + 1, PlayConfig.progressRewardConfig.GetRewardData(i));

                listProgressRewardItems[i].SetRewardClaimed(i < PlayerData.currentProgressMilestone);

                if (i == PlayerData.currentProgressMilestone)
                {
                    CurrentRewardItem = listProgressRewardItems[i];
                }
                
            }
            
            progressBar.Setup();
            
            listProgressRewardItems[0].SetBotItem();
            listProgressRewardItems[^1].SetTopItem();
        }
    }
}
