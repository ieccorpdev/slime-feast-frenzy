using System.Collections;
using System.Collections.Generic;
using EnhancedUI.EnhancedScroller;
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
        public TextMeshProUGUI medalTxt;

        public TextMeshProUGUI rankTxt;
        public TextMeshProUGUI nameUser;

        public GameObject rankDownObj;
        public GameObject rankUpObj;

        public List<ItemUI> items = new List<ItemUI>();
        public void Init(UserData data, int type)
        {
            rankIcon.gameObject.SetActive(data.rank <= 3);
            rankTxt.gameObject.SetActive(data.rank > 3);

            nameUser.text = data.data.name;
            medalTxt.text = "" + data.data.medal;

            leagueIcon.sprite = LeaderboardConfig.GetSpirteCurrentLeague();
            if (data.rank <= 3)
            {
                rankIcon.sprite = LeaderboardConfig.GetSpirteRank(data.rank);
            }
            else
            {
                rankTxt.text = "" + data.rank;
            }
            rankDownObj.SetActive(false);
            rankUpObj.SetActive(false);

            if (!data.isPlayer)
            {
                frameImg.sprite = LeaderboardConfig.frameSprites[type];
                //var color = LeaderboardConfig.lstColor[type];
                //rankTxt.color = color.rankColor;
                //nameUser.color = color.nameColor;
                //rankTxt.fontMaterial = color.materialRankTxt;
            }
        }
        public void SetRewardData(List<RewardData> rewards)
        {
            items.ForEach(x => x.gameObject.SetActive(false));
            for(int i = 0; i < rewards.Count; i++)
            {
                Sprite spr = Pool.Get<GameConfig>().GetSpriteByReward(rewards[i].type);
                string txt = "" + rewards[i].amount;
                items[i].SetData(spr, txt);
                items[i].gameObject.SetActive(true);
            }
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