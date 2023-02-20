using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using DG.Tweening;
namespace IEC.FakeLeaderboard
{

    public class PopupFakeLeaderboardUserNameEdit : MonoBehaviour
    {
        [SerializeField]
        private TMP_InputField m_inputName;
        [SerializeField]
        private Button m_btnButtonOK;
        [SerializeField]
        private Transform m_panelWarning;
        [SerializeField]
        private TextMeshProUGUI[] m_textWarnings;

        private PopupFakeLeaderboard m_panelMain;
        private Vector3 m_posWarning;


        private void Start()
        {
            m_posWarning = m_panelWarning.transform.localPosition;
            m_btnButtonOK.onClick.AddListener(OnClickOKEvent);
            m_inputName.onValueChanged.AddListener(OnEndEditEvent);
        }

        private void OnEndEditEvent(string arg0)
        {
            // throw new NotImplementedException();
            if (m_inputName.text.Length < 4 || m_inputName.text.Length > 12)
            {
                m_btnButtonOK.interactable = false;
            }
            else
                m_btnButtonOK.interactable = true;
        }

        private void OnEnable()
        {
            if (m_panelMain != null && m_panelMain.Controller.leaderDataSave.userData != null)
                m_inputName.text = m_panelMain.Controller.leaderDataSave.userData.name;
            OnEndEditEvent("");
        }
        private void OnClickOKEvent()
        {
            m_panelWarning.transform.DOKill();
            m_panelWarning.transform.localPosition = m_posWarning;
            if (m_inputName.text.Length < 4 || m_inputName.text.Length > 12)
            {
                //    m_panelWarning.transform.DOShakePosition(1, 10);
                return;
            }
            
            // Controllers.Get<ServicesController>().PushEvent(new IEC.TrackingEvent.ClickLeaderboardNameEditOk());
            m_panelMain.Controller.UpdateUsername(m_inputName.text);
            m_panelMain.LoadData();
            m_panelMain.SetPopupType(PopupFakeLeaderboard.PopupType.UserAllRank);
        }

        public void Init(PopupFakeLeaderboard panelMain)
        {
            m_panelMain = panelMain;
        }
    }
}
