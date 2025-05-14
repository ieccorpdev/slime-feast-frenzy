using System.Collections;
using System.Collections.Generic;
using EnhancedUI.EnhancedScroller;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace IEC
{
    public class GameEventPlayerDataUI : EnhancedScrollerCellView
    {
        public Image displayFrame;
        public TextMeshProUGUI playerName;
        public Image playerRankImage;
        public TextMeshProUGUI playerRankText;
        public TextMeshProUGUI playerProgress;

        public Button rewardInfoButton;
        public Image rewardInfoButtonImage;
        public EventPlayerData EventPlayerData { get; private set; }

        private GameEventConfig _gameEventConfig;

        private void Start()
        {
            rewardInfoButton.onClick.AddListener(OnShowRewardInfoUI);
        }

        private void OnShowRewardInfoUI()
        {
            
        }

        protected virtual void UpdatePlayerRank(int rank)
        {
            playerRankText.gameObject.SetActive(true);
            playerRankImage.gameObject.SetActive(false);
            playerRankText.text = rank.ToString();

            Sprite rankSprite;
            if ((rankSprite = _gameEventConfig.GetRankSprite(rank)) != null)
            {
                playerRankText.gameObject.SetActive(false);
                playerRankImage.gameObject.SetActive(true);
                playerRankImage.sprite = rankSprite;
            }

            rewardInfoButtonImage.sprite = _gameEventConfig.GetRewardButtonSprite(rank);
            rewardInfoButtonImage.SetNativeSize();
        }
        
        protected virtual void UpdatePlayerProgress()
        {
            playerProgress.text = EventPlayerData.progress.ToString();
        }

        public void Setup(GameEventConfig gameEventConfig, EventPlayerData playerData, int rank)
        {
            _gameEventConfig = gameEventConfig;
            EventPlayerData = playerData;

            playerName.text = EventPlayerData.id;
            
            UpdatePlayerRank(rank);
            UpdatePlayerProgress();
            
            CheckSetMainPlayerUI();

        }

        protected virtual void CheckSetMainPlayerUI()
        {
            if (EventPlayerData.isPlayer)
            {
                displayFrame.sprite = _gameEventConfig.mainPlayerFrameSprite;
            }
            else
            {
                displayFrame.sprite = _gameEventConfig.normalFrameSprite;
            }
        }
    }
}
