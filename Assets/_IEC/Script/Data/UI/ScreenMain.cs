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
        public GameObject shopTab => m_Shop;
        public UICurrency GoldCurrencyDisplay => Locator.Service.GameResourceDisplay.GetDisplay(ResourceType.Gold);
        public UICurrency LifeCurrencyDisplay => Locator.Service.GameResourceDisplay.GetDisplay(ResourceType.Live);
        
        [Header("Avatar Button")] 
        public Button avatarButton;
        public Image avatarImage;

        [Header("Claim reward effect")] 
        public EffectRewardClaim effectRewardClaim;
        public Stack<RewardClaimData> listClaimRewardData = new();
        
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
                        Services.PushEvent(new IEC.TrackingEvent.ClickBtnTracking("icon_home_shop_iap"));
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
                        Services.PushEvent(new IEC.TrackingEvent.ClickBtnTracking("icon_home_leaderboard"));
                        //EventController.RaiseEvent(EventList.WindLeaderboard.CHANGE_AVATAR);
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
                UI.ShowScreen(Locator.Service.PopupSettingInHome);
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
            Services.SetActiveBanner(AdsEnum.BANNER_BOTTOM, false);
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
            avatarImage.sprite = Config.playerAvatarConfig.GetAvatarSprite(PlayerData.playerAvatarSpriteName);
        }

        #endregion
        
        #region Claim Reward Effect
        public void AddRewardClaimEffect(ResourceType resourceType, int amount)
        {
            AddRewardClaimEffect(resourceType,amount,Vector3.zero, Vector2.one*100);
        }
        public void AddRewardClaimEffect(ResourceType resourceType, int amount,Vector3 startPos,Vector2 startSize)
        {
            switch (resourceType)
            {
                case ResourceType.Gold:
                    AddRewardGoldToClaim(amount,startPos,startSize);
                    break;           
                case ResourceType.Live:
                    AddRewardLifeToClaim(amount,startPos,startSize);
                    break;
                case ResourceType.Vortex:
                    AddRewardHelperToClaim(resourceType, amount,startPos,startSize,1f,0.3f);
                    break;
                case ResourceType.Compass:
                    AddRewardHelperToClaim(resourceType, amount,startPos,startSize,1f,0.3f);
                    break;
                case ResourceType.Enlarge:
                    AddRewardHelperToClaim(resourceType, amount,startPos,startSize,1f,0.3f);
                    break;
                case ResourceType.TimeStop:
                    AddRewardHelperToClaim(resourceType, amount,startPos,startSize,1f,0.3f);
                    break;
                case ResourceType.TimeUnlimitedEvolveStone:
                    AddRewardHelperToClaim(resourceType, amount,startPos,startSize,1f,0.3f);
                    break;
                case ResourceType.TimeUnlimitedWindSurf:
                    AddRewardHelperToClaim(resourceType, amount,startPos,startSize,1f,0.3f);
                    break;
                case ResourceType.EvolveStone:
                    AddRewardHelperToClaim(resourceType, amount,startPos,startSize,1f,0.3f);
                    break;
                case ResourceType.WindSurf:
                    AddRewardHelperToClaim(resourceType, amount,startPos,startSize,1f,0.3f);
                    break;
                case ResourceType.OceanGiftObject:
                    AddSpecialObjectToClaim(amount,startPos,startSize, 1f);
                    break;
                case ResourceType.LevelStar:
                    AddLevelStarToClaim(amount,startPos,startSize, 1f, 0.15f);
                    break;
            }
        }

        private void AddLevelStarToClaim(int amount,Vector3 startPos,Vector2 startSize, float animTime = 1f, float delay = 0f)
        {
            RectTransform targetTransform = Locator.Main.HomeScreenTab.btnStarChestReward.GetComponent<ButtonClickEffect>().buttonDisplayImage.GetComponent<RectTransform>();
            
            AddCustomRewardToClaimEffect(
                ResourceType.LevelStar, 
                Config.dataEffectRewardClaimConfig.GetRewardSprite(ResourceType.LevelStar),
                startPos, 
                EffectRewardClaim.SwitchToRectTransform(targetTransform, Locator.Main.ScreenMain.effectRewardClaim.GetComponent<RectTransform>()),
                amount,
                startSize,
                () =>
                {
                    
                },
                () =>
                {
                    DOTween.Kill(targetTransform);
                    targetTransform.localScale = Vector3.one;
                    targetTransform.DOPunchScale(Vector3.one * 0.1f, 0.2f);
                },
                delay,
                animTime
            );
        }

        private void AddSpecialObjectToClaim(int amount,Vector3 startPos,Vector2 startSize, float animTime = 1f, float delay = 0f)
        {
            RectTransform targetTransform = Locator.Main.HomeScreenTab.progressRewardBar.GetComponent<RectTransform>();
            
            AddCustomRewardToClaimEffect(
                ResourceType.OceanGiftObject, 
                Config.specialObjectConfig.GetSpecialItemDefinition(PlayerData.currentSpecialItemIndex).GetIcon(),
                startPos, 
                EffectRewardClaim.SwitchToRectTransform(targetTransform, Locator.Main.ScreenMain.effectRewardClaim.GetComponent<RectTransform>()),
                amount,
                startSize,
                () =>
                {
                    
                },
                () =>
                {
                    DOTween.Kill(targetTransform);
                    targetTransform.localScale = Vector3.one;
                    targetTransform.DOPunchScale(Vector3.one * 0.1f, 0.2f);
                },
                delay,
                animTime
            );
        }
        
        private void AddRewardHelperToClaim(ResourceType helperType, int amount,Vector3 startPos,Vector2 startSize, float animTime = 1f, float delay = 0f)
        {
            RectTransform targetTransform = bottomBar.BigHomeTab.type == TabType.Home
                ? Locator.Main.HomeScreenTab.playButton.GetComponent<RectTransform>()
                : bottomBar.GetTab(TabType.Home).IconTransform
                ;
            
            AddCustomRewardToClaimEffect(
                helperType, 
                Config.dataEffectRewardClaimConfig.GetRewardSprite(helperType),
                startPos, 
                EffectRewardClaim.SwitchToRectTransform(targetTransform, Locator.Main.ScreenMain.effectRewardClaim.GetComponent<RectTransform>()),
                amount,
                startSize,
                () =>
                {
                    
                },
                () =>
                {
                    DOTween.Kill(targetTransform);
                    targetTransform.localScale = Vector3.one;
                    targetTransform.DOPunchScale(Vector3.one * 0.1f, 0.2f);
                },
                delay,
                animTime
            );
        }

        private void AddRewardGoldToClaim(int amount,Vector3 startPos,Vector2 startSize, float animTime = 2f, float delay = 0f)
        {
            GoldCurrencyDisplay.ForceUpdateCurrencyText(PlayerData.GetPlayerCurrencyCount(ResourceType.Gold) - amount);
            GoldCurrencyDisplay.SetIsForcedCurrencyDisplay(true);
            AddCustomRewardToClaimEffect(
                ResourceType.Gold, 
                Config.dataEffectRewardClaimConfig.GetRewardSprite(ResourceType.Gold),
                startPos, 
                EffectRewardClaim.SwitchToRectTransform(Locator.Main.ScreenMain.GoldCurrencyDisplay.targetIcon.rectTransform, Locator.Main.ScreenMain.effectRewardClaim.GetComponent<RectTransform>()),
                amount,
                startSize,
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
                delay,
                animTime
            );
        }
        private void AddRewardLifeToClaim(int amount,Vector3 startPos,Vector2 startSize, float animTime = 2f, float delay = 0f)
        {
            LifeCurrencyDisplay.ForceUpdateCurrencyText(PlayerData.playerLife - amount);
            LifeCurrencyDisplay.SetIsForcedCurrencyDisplay(true);
            AddCustomRewardToClaimEffect(
                ResourceType.Gold, 
                Config.dataEffectRewardClaimConfig.GetRewardSprite(ResourceType.Live),
                startPos, 
                EffectRewardClaim.SwitchToRectTransform(Locator.Main.ScreenMain.LifeCurrencyDisplay.GetComponent<RectTransform>(), Locator.Main.ScreenMain.effectRewardClaim.GetComponent<RectTransform>()),
                amount,
                startSize,
                () =>
                {
                    LifeCurrencyDisplay.SetIsForcedCurrencyDisplay(false);
                    //goldCurrencyDisplay.UpdateCurrency();
                    EventController.RaiseEvent(EventList.GamePlay.UPDATE_RESOURCE_UI);
                },
                () =>
                {
                    LifeCurrencyDisplay.ForceUpdateCurrencyText(LifeCurrencyDisplay.GetCurrentCurrencyTextAmount() + Mathf.FloorToInt((float)amount / effectRewardClaim.maxSpawn));
                },
                delay,
                animTime
            );
        }
        
        
        public void AddCustomRewardToClaimEffect(ResourceType rewardType,Sprite rewardSprite, Vector2 posSpawn, Vector2 posEnd, int addAmount,Vector2 startSize, Action onComplete = null,Action onEndMove = null, float delay = 0f, float animTime = 1f)
        {
           var dataEffect = new RewardClaimData(rewardType, rewardSprite, posSpawn, posEnd, addAmount, onComplete, onEndMove, delay, animTime);
           dataEffect.SetStartSize(startSize);
            listClaimRewardData.Push(dataEffect);
        }

        public void DoAnimAllAvailableReward(Action onComplete = null,float delayEachStep =0.5f)
        {
            StartCoroutine(DoAnimRewardWhileLoop(onComplete,delayEachStep));
        }

        private IEnumerator DoAnimRewardWhileLoop(Action onComplete,float delayEachStep)
        {
            while (listClaimRewardData.Count > 0)
            {
                var data = listClaimRewardData.Peek();
                float timeWait = data.delay;
                if (onComplete != null && listClaimRewardData.Count == 1)
                {
                    DoAnimLatestAvailableReward(onComplete);
                }
                else
                {
                    DoAnimLatestAvailableReward();
                }

                if (delayEachStep != 0f)
                {
                    yield return new WaitForSeconds(delayEachStep+0.5f); // Delay between iterations   
                }

            }
        }

        public void DoAnimLatestAvailableReward(Action onComplete = null)
        {
            var rewardData = listClaimRewardData.Pop();

            if (onComplete != null)
            {
                rewardData.onComplete += onComplete;
            }
            
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