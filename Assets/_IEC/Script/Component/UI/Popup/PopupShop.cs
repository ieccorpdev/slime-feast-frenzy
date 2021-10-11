using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DanielLochner.Assets.SimpleScrollSnap;

namespace IEC
{
    public class PopupShop : _BaseBehaviour
    {
        [SerializeField]
        private Button m_BtnTabBackground, m_BtnTabTheme, m_BtnTabNoAds;

        [SerializeField]
        private Button m_BtnBack, m_BtnBuyItemByAds, m_BtnBuyItemByGold, m_BtnAddGolds, 
            m_BtnNoAds, m_BtnRestorePurchase;

        [SerializeField]
        private GameObject[] m_Tabs, m_BtnNotSelect, m_BtnSelect;

        private ItemShopType TabSelected;

        private List<ItemShop> m_ListItemTheme = new List<ItemShop>();
        private List<ItemShop> m_ListItemBG = new List<ItemShop>();

        [SerializeField]
        private TextMeshProUGUI m_TxtTitle, m_TxtGoldBuyItem, m_TxtTimeCountDown;

        private static string[] SHOP_TITLE = new string[] {"THEME", "BACKGROUND", "NO ADS" };

        [SerializeField]
        private ItemClaimShop m_ItemClaim;

        [SerializeField]
        private SimpleScrollSnap[] m_ScrollSnap;

        [SerializeField]
        private RectTransform m_RectBottomGroup;

        [SerializeField]
        private GameObject m_GoldObj, m_PurchasedObj;

        private List<Transform> m_ListTransTabTheme, m_ListTransTabBG;
        private List<GameObject> m_ListToggleThemes, m_ListToggleBG;

        [SerializeField]
        private GameObject m_TabThemeObj, m_TabBGObj;

        [SerializeField]
        private GameObject m_ToggleThemeObj, m_ToggleBGObj;

        private int m_BGCount, m_ThemeCount;
        private bool m_IsInit;

        public void OnInit()
        {
            if (!m_IsInit)
            {
                m_IsInit = true;
                m_ItemClaim.OnInit(GotoPanel);
                InitButtonEvent();

                m_ListTransTabTheme = new List<Transform>();
                m_ListTransTabBG = new List<Transform>();

                m_ListToggleThemes = new List<GameObject>();
                m_ListToggleBG = new List<GameObject>();
            }

            m_ListItemTheme.ForEach(x => x.OnDestroyMe());
            m_ListItemTheme.Clear();

            m_ListItemBG.ForEach(x => x.OnDestroyMe());
            m_ListItemBG.Clear();

            for (int i = 1; i < m_ListTransTabTheme.Count; i++)
            {
                Destroy(m_ListTransTabTheme[i].gameObject);
            }

            m_ListTransTabTheme.Clear();
            m_ListTransTabTheme.Add(m_TabThemeObj.transform);

            for (int i = 1; i < m_ListToggleThemes.Count; i++)
            {
                Destroy(m_ListToggleThemes[i]);
            }

            m_ListToggleThemes.Clear();
            m_ListToggleThemes.Add(m_ToggleThemeObj);

            var listThemeConfig = ShopConfig.listThemeConfigs;
            m_ThemeCount = Game.addThemeVersion == 2 ? listThemeConfig.Count : 5;
            var totalTabTheme = Mathf.CeilToInt(m_ThemeCount / 4f);

            if(totalTabTheme > 1)
            {
                for (int i = 1; i < totalTabTheme; i++)
                {
                    var tabObj = Instantiate(m_TabThemeObj, m_TabThemeObj.transform.parent);
                    m_ListTransTabTheme.Add(tabObj.transform);

                    var toggleObj = Instantiate(m_ToggleThemeObj, m_ToggleThemeObj.transform.parent);
                    m_ListToggleThemes.Add(toggleObj);
                }
            }

            for (int i = 0; i < m_ThemeCount; i++)
            {
                var itemConfig = listThemeConfig[i];
                var itemObj = Instantiate(ShopConfig.itemShopObj, m_ListTransTabTheme[i / 4]);
                var item = itemObj.GetComponent<ItemShop>();
                item.OnInit(i, itemConfig);
                m_ListItemTheme.Add(item);
            }


            for (int i = 1; i < m_ListTransTabBG.Count; i++)
            {
                Destroy(m_ListTransTabBG[i]);
            }

            m_ListTransTabBG.Clear();
            m_ListTransTabBG.Add(m_TabBGObj.transform);

            for (int i = 1; i < m_ListToggleBG.Count; i++)
            {
                Destroy(m_ListToggleBG[i]);
            }
            m_ListToggleBG.Clear();
            m_ListToggleBG.Add(m_ToggleBGObj);

            var listBGConfig = ShopConfig.listBGConfig;
            m_BGCount = Game.addBackgroundVersion == 2 ? listBGConfig.Count : 4;
            var totalTabBG = Mathf.CeilToInt(m_BGCount / 4f);

            if (totalTabBG > 1)
            {
                for (int i = 1; i < totalTabBG; i++)
                {
                    var tabObj = Instantiate(m_TabBGObj, m_TabBGObj.transform.parent);
                    m_ListTransTabBG.Add(tabObj.transform);


                    var toggleObj = Instantiate(m_ToggleBGObj, m_ToggleBGObj.transform.parent);
                    m_ListToggleThemes.Add(toggleObj);
                }
            }

            for (int i = 0; i < m_BGCount; i++)
            {
                var itemConfig = listBGConfig[i];
                var itemObj = Instantiate(ShopConfig.itemShopObj, m_ListTransTabBG[i / 4]);
                var item = itemObj.GetComponent<ItemShop>();
                item.OnInit(i, itemConfig);
                m_ListItemBG.Add(item);
            }

            OnSelectTab(0);
        }

        public void SetupAbUI()
        {
            OnInit();
            m_BtnTabNoAds.gameObject.SetActive(Game.isHaveNoAdsInShopTab);
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

            m_BtnTabNoAds.onClick.RemoveAllListeners();
            m_BtnTabNoAds.onClick.AddListener(() => {
                OnSelectTab(2);
            });

            m_BtnBack.onClick.RemoveAllListeners();
            m_BtnBack.onClick.AddListener(OnHide);

            m_BtnBuyItemByAds.onClick.RemoveAllListeners();
            m_BtnBuyItemByAds.onClick.AddListener(() => {
                OnHandleBuyItem(true);
            });

            m_BtnBuyItemByGold.onClick.RemoveAllListeners();
            m_BtnBuyItemByGold.onClick.AddListener(() => {
                OnHandleBuyItem(false);
            });

            m_BtnAddGolds.onClick.RemoveAllListeners();
            m_BtnAddGolds.onClick.AddListener(() => {
                EventController.RaiseEvent(EventList.GamePlay.SHOP_ADD_GOLD);
            });

            m_BtnNoAds.onClick.RemoveAllListeners();
            m_BtnNoAds.onClick.AddListener(() => {
                EventController.RaiseEvent(EventList.GamePlay.IAP_BUY_PACKAGES, new EventParam(this, IapPackages.NoAds));
            });

            m_BtnRestorePurchase.onClick.RemoveAllListeners();
            m_BtnRestorePurchase.onClick.AddListener(() => {
                EventController.RaiseEvent(EventList.GamePlay.IAP_RESTORE_NO_ADS);
            });
        }

        public void SetActiveNoAds()
        {
            m_PurchasedObj.SetActive(PlayerData.isNoAds);
            m_BtnNoAds.gameObject.SetActive(!PlayerData.isNoAds);
        }

        private bool m_IsOldStatus = true, m_IsActiveTimer;
        public void SetCountDown()
        {
            m_IsActiveTimer = Game.timeLeftAddGold > 0;

            if (m_IsActiveTimer)
            {
                m_TxtTimeCountDown.text = StringHelper.GetTime((int)Game.timeLeftAddGold);
            }
            if (m_IsActiveTimer != m_IsOldStatus)
            {
                m_IsOldStatus = m_IsActiveTimer;
                m_TxtTimeCountDown.gameObject.SetActive(m_IsActiveTimer);
                m_GoldObj.SetActive(!m_IsActiveTimer);
                m_BtnAddGolds.interactable = !m_IsActiveTimer;
            }
        }

        public void OnResetAll()
        {
            m_ItemClaim.OnResetAll();
            m_ItemClaim.gameObject.SetActive(false);
            SetActiveNoAds();
        }

        public void OnUpdateData()
        {
            for (int i = 0; i < m_ThemeCount; i++)
            {
                m_ListItemTheme[i].OnUpdateData(PlayerData.listThemeUnlocked.Contains(i), i == PlayerData.themeSelected);
            }

            for (int i = 0; i < m_BGCount; i++)
            {
                m_ListItemBG[i].OnUpdateData(PlayerData.listBackgroundUnlocked.Contains(i), i == PlayerData.backgroundSelected);
            }

            OnUpdateButtonBuy();
        }

        private void OnUpdateButtonBuy()
        {
            OnUpdateBuyItemStatus(PlayerData.listThemeUnlocked, PlayerData.listBackgroundUnlocked);
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

            m_BtnBuyItemByAds.interactable = isAtive;
            m_BtnBuyItemByGold.interactable = isAtive && (PlayerData.gold >= ShopController.GetGoldItem(TabSelected));
        }

        public void EffectBuyItem(ItemShopType type, int index)
        {
            ItemShopConfig itemShopConfig = null;
            if (type == ItemShopType.Theme)
            {
                itemShopConfig = ShopConfig.listThemeConfigs[index];
            }
            else if (type == ItemShopType.Background)
            {
                itemShopConfig = ShopConfig.listBGConfig[index];
            }

            m_ItemClaim.gameObject.SetActive(true);
            m_ItemClaim.ShowEffect(itemShopConfig.spriteIcon, itemShopConfig.name);
        }

        private void GotoPanel()
        {
            var index = GetIndex(TabSelected);
            m_ScrollSnap[(int)TabSelected].GoToPanel(index);
        }

        private int GetIndex(ItemShopType type)
        {
            if(type == ItemShopType.Theme)
            {
                return PlayerData.themeSelected / 4;
            }
            else
            {
                return PlayerData.backgroundSelected / 4;
            }
        }

        public void SetupPositionBtnBuy(float bannerHeight)
        {

        }

        private void OnHandleBuyItem(bool isbyAds)
        {
            var totalItem = 0;
            if(TabSelected == ItemShopType.Theme)
            {
                totalItem = m_ThemeCount;
            }
            else if(TabSelected == ItemShopType.Background)
            {
                totalItem = m_BGCount;
            }

            EventController.RaiseEvent(EventList.GamePlay.SHOP_BUY_ITEM, new EventParam(this, isbyAds, TabSelected, totalItem));
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
            m_TxtGoldBuyItem.text = StringHelper.GetNumber(ShopController.GetGoldItem(TabSelected));
            OnUpdateButtonBuy();

            m_RectBottomGroup.gameObject.SetActive(tabSelect != 2);
        }

        private void OnHide()
        {
            gameObject.SetActive(false);
            Game.isShowPopup = false;
            OnResetAll();
            Pool.Get<UIGroup>().SetActiveGroupGold(false);
        }
    }
}
