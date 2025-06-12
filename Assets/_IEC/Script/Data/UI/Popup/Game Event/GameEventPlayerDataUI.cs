using System.Collections;
using System.Collections.Generic;
using EnhancedUI.EnhancedScroller;
using IEC.Common.Event;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace IEC
{
    public class GameEventPlayerDataUI : EnhancedScrollerCellView
    {
        public int spriteSetIndex;
        public Image avatarImage;
        public Image avatarFrameImage;
        public Image displayFrame;
        public Image progressDisplayFrame;
        public TextMeshProUGUI playerName;
        public Image playerRankImage;
        public TextMeshProUGUI playerRankText;
        public TextMeshProUGUI playerProgress;

        public Button rewardInfoButton;
        public Image rewardInfoButtonImage;

        public RewardInfoGroup rewardInfo;
        public EventPlayerData EventPlayerData { get; private set; }

        private GameEventConfig _gameEventConfig;
        private EventData _eventData;
        
        public int CurrentRank { get; private set; }

        private void Start()
        {
            if (rewardInfoButton != null)
            {
                rewardInfoButton.onClick.AddListener(OnShowRewardInfoUI);
            }
        }

        protected virtual void SetRewardInfoDown() { }

        protected virtual void SetRewardInfoUp() { }

        private void OnShowRewardInfoUI()
        {
            if (rewardInfoButton == null)
                return;
            var pos = GetHalf(Locator.Main.PopupStarChase.scroller.GetComponent<RectTransform>(),
                GetComponent<RectTransform>());
            var eventRewardData = _eventData.listEventRewardData.Find(data =>
                data.rankStartReward <= CurrentRank && (data.rankEndReward >= CurrentRank || data.rankEndReward == -1)); // = -1 means from rank start reward to the end of the leaderboard

            if (eventRewardData == null)
                return;
            if (pos == HalfPosition.Top)
            {
                SetRewardInfoDown();
            }
            else if (pos == HalfPosition.Bottom)
            {
                SetRewardInfoUp();
            }
            rewardInfo.SetupDataItem(eventRewardData.listReward);
            rewardInfo.Show();
        }

        protected virtual void UpdatePlayerAvatar()
        {
            var config = Pool.Get<GameConfig>();
            var playerData = Pool.Get<PlayerSecret>();
            if (EventPlayerData.isPlayer)
            {
                if(avatarImage != null)
                    avatarImage.sprite = config.playerAvatarConfig.GetAvatarSprite(playerData.playerAvatarSpriteName);
                if(avatarFrameImage != null)
                    avatarFrameImage.sprite = config.playerAvatarConfig.GetAvatarFrameSprite(playerData.playerAvatarFrameName);
            }
            else
            {
                if(avatarImage != null)
                    avatarImage.sprite = config.playerAvatarConfig.GetAvatarSprite(EventPlayerData.avatarID);
                if(avatarFrameImage != null)
                    avatarFrameImage.sprite = config.playerAvatarConfig.GetAvatarFrameSprite(EventPlayerData.avatarFrameID);
            }
        }

        protected virtual void UpdatePlayerRank()
        {
            if (playerRankText != null)
            {
                playerRankText.gameObject.SetActive(true);
                playerRankText.text = CurrentRank.ToString();
            }

            if(playerRankImage != null)
                playerRankImage.gameObject.SetActive(false);


            Sprite rankSprite;
            if ((rankSprite = _gameEventConfig.GetRankSprite(spriteSetIndex, CurrentRank)) != null)
            {
                playerRankText.gameObject.SetActive(false);
                playerRankImage.gameObject.SetActive(true);
                playerRankImage.sprite = rankSprite;
            }

            if (rewardInfoButtonImage != null)
            {
                var eventRewardData = _eventData.listEventRewardData.Find(data =>
                    data.rankStartReward <= CurrentRank && (data.rankEndReward >= CurrentRank || data.rankEndReward == -1));

                if (eventRewardData == null)
                {
                    rewardInfoButtonImage.gameObject.SetActive(false);
                }
                else
                {
                    rewardInfoButtonImage.gameObject.SetActive(true);
                    var sprite = _gameEventConfig.GetRewardButtonSprite(spriteSetIndex, CurrentRank);
                    rewardInfoButtonImage.sprite = sprite;
                    rewardInfoButtonImage.SetNativeSize();
                }
            }
        }
        
        protected virtual void UpdatePlayerProgress()
        {
            playerProgress.text = EventPlayerData.progress.ToString();
        }

        public void SetAvatar(Sprite sprite)
        {
            avatarImage.sprite = sprite;
            avatarImage.SetNativeSize();
        }

        public void SetName(string text)
        {
            playerName.text = text;
        }

        public void SetProgressText(string text)
        {
            playerProgress.text = text;
        }

        public void Setup(GameEventConfig gameEventConfig, EventData eventData, EventPlayerData playerData, int rank)
        {
            _gameEventConfig = gameEventConfig;
            _eventData = eventData;
            EventPlayerData = playerData;

            CurrentRank = rank;

            playerName.text = EventPlayerData.id;
            
            UpdatePlayerRank();
            UpdatePlayerProgress();
            
            CheckSetMainPlayerUI();
            UpdatePlayerAvatar();
        }

        public void UpdateUI()
        {
            playerName.text = EventPlayerData.id;
            
            UpdatePlayerRank();
            UpdatePlayerProgress();
            
            CheckSetMainPlayerUI();
            UpdatePlayerAvatar();
        }

        protected virtual void CheckSetMainPlayerUI()
        {
            if (displayFrame == null)
                return;
            
            if (EventPlayerData.isPlayer)
            {
                displayFrame.sprite = _gameEventConfig.GetDisplaySprite(spriteSetIndex).mainPlayerFrameSprite;
                if(progressDisplayFrame != null)
                    progressDisplayFrame.sprite = _gameEventConfig.GetDisplaySprite(spriteSetIndex).mainPlayerProgressFrameSprite;
            }
            else
            {
                displayFrame.sprite = _gameEventConfig.GetDisplaySprite(spriteSetIndex).normalFrameSprite;
                if(progressDisplayFrame != null)
                    progressDisplayFrame.sprite = _gameEventConfig.GetDisplaySprite(spriteSetIndex).normalProgressFrameSprite;
            }
        }
        
        public enum HalfPosition { Top, Bottom, Left, Right }

        public HalfPosition GetHalf(RectTransform container, RectTransform nestedChild)
        {
            // Get the child's world position and convert it to container's local space
            Vector3 localPos = container.InverseTransformPoint(nestedChild.position);

            // Get container's half height
            float halfHeight = container.rect.height / 2f;

            // In local space, Y=0 is at center; +Y is up, -Y is down
            if (localPos.y > 0)
                return HalfPosition.Top;
            else
                return HalfPosition.Bottom;
        }
    }
}
