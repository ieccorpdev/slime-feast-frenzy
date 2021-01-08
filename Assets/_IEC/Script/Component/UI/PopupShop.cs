using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class PopupShop : MonoBehaviour
    {
        [SerializeField]
        private Button m_BtnTabBackground, m_BtnTabTheme;

        [SerializeField]
        private Button m_BtnBack, m_BtnBuyItemWatchAds;

        [SerializeField]
        private GameObject[] m_Tabs;

        private ItemShopType TabSelected;

        public void OnInit()
        {
            InitButtonEvent();
            OnSelectTab(0);
        }

        public void OnUpdateData(List<int> listBGUnlocked, List<int> listThemeUnlocked)
        {

        }

        private void InitButtonEvent()
        {
            m_BtnTabBackground.onClick.RemoveAllListeners();
            m_BtnTabBackground.onClick.AddListener(() => {
                OnSelectTab(0);
            });

            m_BtnTabTheme.onClick.RemoveAllListeners();
            m_BtnTabTheme.onClick.AddListener(() => {
                OnSelectTab(1);
            });

            m_BtnBack.onClick.RemoveAllListeners();
            m_BtnBack.onClick.AddListener(OnHide);

            m_BtnBuyItemWatchAds.onClick.RemoveAllListeners();
            m_BtnBuyItemWatchAds.onClick.AddListener(() => {
                EventController.RaiseEvent(EventList.GamePlay.SHOP_BUY_ITEM, new EventParam(this, TabSelected));
            });
        }

        public void OnSelectTab(int tabSelect)
        {
            TabSelected = (ItemShopType)tabSelect;
            for (int i = 0; i < m_Tabs.Length; i++)
            {
                m_Tabs[i].SetActive(i == tabSelect);
            }
        }

        private void OnHide()
        {
            gameObject.SetActive(false);
        }

    }
}
