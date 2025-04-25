using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IEC.Wind.Leaderboard
{
    public class PopupLeaderboardFinished : BasePopup
    {
        [SerializeField] Button btnOK;
        public TextMeshProUGUI title;
        public GameObject legendSafe;
        public GameObject otherSafe;
        public GameObject rankUp;
        public GameObject rankDown;

        public List<ItemUI> rewardItems = new List<ItemUI>();
        public List<TextMeshProUGUI> lstLeagueTxt = new List<TextMeshProUGUI>();
        public List<Image> lstLeagueImgs = new List<Image>();

        int goldAdd = 0;
        private void Start()
        {
            SetEvent(btnOK, () =>
            {
                if (goldAdd > 0)
                {
                    //UI.ClaimCoin(rewardItems.FindLast(x => x.gameObject.activeInHierarchy).transform.position, 10, PlayerData.gold, () =>
                    //{
                    //    Hide();
                    //});
                }
                else
                {
                    Hide();
                }
            });
        }        
        public void SetData(LeaderboardRankType currentLeague, TypeFinish typeFinish, List<RewardData> rewards)
        {
            legendSafe.SetActive(false);
            otherSafe.SetActive(false);
            rankUp.SetActive(false);
            rankDown.SetActive(false);
            switch (typeFinish)
            {
                case TypeFinish.Safe:
                    title.text = "RANK REWARD";
                    if (currentLeague == LeaderboardRankType.Legend)
                    {
                        legendSafe.SetActive(true);
                    }
                    else
                    {
                        otherSafe.SetActive(true);
                    }
                    break;
                case TypeFinish.Up:
                    title.text = "RANK UP";
                    rankUp.SetActive(true);
                    break;
                case TypeFinish.Down:
                    title.text = "RANK DOWN";
                    rankDown.SetActive(true);
                    break;
            }
            lstLeagueImgs.ForEach(x => x.sprite = LeaderboardConfig.GetSpirteCurrentLeague());
            lstLeagueTxt.ForEach(x => x.text = LeaderboardData.currentRankView.ToString().ToUpper());

            rewards.Sort((x,y)=> y.type.CompareTo(x.type));
            rewardItems.ForEach(x => x.gameObject.SetActive(false));
            for(int i = 0; i < rewards.Count; i++)
            {
                Sprite spr = PlayConfig.GetSpriteByReward(rewards[i].type);
                if (rewards[i].type == ResourceType.Gold)
                {
                    goldAdd = rewards[i].amount;
                    spr = PlayConfig.GetGoldSprite(rewards[i].amount);
                }
                string rwTxt = "x" + rewards[i].amount;
                rewardItems[i].SetData(spr, rwTxt);
                rewardItems[i].gameObject.SetActive(true);
            }
            Show();
        }
    }
}