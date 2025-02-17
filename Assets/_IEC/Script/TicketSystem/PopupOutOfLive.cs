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
        public SpineAnimOT panelAnim;
        public Sprite spriteCanBuy;
        public Sprite spriteOutOfGold;
        private bool InClose;
        Vector2 pos = new Vector2(300, -1250);

        [SerializeField] GameObject offerVipPass;

        protected override void Awake()
        {
            base.Awake();
            refillBtn.onClick.AddListener(OnClickButtonRefill);
            adsMoreLifeBtn.onClick.AddListener(OnClickButtonAdsMoreLife);
        }

        void OnClickButtonRefill()
        {
            //PlayerData.UseCurrencyBuyItem(CurrencyType.Gold, Config.goldToRefill, OnRequestRefill);
        }
        void OnClickButtonAdsMoreLife()
        {
            Services.ShowRewardAds(() =>
            {
                PlayerData.AddPlayerLife();
                //UI.screenHome.Hide();
                // UI.GetGroupsGold().EffectLifeClaim(pos, 1, PlayerData.playerLife, () =>
                // {
                //     UI.GetGroupsGold().UpdateUI();
                // });
                Hide();
                //EventController.RaiseEvent(EventList.GamePlay.GAME_INIT, new EventParam() { p0 = GameController.GameType.NormalLevel });
            }, null);
        }

        void OnRequestRefill(bool success)
        {
            if (success)
            {
                //UI.GetGroupsGold().EffectLifeClaim(pos, Config.maxHealth, PlayerData.playerLife, () => { });
                PlayerData.RaiseToMaxHealth();
                //UI.GetGroupsGold().UpdateUI();
                //UI.screenHome.Hide();
                Hide();
                //EventController.RaiseEvent(EventList.GamePlay.GAME_INIT, new EventParam() { p0 = GameController.GameType.NormalLevel });
            }
            else
            {
                Hide();
                //UI.screenIAPShop.Show();
            }
        }

        void Update()
        {
            timeNextLifeTMP.text = PlayerData.GetTimeToNextLife();
        }
        public override void Show()
        {
            SetupData();
            base.Show();
        }
        void SetupData()
        {
            InClose = false;
            closeNBtn.onClick.RemoveAllListeners();
            closeNBtn.onClick.AddListener(OnCloseButton);
            goldRefillTMP.text = Config.goldToRefill.ToString();
            //refillBtn.image.sprite = PlayerData.gold >= Config.goldToRefill ? spriteCanBuy : spriteOutOfGold;
            StartAnim();
            panelAnim.SetAnimation("Out_of_Lives_Open", false, EndAnim, StartAnim);
            //bool haveOfferVipPass = Game.isHaveJourneyTrip && PlayerData.level >= Config.JourneyTripConfig.levelUnlock && !PlayerData.purchasedJourneyPass;
            // offerVipPass.SetActive(haveOfferVipPass);
            // if (haveOfferVipPass)
            //     TweenControl.GetInstance().ScaleFromZero(offerVipPass, 0.35f);
        }
        public void OnCloseButton()
        {
            if (!InClose)
            {
                InClose = true;
                panelAnim.SetAnimation("Out_of_Lives_Close", false, Hide, StartAnimClose);
                // bool haveOfferVipPass = Game.isHaveJourneyTrip && PlayerData.level >= Config.JourneyTripConfig.levelUnlock && !PlayerData.purchasedJourneyPass;
                // if (haveOfferVipPass)
                //     TweenControl.GetInstance().ScaleFromOne(offerVipPass, 0.35f);
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
        public void StartAnimClose()
        {
            TweenControl.GetInstance().ScaleFromOne(refillBtn.gameObject, 0.3f);
            TweenControl.GetInstance().ScaleFromOne(adsMoreLifeBtn.gameObject, 0.3f);
            TweenControl.GetInstance().ScaleFromOne(closeNBtn.gameObject, 0.3f);
            TweenControl.GetInstance().ScaleFromOne(timeNextLifeTMP.gameObject, 0.3f);
        }
        public void EndAnim()
        {
            TweenControl.GetInstance().ScaleFromZero(refillBtn.gameObject, 0.5f);
            TweenControl.GetInstance().ScaleFromZero(adsMoreLifeBtn.gameObject, 0.5f);
            TweenControl.GetInstance().ScaleFromZero(closeNBtn.gameObject, 0.5f);
            TweenControl.GetInstance().ScaleFromZero(timeNextLifeTMP.gameObject, 0.5f);
            timeNextLifeTMP.text = PlayerData.GetTimeToNextLife();
        }
    }
}
