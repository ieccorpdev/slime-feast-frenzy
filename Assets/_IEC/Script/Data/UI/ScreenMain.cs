using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using IEC.Common.Event;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace IEC
{
    public class ScreenMain : ScreenUI
    {
        [SerializeField] private Button m_BtnSetting;
        [SerializeField] private GameObject m_ResourceUI, m_Shop, m_Home, m_Tab2, m_Tab4, m_Tab5;
        public RectTransform topBar; 
        public BottomBar bottomBar;
        private Dictionary<TabType, GameObject> m_DictTab = new Dictionary<TabType, GameObject>();
        private List<GameObject> m_ListTabObj = new List<GameObject>();
        private ScreenNavigateUI _currentScreen;
        
        public UICurrency GoldCurrencyDisplay => Locator.Service.GameResourceDisplay.GetDisplay(ResourceType.Gold);
        
        [Header("Avatar Button")] 
        public Button avatarButton;
        public Image avatarImage;

        [Header("Claim reward effect")] 
        public EffectRewardClaim effectRewardClaim;
        private Stack<RewardClaimData> listClaimRewardData = new Stack<RewardClaimData>();
        
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
                        //Locator.Service.MoneyPanel.Show();
                        //m_ResourceUI.SetActive(true);
                        Locator.Service.GameResourceDisplay.ToggleDisplayAllResources(true);
                        m_BtnSetting.gameObject.SetActive(true);
                        m_Shop.GetComponent<UIShopHomeTab>().SortVerticalGroups();
                        break;
                    case (TabType.Home):
                        //Locator.Service.MoneyPanel.Hide();
                        //m_ResourceUI.SetActive(true);
                        Locator.Service.GameResourceDisplay.ToggleDisplayAllResources(true);
                        m_BtnSetting.gameObject.SetActive(true);
                        break;     
                    case (TabType.LeaderBoard):
                        //m_ResourceUI.SetActive(false);
                        Locator.Service.GameResourceDisplay.ToggleDisplayAllResources(false);
                        m_BtnSetting.gameObject.SetActive(false);
                        EventController.RaiseEvent(EventList.WindLeaderboard.CHANGE_AVATAR);
                        break;
                    default:
                        Locator.Service.MoneyPanel.Hide();
                        Locator.Service.GameResourceDisplay.ToggleDisplayAllResources(false);
                        //m_ResourceUI.SetActive(true);
                        break;
                }

                // if (Game.hybrid.currentTab != TabType.Home)
                // {
                //     Locator.Main.HomeScreenTab.buildCityUI.HiddenBuildCity();
                // }
                
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
                        //screen.MoveMiddleShow();
                    }
                    else if (_currentScreen.tabType > Game.hybrid.currentTab)
                    {
                        _currentScreen.MoveRightHide();
                        screen.SetPosLeft();
                        //screen.MoveMiddleShow();
                    }   
                    screen.Show();
                    screen.MoveMiddleShow();
                }

                _currentScreen = screen;
            });
            SetEvent(m_BtnSetting, () =>
            {
                //UI.ShowScreen(Locator.Service.PopupSettingInHome);
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
            PlayAnimCollectedLeaderBoardRank();
            CheckBannerHome();
        }

        public void CheckBannerHome()
        {
            //MeoMew remove ads banner in home
        }
        private void HideAllTab()
        {
            if (m_ListTabObj.Count == 0) m_ListTabObj = m_DictTab.Values.ToList();
            m_ListTabObj.ForEach(obj => obj.SetActive(false));
        }

        public void PlayAnimCollectedLeaderBoardRank()
        {
            if (Game.isHaveAnimMedal)
            {
                Game.isHaveAnimMedal = false;
                //meomew update after finish collection anim , may be in show ? 
                //UI.medalEffect.AnimReward(LeaderboardConfig.GetSpirteCurrentLeague(), Vector3.zero, UI.menu.btnLeaderboard.transform.position);
            }
        }
        
        #region Player Avatar Profile

        public void UpdatePlayerAvatar()
        {
            avatarImage.sprite = PlayConfig.playerAvatarConfig.GetAvatarSprite(PlayerData.playerAvatarSpriteName);
        }

        #endregion
        
        #region Claim Reward Effect

        public void AddRewardClaimEffect(ResourceType resourceType, int amount)
        {
            switch (resourceType)
            {
                case ResourceType.Gold:
                    AddRewardGoldToClaim(amount);
                    break;
                case ResourceType.Vortex:
                    AddRewardHelperToClaim(resourceType, amount, 0.1f);
                    break;
                case ResourceType.Compass:
                    AddRewardHelperToClaim(resourceType, amount, 0.2f);
                    break;
                case ResourceType.Enlarge:
                    AddRewardHelperToClaim(resourceType, amount, 0.3f);
                    break;
                case ResourceType.TimeStop:
                    AddRewardHelperToClaim(resourceType, amount, 0.4f);
                    break;
            }
        }
        
        private void AddRewardHelperToClaim(ResourceType helperType, int amount, float delay = 0f)
        {

            
        }

        private void AddRewardGoldToClaim(int amount, float delay = 0f)
        {
            GoldCurrencyDisplay.ForceUpdateCurrencyText(PlayerData.GetPlayerCurrencyCount(ResourceType.Gold) - amount);
            GoldCurrencyDisplay.SetIsForcedCurrencyDisplay(true);
            AddCustomRewardToClaimEffect(
                ResourceType.Gold, 
                Vector2.zero, 
                EffectRewardClaim.SwitchToRectTransform(Locator.Main.ScreenMain.GoldCurrencyDisplay.GetComponent<RectTransform>(), Locator.Main.ScreenMain.effectRewardClaim.GetComponent<RectTransform>()),
                amount,
                () =>
                {
                    GoldCurrencyDisplay.SetIsForcedCurrencyDisplay(false);
                    //goldCurrencyDisplay.UpdateCurrency();
                    EventController.RaiseEvent(EventList.GamePlay.UPDATE_RESOURCE_UI);
                },
                () =>
                {
                    GoldCurrencyDisplay.ForceUpdateCurrencyText(GoldCurrencyDisplay.GetCurrentCurrencyTextAmount() + Mathf.FloorToInt((float)amount / effectRewardClaim.maxSpawn));
                },
                delay
            );
        }

        public void AddCustomRewardToClaimEffect(ResourceType rewardType, Vector2 posSpawn, Vector2 posEnd, int addAmount, Action onComplete = null,Action onEndMove = null, float delay = 0f)
        {
            listClaimRewardData.Push(new RewardClaimData(rewardType, posSpawn, posEnd, addAmount, onComplete,onEndMove, delay));
        }

        public void DoAnimAllAvailableReward()
        {
            while (listClaimRewardData.Count > 0)
            {
                // var rewardData = listClaimRewardData.Pop();
                //
                // effectRewardClaim.SpawnEffect(rewardData);
                DoAnimLatestAvailableReward();
            }
        }

        public void DoAnimLatestAvailableReward()
        {
            var rewardData = listClaimRewardData.Pop();
            
            effectRewardClaim.SpawnEffect(rewardData);
        }

        #endregion
        public bool OnMainHomeScreen()
        {
            if (_currentScreen == null) return false;
            return _currentScreen.tabType == TabType.Home;
        }
        public bool BuildCityCheckShow()
        {
            if (_currentScreen == null) return true;
            return _currentScreen.tabType == TabType.Home;
        }
    }
}