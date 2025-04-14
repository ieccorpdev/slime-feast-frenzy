using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class ProgressRewardItem : _BaseBehaviour
    {
        public Image icon;
        public TextMeshProUGUI indexText;
        public TextMeshProUGUI amountText;
        public GameObject lockGameObj;
        public GameObject claimedGameObj;

        public GameObject topLineImage, botLineImage;
        
        public ProgressRewardData ProgressRewardData { get; private set; }
        
        public void Setup(int index, ProgressRewardData data)
        {
            ProgressRewardData = data;
            
            indexText.text = index.ToString();

            icon.sprite = Config.progressRewardConfig.GetRewardSprite(data.reward.typeReward);
            amountText.text = data.reward.rewardAmount.ToString();
            
            // topLineImage.gameObject.SetActive(true);
            // botLineImage.gameObject.SetActive(true);
        }

        public void SetRewardClaimed(bool isClaimed)
        {
            indexText.gameObject.SetActive(!isClaimed);
            lockGameObj.SetActive(!isClaimed);
            claimedGameObj.SetActive(isClaimed);
        }

        public void SetTopItem()
        {
            topLineImage.gameObject.SetActive(false);
        }

        public void SetBotItem()
        {
            botLineImage.gameObject.SetActive(false);
        }
        
    }
}
