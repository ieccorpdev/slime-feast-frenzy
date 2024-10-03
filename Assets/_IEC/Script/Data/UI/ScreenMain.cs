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
        [SerializeField] private GameObject m_Shop, m_Home, m_Tab2, m_Tab4, m_Tab5;
        private Dictionary<TabType, GameObject> m_DictTab = new Dictionary<TabType, GameObject>();
        private List<GameObject> m_ListTabObj = new List<GameObject>();
        private void Awake()
        {
            Init();
            gameObject.OnEvent(EventList.GamePlay.ON_CHOOSE_TAB, () =>
            {
                // Debug.LogError("OnChooseTab");
                HideAllTab();
                // Debug.LogError(Game.hybrid.currentTab);
                m_DictTab[Game.hybrid.currentTab].SetActive(true);
            });
            SetEvent(m_BtnSetting, () =>
            {
                UI.ShowScreen(Locator.PopupSetting);
            });
        }

        private void Init()
        {
            // Debug.LogError("Init");
            m_DictTab[TabType.Home] = m_Home;
            m_DictTab[TabType.Shop] = m_Shop;
            m_DictTab[TabType.Tab2] = m_Tab2;
            m_DictTab[TabType.Tab4] = m_Tab4;
            m_DictTab[TabType.Tab5] = m_Tab5;
            HideAllTab();
        }

        private void HideAllTab()
        {
            if (m_ListTabObj.Count == 0) m_ListTabObj = m_DictTab.Values.ToList();
            m_ListTabObj.ForEach(obj => obj.SetActive(false));
        }
    }
}