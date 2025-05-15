using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using IEC;
using UnityEngine.Serialization;

namespace _IEC
{
    public class DailyRewardProgressBar : _BaseBehaviour
    {
        public Image progressFillImage;
        public GameObject[] progressNodes;

        public void UpdateProgressBar()
        {
            UpdateRewardNodes();
            UpdateProgressFillBar();
        }

        public void UpdateRewardNodes()
        {
            for (int i = 0; i < Locator.Main.PopupDailyReward.dailyReward.Length; i++)
            {
                progressNodes[i].transform.GetChild(0).gameObject.SetActive(PlayerData.dailyRewardsClaimed.ContainsKey(i) && PlayerData.dailyRewardsClaimed[i]);
            }
        }

        public void UpdateProgressFillBar()
        {
            progressFillImage.fillAmount = (float)(PlayerData.currentDailyAdReward - 1) / (progressNodes.Length - 1);
        }
    }
}
