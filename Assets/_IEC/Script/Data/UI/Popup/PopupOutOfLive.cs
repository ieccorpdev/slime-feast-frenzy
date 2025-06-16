using System;
using DG.Tweening;
using IEC;
using IEC.TrackingEvent;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _IEC
{
    public class PopupOutOfLive : BasePopup
    {
        public Button refillBtn, adsMoreLifeBtn, closeNBtn;
        public TextMeshProUGUI goldRefillTMP, timeNextLifeTMP;
        public TextMeshProUGUI playerLifeText;
        public TextMeshProUGUI playerLifeAddWithGoldText;
        public RectTransform countdownGroup, fullGroup;
        private bool InClose;
        Vector2 pos = new Vector2(300, -1250);

        private bool IsRaiseToMaxLife = false;

        protected override void Awake()
        {
            base.Awake();
            refillBtn.onClick.AddListener(OnClickButtonRefill);
            adsMoreLifeBtn.onClick.AddListener(OnClickButtonAdsMoreLife);
        }

        void OnClickButtonRefill()
        {
            OnRequestRefill(PlayerData.SubtractCurrency(ResourceType.Gold, IsRaiseToMaxLife ? PlayConfig.goldToRefill : PlayConfig.goldToAddOneLife));
        }
        void OnClickButtonAdsMoreLife()
        {
            
        }

        void OnRequestRefill(bool success)
        {
            if (success)
            {
                //UI.GetGroupsGold().EffectLifeClaim(pos, Config.maxHealth, PlayerData.playerLife, () => { });
                if (IsRaiseToMaxLife)
                {
                    PlayerData.RaiseToMaxHealth();
                }
                else
                {
                    PlayerData.AddPlayerLife();
                }

                Locator.Main.HomeScreenTab.UpdateMainHome();
                //UI.GetGroupsGold().UpdateUI();
                //UI.screenHome.Hide();
                Hide();
                EventController.RaiseEvent(EventList.GamePlay.UPDATE_RESOURCE_UI);
                //EventController.RaiseEvent(EventList.GamePlay.GAME_INIT, new EventParam() { p0 = GameController.GameType.NormalLevel });
            }
            else
            {
                Locator.Main.PopupGoldOffer.Show();
                //Hide();
            }
        }

        private void UpdateUI()
        {
            countdownGroup.gameObject.SetActive(PlayerData.playerLife < PlayConfig.maxHealth);
            fullGroup.gameObject.SetActive(PlayerData.playerLife == PlayConfig.maxHealth);
            playerLifeText.text = PlayerData.playerLife.ToString();
        }

        void Update()
        {
            timeNextLifeTMP.text = PlayerData.GetTimeToNextLife();
        }
        public override void Show()
        {
            SetupData();
            base.Show();
            UpdateUI();
        }
        void SetupData()
        {
            IsRaiseToMaxLife = true;
            InClose = false;
            closeNBtn.onClick.RemoveAllListeners();
            closeNBtn.onClick.AddListener(OnCloseButton);
            goldRefillTMP.text = IsRaiseToMaxLife ? PlayConfig.goldToRefill.ToString() : PlayConfig.goldToAddOneLife.ToString();
            playerLifeAddWithGoldText.text = IsRaiseToMaxLife ? "+5" : "+1";
            //refillBtn.image.sprite = PlayerData.gold >= Config.goldToRefill ? spriteCanBuy : spriteOutOfGold;
            //StartAnim();
        }
        public void OnCloseButton()
        {
            if (!InClose)
            {
                InClose = true;
                Hide();
            }

        }
        public override void Hide()
        {
            base.Hide();
        }
        public void StartAnim()
        {
            closeNBtn.gameObject.SetActive(false);
            refillBtn.gameObject.SetActive(false);
            timeNextLifeTMP.gameObject.SetActive(false);
            adsMoreLifeBtn.gameObject.SetActive(false);
        }
        // DOTweenModuleUtils
    }
}
