using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace IEC
{
    public class PopupShop : MonoBehaviour
    {
        [SerializeField]
        private Button m_BtnTabBackground, m_BtnTabTheme;

        [SerializeField]
        private Button m_BtnBack, m_BtnBuyItemWatchAds;

        [SerializeField]
        private GameObject[] m_Tabs, m_BtnNotSelect, m_BtnSelect;

        private ItemShopType TabSelected;

        private List<ItemShop> m_ListItemTheme = new List<ItemShop>();
        private List<ItemShop> m_ListItemBG = new List<ItemShop>();

        private int m_BGCount, m_ThemeCount;

        [SerializeField]
        private Transform[] m_TransGroupTheme;

        [SerializeField]
        private Transform m_TransGroupBG;

        [SerializeField]
        private TextMeshProUGUI m_TxtTitle;

        private static string[] SHOP_TITLE = new string[] {"THEME", "BACKGROUND" };

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

            var listBGConfig = shopConfig.listBGConfig;
            m_BGCount = listBGConfig.Count;
            for (int i = 0; i < m_BGCount; i++)
            {
                var itemConfig = listBGConfig[i];
                var itemObj = Instantiate(shopConfig.itemShopObj, m_TransGroupBG);
                var item = itemObj.GetComponent<ItemShop>();
                item.OnInit(i, itemConfig.itemShopConfig);
                m_ListItemBG.Add(item);
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

            OnUpdateBuyItemStatus(listThemeUnlocked, listBGUnlocked);
        }

        private void OnUpdateBuyItemStatus(List<int> listThemeUnlocked, List<int> listBGUnlocked)
        {
            bool isAtive = false;
            if (TabSelected == ItemShopType.Theme)
            {
                isAtive = listThemeUnlocked.Count < m_ThemeCount;
            }
            else if (TabSelected == ItemShopType.Background)
            {
                isAtive = listBGUnlocked.Count < m_BGCount;
            }

            m_BtnBuyItemWatchAds.interactable = isAtive;
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
            m_BtnBuyItemWatchAds.onClick.AddListener(OnHandleBuyItem);
        }

        private void OnHandleBuyItem()
        {
            var totalItem = 0;
            if(TabSelected == ItemShopType.Theme)
            {
                totalItem = m_ThemeCount;
            }else if(TabSelected == ItemShopType.Background)
            {
                totalItem = m_BGCount;
            }

            EventController.RaiseEvent(EventList.GamePlay.SHOP_BUY_ITEM, new EventParam(this, TabSelected, totalItem));
        }

        public void OnSelectTab(int tabSelect)
        {
            TabSelected = (ItemShopType)tabSelect;
            for (int i = 0; i < m_Tabs.Length; i++)
            {
                var isActive = i == tabSelect;
                m_Tabs[i].SetActive(isActive);
                m_BtnSelect[i].SetActive(isActive);
                m_BtnNotSelect[i].SetActive(!isActive);
            }

            m_TxtTitle.text = SHOP_TITLE[tabSelect];
        }

        private void OnHide()
        {
            gameObject.SetActive(false);
        }

    }
}
