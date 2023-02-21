using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
namespace IEC.FakeLeaderboard
{
    public class PopupFakeLeaderboardEndgame : _BaseBehaviour
    {
        FakeLeaderboardController Controller => Controllers.Get<FakeLeaderboardController>();

        [SerializeField]
        private GameObject m_panelWinTrophy, m_panelRank, m_panelRankup;
        [Header("Win trophy")]
        [SerializeField]
        private TextMeshProUGUI m_txtWintrophy;
        [SerializeField]
        private TextMeshProUGUI m_txtWintrophy2;

        [Header("Rank")]
        [SerializeField]
        private TextMeshProUGUI m_myRank;
        [SerializeField]
        private TextMeshProUGUI m_myRankPromote;
        [SerializeField]
        private Image m_imgRank;


        [Header("RankUp")]
        [SerializeField]
        private TextMeshProUGUI m_myRankOnRankup;
        [SerializeField]
        private Image m_imgRankOnRankup;
        [SerializeField]
        private FakeLeaderboardCellView[] m_otherUserRankCellViews;
        [SerializeField]
        private FakeLeaderboardCellView m_myRankCellView;
        [SerializeField]
        private Transform m_transMyRankParent, m_transNewRankParent, m_transMyRankAllView;


        // private PopupFakeLeaderboard m_panelMain;
        //public void Init(PopupFakeLeaderboard panelMain)
        //{
        //    m_panelMain = panelMain;
        //}

        public void ShowWinTrophy(int trophyEarn)
        {
            HideAllPanel();
            m_txtWintrophy.text = "You get x" + trophyEarn;
            if (trophyEarn == 1)
                m_txtWintrophy2.text = "Trophy!";
            else
                m_txtWintrophy2.text = "Trophies!";
            m_panelWinTrophy.SetActive(true);
        }

        void ShowRank(bool isPromote = false)
        {
            HideAllPanel();
            m_panelRank.SetActive(true);
            m_myRank.text = "#" + Controller.UserPosition() + " " + Controller.UserRank().rankName + "";
            m_myRankPromote.text = Controller.UserRank().rankName + "";
            m_myRank.gameObject.SetActive(!isPromote);
            m_myRankPromote.transform.parent.gameObject.SetActive(isPromote);
            m_imgRank.sprite = Controller.UserRank().rankView;
        }

        public void ShowRankup(bool isPromote = false, bool isRankUp = true)
        {
            HideAllPanel();
            int usePosition = Controller.UserPosition();
            int indexView = 0;
            for (int i = -2; i <= 8; i++)
            {
                if (i == 0) continue;
                var userOther = Controller.leaderDataSave.playerByIndex(usePosition + i);
                //if (userOther != null)
                //    userOther.index = usePosition + i;
                m_otherUserRankCellViews[indexView].LoadData(userOther, true);
                indexView++;
            }

            m_panelRankup.SetActive(true);
            m_transMyRankAllView.gameObject.SetActive(true);
            m_myRankCellView.transform.parent = m_transMyRankParent.transform;
            m_myRankCellView.transform.localPosition = Vector3.zero;
            Controller.UpdateMyUserCellView(m_myRankCellView);
            m_myRankOnRankup.text = Controller.UserRank().rankName + "";
            m_imgRankOnRankup.sprite = Controller.UserRank().rankView;

            if (isRankUp)
                StartCoroutine(DoPromote(isPromote));
            else
                ShowRank(isPromote);
        }

        void GetUserIndex()
        { }

        IEnumerator DoPromote(bool isPromote = false)
        {
            yield return new WaitForEndOfFrame();
            var _scroll = m_transNewRankParent.parent.GetComponent<ScrollRect>();
            _scroll.normalizedPosition = new Vector2(0, 0);

            m_myRankCellView.transform.parent = m_panelRankup.transform;
            yield return new WaitForSeconds(1);
            m_transMyRankAllView.gameObject.SetActive(false);
            Vector3 temp = m_myRankCellView.transform.localScale;

            DOVirtual.Float(0, 1, .5f, f =>
            {
                _scroll.normalizedPosition = new Vector2(0, f);
            }).OnComplete(() =>
            {
                _scroll.normalizedPosition = new Vector2(0, 1);
            }).SetEase(Ease.OutBack);
            yield return new WaitForSeconds(.5f);
            m_myRankCellView.transform.localScale = 1.1f * temp;

            m_myRankCellView.transform.DOMove(m_otherUserRankCellViews[2].transform.position, .5f).SetEase(Ease.OutBack);
            yield return new WaitForSeconds(.6f);
            m_myRankCellView.transform.localScale = temp;
            m_myRankCellView.transform.parent = m_transNewRankParent.transform;
            m_myRankCellView.transform.SetSiblingIndex(2);
            yield return new WaitForSeconds(3);
            m_panelRankup.SetActive(false);
            ShowRank(isPromote);
        }

        void HideAllPanel()
        {
            m_panelWinTrophy.SetActive(false);
            m_panelRank.SetActive(false);
            m_panelRankup.SetActive(false);
        }

    }
}
