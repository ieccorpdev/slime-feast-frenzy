using System;
using System.Collections;
using System.Collections.Generic;
using EnhancedUI.EnhancedScroller;
using IEC.Common.Event;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IEC.Wind.Leaderboard
{
    public class RankItem : EnhancedScrollerCellView
    {
        protected static LeaderboardConfig LeaderboardConfig => Pool.Get<LeaderboardConfig>();
        protected static LeaderboardSaveData LeaderboardData => Pool.Get<LeaderboardSaveData>();

        public Image frameImg;
        public Image rankIcon;
        public Image leagueIcon;
        public Image leagueFrameIcon;
        public TextMeshProUGUI medalTxt;

        public TextMeshProUGUI rankTxt;
        public TextMeshProUGUI nameUser;
        public TextMeshProUGUI namePlayer;

        public GameObject rankDownObj;
        public GameObject rankUpObj;
        public Image itemAvatarImg;
        public Button infoItemReward;
        public GameObject rewardObject;
        public List<ItemUI> items = new List<ItemUI>();

        public void Start()
        {
            infoItemReward.onClick.AddListener(ShowItemReward);
            gameObject.OnEvent(EventList.GamePlay.SHOW_REWARD_INFO_LEADERBOARD, HideItemReward);
        }

        private void OnEnable()
        {
            rewardObject.gameObject.SetActive(false);
        }

        public void Init(UserData data, int type)
        {
            rankIcon.gameObject.SetActive(data.rank <= 3);
            rankTxt.gameObject.SetActive(data.rank > 3);
            nameUser.text = data.data.name;
            namePlayer.text = data.data.name;
            medalTxt.text = "" + data.data.medal;

            leagueIcon.sprite = LeaderboardConfig.GetSpirteCurrentLeague();
            if (data.rank <= 3)
            {
                rankIcon.sprite = LeaderboardConfig.GetSpriteRank(data.rank);
            }
            else
            {
                rankTxt.text = "" + data.rank;
            }
            rankDownObj.SetActive(false);
            rankUpObj.SetActive(false);
            infoItemReward.image.sprite = LeaderboardConfig.GetBoxRewardSprite(data.rank);
            infoItemReward.image.SetNativeSize();
            if (!data.isPlayer)
            {
                frameImg.sprite = LeaderboardConfig.frameSprites[type];
                itemAvatarImg.sprite = LeaderboardConfig.avatarSprites[data.data.avatarId];
                namePlayer.gameObject.SetActive(false);
                nameUser.gameObject.SetActive(true);
                leagueFrameIcon.sprite = LeaderboardConfig.botLeagueFrameSprite;
                //var color = LeaderboardConfig.lstColor[type];
                //rankTxt.color = color.rankColor;
                //nameUser.color = color.nameColor;
                //rankTxt.fontMaterial = color.materialRankTxt;
            }
            else
            {
                nameUser.gameObject.SetActive(false);
                namePlayer.gameObject.SetActive(true);
                frameImg.sprite = LeaderboardConfig.playerFrameSprites;
                itemAvatarImg.sprite = LeaderboardConfig.avatarSprites[LeaderboardData.avatarId];
                leagueFrameIcon.sprite = LeaderboardConfig.playerLeagueFrameSprite;
            }
        }
        public void SetRewardData(List<RewardData> rewards)
        {
            items.ForEach(x => x.gameObject.SetActive(false));
            for(int i = 0; i < rewards.Count; i++)
            {
                if(i >= items.Count)
                    break;
                Sprite spr = Pool.Get<GameConfig>().GetSpriteByReward(rewards[i].type);
                string txt = "" + rewards[i].amount;
                items[i].SetData(spr, txt);
                items[i].gameObject.SetActive(true);
            }
        }

        public void ShowItemReward()
        {
            EventController.RaiseEvent(EventList.GamePlay.SHOW_REWARD_INFO_LEADERBOARD);
            rewardObject.gameObject.SetActive(!rewardObject.activeInHierarchy);
        }

        public void HideItemReward()
        {
            rewardObject.gameObject.SetActive(false);
        }
        
        public void SetActiveRankDownObj(bool active)
        {
            rankDownObj.SetActive(active);
        }
        public void SetActiveRankUpObj(bool active)
        {
            rankUpObj.SetActive(active);
        }
    }
}