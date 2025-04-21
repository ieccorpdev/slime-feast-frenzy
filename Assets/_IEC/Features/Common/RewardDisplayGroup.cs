using System;
using IEC;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class RewardDisplayGroup : MonoBehaviour
    {
        public RewardTypes rewardType;
        public Image icon;
        public TextMeshProUGUI txtAmount;
         public bool isHaveData;

        public void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }
        
        public void SetReward(Reward reward, bool isSetActive = true)
        {
            isHaveData = true;
            if (rewardType == RewardTypes.InfiniteLife)
            {
                txtAmount.text = Reward.GetInifiniteLivesString(reward);
            }
            else
            {
                txtAmount.text = $"x{reward.rewardAmount}";
            }
            
            if (isSetActive)
                SetActive(true);
        }

        public void SetIcon(Sprite sprite, bool isSetNativeSize = false)
        {
            icon.sprite = sprite;
            if (isSetNativeSize)
                icon.SetNativeSize();
        }

        public void ResetData()
        {
            isHaveData = false;
        }
    }
}