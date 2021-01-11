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

        private List<ItemShop> m_ListItemTheme = new List<ItemShop>();
        private List<ItemShop> m_ListItemBG = new List<ItemShop>();

        private int m_BGCount, m_ThemeCount;

        [SerializeField]
        private Transform[] m_TransGroupTheme;

        [SerializeField]
        private Transform m_TransGroupBG;

        public void OnInit(ShopConfig shopConfig)
        {
            InitButtonEvent();

            var listThemeConfig = shopConfig.listThemeConfigs;
            m_ThemeCount = listThemeConfig.Count;
            for (int i = 0; i < m_ThemeCount; i++)
            {
                var itemConfig = listThemeConfig[i];
                var itemObj = Instantiate(shopConfig.itemShopObj, m_TransGroupTheme[i / 4]);
                var item = itemObj.GetComponent<ItemShop>();
                item.OnInit(i, itemConfig.itemShopConfig);
                m_ListItemTheme.Add(item);
            }

            OnSelectTab(0);
        }

        public void OnUpdateData(List<int> listThemeUnlocked, int themeSelected, List<int> listBGUnlocked, int bgSelected)
        {
            for (int i = 0; i < m_ThemeCount; i++)
            {
                m_ListItemTheme[i].OnUpdateData(listThemeUnlocked.Contains(i), i == themeSelected);
            }

            for (int i = 0; i < m_BGCount; i++)
            {
                m_ListItemBG[i].OnUpdateData(listBGUnlocked.Contains(i), i == bgSelected);
            }
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
