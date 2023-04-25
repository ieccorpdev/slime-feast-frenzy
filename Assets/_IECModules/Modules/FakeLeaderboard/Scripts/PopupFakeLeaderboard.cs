using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IEC.FakeLeaderboard
{
    public class PopupFakeLeaderboard : BasePopup
    {

        public enum PopupType
        {
            UserAllRank,
            UserNameEdit,
            UserInfor
        }

        public FakeLeaderboardController Controller => Controllers.Get<FakeLeaderboardController>();

        [SerializeField]
        private GameObject m_panelHeader;

        [SerializeField]
        Button m_btnEditNameShow, m_btnEditNameHide;
        [SerializeField]
        Button m_btnUserInforShow1, m_btnUserInforShow2, m_btnUserInforHide;

        [SerializeField]
        private PopupFakeLeaderboardAllUser m_panelAllUser;
        [SerializeField]
        private PopupFakeLeaderboardUserNameEdit m_panelUsernameEdit;
        [SerializeField]
        private PopupFakeLeaderboardUserInfor m_panelUserInfor;


        [SerializeField]
        private PopupFakeLeaderboardEndgame m_panelEndgame;



        //public override void Awake()
        //{
        //    base.Awake();
        //    m_panelEndgame.Init(this);
        //}

        // Start is called before the first frame update
        void Start()
        {
            LoadData();
            m_btnEditNameShow.onClick.AddListener(() =>
            {
                SetPopupType(PopupType.UserNameEdit);
            });

            m_btnEditNameHide.onClick.AddListener(() =>
            {
                SetPopupType(PopupType.UserAllRank);
            });


            m_btnUserInforShow1.onClick.AddListener(() =>
            {
                SetPopupType(PopupType.UserInfor);
            });
            m_btnUserInforShow2.onClick.AddListener(() =>
            {
                SetPopupType(PopupType.UserInfor);
            });
            m_btnUserInforHide.onClick.AddListener(() =>
            {
               // Debug.LogError("m_btnUserInforHide");
                SetPopupType(PopupType.UserAllRank);
            });
        }


        public void LoadData()
        {

            m_panelAllUser.Init(this);
            m_panelUsernameEdit.Init(this);
            m_panelUserInfor.Init(this);
            //  m_panelEndgame.Init(this);


        }



        public override void Show()
        {

            base.Show();
            LoadData();
        }

        public void SetPopupType(PopupType popupType)
        {
            switch (popupType)
            {
                case PopupType.UserAllRank:
                    m_panelUserInfor.gameObject.SetActive(false);
                    m_panelUsernameEdit.gameObject.SetActive(false);
                    m_panelAllUser.gameObject.SetActive(true);
                    break;
                case PopupType.UserNameEdit:
                    // Controllers.Get<ServicesController>().PushEvent(new IEC.TrackingEvent.ClickLeaderboardNameEdit());
                    m_panelUsernameEdit.gameObject.SetActive(true);
                    break;
                case PopupType.UserInfor:
                    // Controllers.Get<ServicesController>().PushEvent(new IEC.TrackingEvent.ClickLeaderboardInfor());
                    m_panelUserInfor.gameObject.SetActive(true);
                    break;
                default:
                    break;
            }
        }



    }
}
