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
        
        public ProgressRewardData ProgressRewardData { get; private set; }
        
        public void Setup(int index, ProgressRewardData data)
        {
            ProgressRewardData = data;
            
            indexText.text = index.ToString();

            icon.sprite = Config.progressRewardConfig.GetRewardSprite(data.reward.typeReward);

        }

        public void SetRewardClaimed(bool isClaimed)
        {
            indexText.gameObject.SetActive(!isClaimed);
            lockGameObj.SetActive(!isClaimed);
            claimedGameObj.SetActive(isClaimed);
        }
        
    }
}
