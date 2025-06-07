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
        public GameObject rewardInfoGroup;
        public GameObject rewardInfoDownImg;
        public GameObject rewardInfoUpImg;

        public List<GameEventRewardInfoUI> listRewardInfoUI = new();
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

            OnHideRewardInfoUI();
            gameObject.OnEvent(EventList.GamePlay.SHOW_REWARD_INFO_GAME_EVENT, OnHideRewardInfoUI);
        }

        private void OnHideRewardInfoUI()
        {
            if (rewardInfoButton == null)
                return;

            if (rewardInfoGroup == null)
                return;
            
            rewardInfoGroup.SetActive(false);
            
            if(rewardInfoDownImg != null)
                rewardInfoDownImg.gameObject.SetActive(false);
            if(rewardInfoUpImg != null)
                rewardInfoUpImg.gameObject.SetActive(false);
        }

        private void SetRewardInfoDown()
        {
            if (rewardInfoButton == null)
                return;

            if (rewardInfoDownImg == null)
                return;
            
            rewardInfoDownImg.gameObject.SetActive(true);
            rewardInfoGroup.GetComponent<RectTransform>().anchoredPosition =
                rewardInfoDownImg.GetComponent<RectTransform>().anchoredPosition;
        }

        private void SetRewardInfoUp()
        {
            if (rewardInfoButton == null)
                return;
            
            if (rewardInfoUpImg == null)
                return;
            
            rewardInfoUpImg.gameObject.SetActive(true);
            rewardInfoGroup.GetComponent<RectTransform>().anchoredPosition =
                rewardInfoUpImg.GetComponent<RectTransform>().anchoredPosition + Vector2.up * 30;
        }

        private void OnShowRewardInfoUI()
        {
            if (rewardInfoButton == null)
                return;
            
            bool wasActive = rewardInfoGroup.gameObject.activeInHierarchy;
            EventController.RaiseEvent(EventList.GamePlay.SHOW_REWARD_INFO_GAME_EVENT);
            var eventRewardData = _eventData.listEventRewardData.Find(data =>
                data.rankStartReward <= CurrentRank && (data.rankEndReward >= CurrentRank || data.rankEndReward == -1)); // = -1 means from rank start reward to the end of the leaderboard

            if (eventRewardData == null)
                return;
            
            if (wasActive)
                return;

            var pos = GetHalf(Locator.Main.PopupStarChase.scroller.GetComponent<RectTransform>(),
                GetComponent<RectTransform>());

            if (pos == HalfPosition.Top)
            {
                SetRewardInfoDown();
            }
            else if (pos == HalfPosition.Bottom)
            {
                SetRewardInfoUp();
            }
            
            rewardInfoGroup.SetActive(true);
            
            foreach (var rewardUI in listRewardInfoUI)
            {
                Reward rewardData;
                if ((rewardData = eventRewardData.listReward.Find(data => data.typeReward == rewardUI.resourceType)) != null)
                {
                    rewardUI.gameObject.SetActive(true);
                    rewardUI.UpdateAmountText(rewardData.rewardAmount.ToString());
                }
                else
                {
                    rewardUI.gameObject.SetActive(false);
                }
            }
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
                    rewardInfoButtonImage.transform.GetChild(0).GetComponent<Image>().sprite = sprite;
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
