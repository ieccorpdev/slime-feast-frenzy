using EnhancedUI.EnhancedScroller;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace IEC.FakeLeaderboard
{
    public class PopupFakeLeaderboardAllUser : MonoBehaviour
    {
        public float cellSize;

        [SerializeField]
        private EnhancedScroller m_enhanScroller;
        [SerializeField]
        private FakeLeaderboardCellView m_cellPrefab;

        [SerializeField]
        private List<FakeLeaderboardCellView> m_cellTop3;
        [SerializeField]
        private FakeLeaderboardCellView m_cellMy;

        [SerializeField]
        private Image img_myRank;
        [SerializeField]
        private TextMeshProUGUI txt_myRank;

        private List<FakeLeaderboardUserData> m_datas;
        private ExtendEnhancedScrollerDelegate<FakeLeaderboardCellView> m_enhanScrollerDelegate;
        private PopupFakeLeaderboard m_panelMain;
        private FakeLeaderboardUserData m_MyUserData;

        // Start is called before the first frame update
        void Start()
        {

        }




        public void Init(PopupFakeLeaderboard panelMain)
        {
            m_panelMain = panelMain;

            m_datas = panelMain.Controller.leaderDataSave.playersToShowOnListView;
            m_enhanScrollerDelegate = new ExtendEnhancedScrollerDelegate<FakeLeaderboardCellView>();
            m_enhanScrollerDelegate.Init(cellSize, m_datas.Count, m_cellPrefab, OnSetDataEvent);
            m_enhanScroller.Delegate = m_enhanScrollerDelegate;

            m_enhanScroller.ReloadData();

            for (int i = 0; i < m_cellTop3.Count; i++)
            {
                //      panelMain.Controller.leaderDataSave.top3ContainPlayer[i].index = m_cellTop3.Count - i;
                var currentUser = panelMain.Controller.leaderDataSave.top3ContainPlayer[i];

                m_cellTop3[i].LoadData(currentUser, false,currentUser.Equals(panelMain.Controller.leaderDataSave.userData));
            }

            m_panelMain.Controller.UpdateMyUserCellView(m_cellMy);

            m_MyUserData = panelMain.Controller.leaderDataSave.userData;

            var m_MyUserRankData = panelMain.Controller.UserRank();
            if (m_MyUserRankData.rankName != FakeLeaderboardRankType.Unrank)
            {
                img_myRank.sprite = m_MyUserRankData.rankView;
                img_myRank.gameObject.SetActive(true);
                txt_myRank.text = m_MyUserRankData.rankName.ToString();
            }
            else
            {
                img_myRank.sprite = null;
                img_myRank.gameObject.SetActive(false);
                txt_myRank.text = m_MyUserRankData.rankName.ToString();
            }

            //  Invoke("ScrollToPlayerPosition", .5f);
            ScrollToPlayerPosition();


        }

        private void ScrollToPlayerPosition()
        {
            int posToScroll = 0;
            for (int i = 0; i < m_datas.Count; i++)
            {
                if (m_datas[i].Equals(m_MyUserData))
                {
                    posToScroll = m_datas.Count - i; break;
                }
            }
            //   Debug.LogError(posToScroll);
            m_enhanScroller.JumpToDataIndex(posToScroll - 3, 0, 0, true, EnhancedScroller.TweenType.immediate, 0, null, EnhancedScroller.LoopJumpDirectionEnum.Closest, true);

            // m_enhanScroller.ReloadData(posToScroll - 1);

        }
        private void OnSetDataEvent(FakeLeaderboardCellView cellView, int dataIndex)
        {
            int index = m_datas.Count - 1 - dataIndex;

            var _data = m_datas[index];

            // m_datas[index].index = dataIndex + 1;
            cellView.LoadData(_data, true, _data == m_MyUserData);
        }

        //// Update is called once per frame
        //void Update()
        //{
        //    if (Input.GetKeyDown(KeyCode.V))
        //    {
        //        ForceUpdate();
        //    }
        //}

        //void ForceUpdate()
        //{
        //    Debug.LogError("ForceUpdate");
        //    LayoutRebuilder.ForceRebuildLayoutImmediate(m_enhanScroller.GetComponent<RectTransform>());
        //    m_enhanScroller.JumpToDataIndex(8);
        //    var rect = m_enhanScroller.GetComponent<RectTransform>();
        //    rect.position += new Vector3(0, 0, 1);
        //}
    }
}
