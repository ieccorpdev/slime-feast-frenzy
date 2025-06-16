using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IEC.Wind.Leaderboard
{
    public class PopupEditInfo : BasePopup
    {
        public Button tabName, tabAvatar,editButton;
        public Image avatarMain;
        public Sprite tabOn, tabOff;
        public Color tabOnColorTxt, tabOffColorTxt;

        public GameObject panelAvatar, panelName;

        public List<AvatarUI> avatarUis = new List<AvatarUI>();

        [SerializeField]
        private TMP_InputField m_inputName;
        [SerializeField]
        private Button m_btnButtonOK;
        private void Start()
        {
            SetEvent(m_btnButtonOK, OnClickOKEvent);

            m_inputName.onValueChanged.AddListener(OnEndEditEvent);

            Init();

            SetEvent(tabName, OnClickTabName);
            SetEvent(tabAvatar, OnClickTabAvatar);
            //OnClickTabAvatar();
            SetEvent(editButton, ()=>
            {
                m_inputName.ActivateInputField();
            });

        }
        public override void Show()
        {
            base.Show();
            m_inputName.text = LeaderboardData.myData.name;
        }
        private void OnClickTabName()
        {
            TabChosen(tabName, true);
            TabChosen(tabAvatar, false);

            panelAvatar.SetActive(false);
            panelName.SetActive(true);
        }
        private void OnClickTabAvatar()
        {
            TabChosen(tabName, false);
            TabChosen(tabAvatar, true);

            panelAvatar.SetActive(true);
            panelName.SetActive(false);
        }
        
        private void TabChosen(Button btn, bool isChosen)
        {
            btn.image.sprite = isChosen ? tabOn : tabOff;
            btn.image.color = isChosen ? tabOnColorTxt : tabOffColorTxt;
            btn.GetComponentInChildren<TextMeshProUGUI>().color = isChosen ? tabOnColorTxt : tabOffColorTxt;
        }
        public void Init()
        {
            avatarUis.ForEach(x => x.gameObject.SetActive(false));
            var sprs = LeaderboardConfig.avatarSprites;

            for(int i = 0; i < sprs.Count; i++)
            {
                avatarUis[i].SetData(sprs[i], i, (x) =>
                {
                    LeaderboardData.ChangeAvatar(x.id);
                    avatarUis.ForEach(y => y.SetChosen(false));
                    x.SetChosen(true);
                });
                avatarUis[i].SetChosen(i == LeaderboardData.avatarId);
                avatarUis[i].gameObject.SetActive(true);
            }
        }

        public void UpdateUI()
        {
            avatarMain.sprite = LeaderboardConfig.avatarSprites[LeaderboardData.avatarId];
        }
        private void OnEndEditEvent(string arg0)
        {
            m_inputName.text = m_inputName.text.Replace(" ","");
            if (m_inputName.text.Length < 4 || m_inputName.text.Length > 12)
            {
                m_btnButtonOK.interactable = false;
            }
            else
                m_btnButtonOK.interactable = true;
        }
        private void OnClickOKEvent()
        {           
            if (m_inputName.text.Length < 4 || m_inputName.text.Length > 12)
            {
                Hide();
                return;
            }
            LeaderboardData.ChangeName(m_inputName.text);
            Hide();
        }
    }
}