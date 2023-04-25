using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
namespace IEC.FakeLeaderboard
{
    public class PopupFakeLeaderboardUserInfor : MonoBehaviour
    {
        private PopupFakeLeaderboard m_panelMain;

        [SerializeField]
        private FakeLeaderboardCellView m_userCell;

        [SerializeField]
        Image img_myRank;
        [SerializeField]
        TextMeshProUGUI txt_myRank;

        public void Init(PopupFakeLeaderboard panelMain)
        {
            m_panelMain = panelMain;

            m_panelMain.Controller.UpdateMyUserCellView(m_userCell);

            var userRank = panelMain.Controller.UserRank();
            if (userRank.rankName != FakeLeaderboardRankType.Unrank)
            {
                img_myRank.sprite = userRank.rankView;
                img_myRank.gameObject.SetActive(true);
                txt_myRank.text = userRank.rankName.ToString();
            }
            else
            {
                img_myRank.sprite = null;
                img_myRank.gameObject.SetActive(false);
                txt_myRank.text = userRank.rankName.ToString();
            }
        }
    }
}
