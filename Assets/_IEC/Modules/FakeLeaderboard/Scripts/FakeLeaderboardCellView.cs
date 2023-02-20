using EnhancedUI.EnhancedScroller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace IEC.FakeLeaderboard
{
    public class FakeLeaderboardCellView : EnhancedScrollerCellView
    {
        FakeLeaderboardController Controller => Controllers.Get<FakeLeaderboardController>();

        public FakeLeaderboardUserData Data { get; set; }

        [SerializeField]
        Image m_imgNormalUser, m_imgMyUser;

        [SerializeField]
        private TextMeshProUGUI m_txtPosition, m_txtName, m_txtScore;
        [SerializeField]
        private Image m_imgFlag;




        public void LoadData(FakeLeaderboardUserData data, bool checkNull = false, bool isMy = false)
        {
            m_imgNormalUser.enabled = !isMy;
            m_imgMyUser.enabled = isMy;
            if (checkNull)
                if (data == null) { gameObject.SetActive(false); return; }
                else gameObject.SetActive(true);
            // Debug.LogError(JsonUtility.ToJson(data));
            this.Data = data;
            m_txtName.text = data.name;
            m_txtScore.text = data.trophy + "";
            m_txtPosition.text = "#" + data.index;

            var currentRank = Controller.TrophyToRank(data.trophy);
            if (currentRank.rankName == FakeLeaderboardRankType.Unrank)
                m_txtPosition.text = "---";

            var national = Controller.NationalByID(data.nationalID);
            if (national == null)
            {
                m_imgFlag.gameObject.SetActive(false);
            }
            else
            {
                m_imgFlag.gameObject.SetActive(true);
                m_imgFlag.sprite = national.flag;
            }
        }
    }
}
