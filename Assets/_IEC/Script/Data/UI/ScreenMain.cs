using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using IEC.Common.Event;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class ScreenMain : ScreenUI
    {
        [SerializeField] private Button m_BtnSetting;
        [SerializeField] private GameObject m_ResourceUI, m_Shop, m_Home, m_Tab2, m_Tab4, m_Tab5;
        private Dictionary<TabType, GameObject> m_DictTab = new Dictionary<TabType, GameObject>();
        private List<GameObject> m_ListTabObj = new List<GameObject>();
        private ScreenNavigateUI _currentScreen;
        [Header("Avatar Button")] 
        public Button avatarButton;
        public Image avatarImage;
        private void Awake()
        {
            Init();
            gameObject.OnEvent(EventList.GamePlay.ON_CHOOSE_TAB, () =>
            {
                if(m_DictTab[Game.hybrid.currentTab].activeInHierarchy)
                    return;
                // Debug.LogError("OnChooseTab");
                //HideAllTab();
                // Debug.LogError(Game.hybrid.currentTab);
                switch (Game.hybrid.currentTab)
                {
                    case (TabType.Shop):
                        Locator.Service.MoneyPanel.Show();
                        m_ResourceUI.SetActive(false);
                        break;
                    case (TabType.Home):
                        Locator.Service.MoneyPanel.Hide();
                        m_ResourceUI.SetActive(true);
                        break;
                    default:
                        Locator.Service.MoneyPanel.Hide();
                        m_ResourceUI.SetActive(true);
                        break;
                }
                
                m_DictTab[Game.hybrid.currentTab].SetActive(true);
                var screen = m_DictTab[Game.hybrid.currentTab].GetComponent<ScreenNavigateUI>();
                if(_currentScreen != null)
                {
                    if (_currentScreen.tabType == Game.hybrid.currentTab)
                    {
                        return;
                    }
                    else if (_currentScreen.tabType < Game.hybrid.currentTab)
                    {
                        _currentScreen.MoveLeftHide();
                        screen.SetPosRight();
                        screen.MoveMiddleShow();
                    }
                    else if (_currentScreen.tabType > Game.hybrid.currentTab)
                    {
                        _currentScreen.MoveRightHide();
                        screen.SetPosLeft();
                        screen.MoveMiddleShow();
                    }
                }

                _currentScreen = screen;
            });
            SetEvent(m_BtnSetting, () =>
            {
                UI.ShowScreen(Locator.Service.PopupSetting);
            });
        }

        private void Init()
        {
            // Debug.LogError("Init");
            m_DictTab[TabType.Home] = m_Home;
            m_DictTab[TabType.Shop] = m_Shop;
            m_DictTab[TabType.Tab2] = m_Tab2;
            m_DictTab[TabType.LeaderBoard] = m_Tab4;
            m_DictTab[TabType.Tab5] = m_Tab5;
            HideAllTab();
        }

        private void HideAllTab()
        {
            if (m_ListTabObj.Count == 0) m_ListTabObj = m_DictTab.Values.ToList();
            m_ListTabObj.ForEach(obj => obj.SetActive(false));
        }

        #region Player Avatar Profile

        public void UpdatePlayerAvatar()
        {
            avatarImage.sprite = Config.playerAvatarConfig.GetAvatarSprite(PlayerData.playerAvatarSpriteName);
        }

        #endregion
    }
}