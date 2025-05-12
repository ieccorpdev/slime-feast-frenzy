using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class StarChasePlayerDataUI : _BaseBehaviour
    {
        public TextMeshProUGUI playerName;
        public TextMeshProUGUI playerRank;
        public TextMeshProUGUI playerProgress;

        public Button rewardInfoButton;
        public EventPlayerData EventPlayerData { get; private set; }

        private void Start()
        {
            rewardInfoButton.onClick.AddListener(OnShowRewardInfoUI);
        }

        private void OnShowRewardInfoUI()
        {
            
        }

        public void Setup(EventPlayerData playerData, int rank)
        {
            EventPlayerData = playerData;

            playerName.text = EventPlayerData.id;
            playerRank.text = rank.ToString();
            playerProgress.text = EventPlayerData.progress.ToString();

            if (EventPlayerData.isPlayer)
            {
                SetMainPlayerUI();
            }
        }

        private void SetMainPlayerUI()
        {
            
        }
    }
}
