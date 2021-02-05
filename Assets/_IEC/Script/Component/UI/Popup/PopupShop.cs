using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DanielLochner.Assets.SimpleScrollSnap;

namespace IEC
{
    public class PopupShop : MonoBehaviour
    {
        private PlayerSecret PlayerData => Pool.Get<PlayerSecret>();

        private Game Game => Pool.Get<Game>();

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

        private int m_BGCount, m_ThemeCount;

        private ShopConfig ShopConfig;

        [SerializeField]
        private Transform[] m_TransGroupTheme;

        [SerializeField]
        private Transform m_TransGroupBG;

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

        public void OnInit(ShopConfig shopConfig)
        {
            ShopConfig = shopConfig;
            m_ItemClaim.OnInit(GotoPanel);

            InitButtonEvent();

            var listThemeConfig = ShopConfig.listThemeConfigs;
            m_ThemeCount = listThemeConfig.Count;
            for (int i = 0; i < m_ThemeCount; i++)
            {
                var itemConfig = listThemeConfig[i];
                var itemObj = Instantiate(ShopConfig.itemShopObj, m_TransGroupTheme[i / 4]);
                var item = itemObj.GetComponent<ItemShop>();
                item.OnInit(i, itemConfig.itemShopConfig);
                m_ListItemTheme.Add(item);
                
                if(i == m_ThemeCount - 1)
                {
                    item.GetComponent<RectTransform>().SetAsFirstSibling();
                }
            }

            var listBGConfig = ShopConfig.listBGConfig;
            m_BGCount = listBGConfig.Count;
            for (int i = 0; i < m_BGCount; i++)
            {
                var itemConfig = listBGConfig[i];
                var itemObj = Instantiate(ShopConfig.itemShopObj, m_TransGroupBG);
                var item = itemObj.GetComponent<ItemShop>();
                item.OnInit(i, itemConfig.itemShopConfig);
                m_ListItemBG.Add(item);
            }

            OnSelectTab(0);
        }

        public void SetupAbUI()
        {
            var width = Game.isHaveNoAdsInShopTab ? 345f : 530f;
            var size = new Vector2(width, 118f);
            m_BtnTabBackground.GetComponent<RectTransform>().sizeDelta = size;
            m_BtnTabTheme.GetComponent<RectTransform>().sizeDelta = size;
            m_BtnTabNoAds.GetComponent<RectTransform>().sizeDelta = size;

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
            m_BtnRestorePurchase.gameObject.SetActive(PlayerData.isNoAds);
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
                itemShopConfig = ShopConfig.listThemeConfigs[index].itemShopConfig;
            }
            else if (type == ItemShopType.Background)
            {
                itemShopConfig = ShopConfig.listBGConfig[index].itemShopConfig;
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
            var ratio = Screen.height / (float)Screen.width;
            var scale = (1920f / 1080) / ratio;
            var sizeY = (1282f * scale) + 315f;
            var minPosY = 1920 - sizeY;
            var posY = (minPosY + bannerHeight) / 2f;
            m_RectBottomGroup.anchoredPosition = new Vector2(0f, posY);
            //m_BtnBuyItemByAds.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, posY);
        }

        private void OnHandleBuyItem(bool isbyAds)
        {
            var totalItem = 0;
            if(TabSelected == ItemShopType.Theme)
            {
                totalItem = m_ThemeCount;
            }else if(TabSelected == ItemShopType.Background)
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
