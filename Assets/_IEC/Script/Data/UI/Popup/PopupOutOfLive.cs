using IEC;
using IEC.TrackingEvent;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _IEC
{
    public class PopupOutOfLive : BasePopup
    {
        public Button refillBtn, adsMoreLifeBtn, closeNBtn, refillBtnOld;
        public TextMeshProUGUI goldRefillTMP, timeNextLifeTMP, goldRefillTMPOld;
        public TextMeshProUGUI playerLifeText;
        public TextMeshProUGUI playerLifeAddWithGoldText;
        public RectTransform countdownGroup, fullGroup, groupBtnRefill;
        private bool InClose;

        private bool IsRaiseToMaxLife = false;

        protected override void Awake()
        {
            base.Awake();
            refillBtn.onClick.AddListener(OnClickButtonRefill);
            refillBtnOld.onClick.AddListener(OnClickButtonRefill);
            adsMoreLifeBtn.onClick.AddListener(OnClickButtonAdsMoreLife);
            groupBtnRefill.gameObject.SetActive(Game.isUsingBtnAdsMoreLife);
            refillBtnOld.gameObject.SetActive(!Game.isUsingBtnAdsMoreLife);
        }

        void OnClickButtonRefill()
        {
            OnRequestRefill(PlayerData.SubtractCurrency(ResourceType.Gold, IsRaiseToMaxLife ? Config.goldToRefill : Config.goldToAddOneLife));
        }
        void OnClickButtonAdsMoreLife()
        {
            Services.ShowRewardAds(() =>
            {
                PlayerData.AddPlayerLife();
                Services.PushEvent(new IEC.TrackingEvent.SRApostition($"Ads_More_life"));
                //UI.screenHome.Hide();
                // UI.GetGroupsGold().EffectLifeClaim(pos, 1, PlayerData.playerLife, () =>
                // {
                //     UI.GetGroupsGold().UpdateUI();
                // });
                Hide();
                //EventController.RaiseEvent(EventList.GamePlay.GAME_INIT, new EventParam() { p0 = GameController.GameType.NormalLevel });
            }, null
            ,default
            , 60);
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
            int maxHealth = PlayerData.journeyData.wasOpenJourney ? Config.maxHealthInKingdomPass : Config.maxHealth;
            countdownGroup.gameObject.SetActive(PlayerData.playerLife < maxHealth);
            fullGroup.gameObject.SetActive(PlayerData.playerLife == maxHealth);
            playerLifeText.text = PlayerData.playerLife.ToString();
        }

        void Update()
        {
            timeNextLifeTMP.text = PlayerData.GetTimeToNextLife();
            UpdateUI();
        }
        public override void Show()
        {
            SetupData();
            base.Show();
            UpdateUI();
        }
        void SetupData()
        {
            isShowTopResourceBar = true;
            IsRaiseToMaxLife = true;
            InClose = false;
            closeNBtn.onClick.RemoveAllListeners();
            closeNBtn.onClick.AddListener(OnCloseButton);
            goldRefillTMP.text = IsRaiseToMaxLife ? Config.goldToRefill.ToString() : Config.goldToAddOneLife.ToString();
            goldRefillTMPOld.text = goldRefillTMP.text;
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
        public void StartAnimClose()
        {
            TweenControl.GetInstance().ScaleFromOne(refillBtn.gameObject, 0.3f);
            //TweenControl.GetInstance().ScaleFromOne(adsMoreLifeBtn.gameObject, 0.3f);
            TweenControl.GetInstance().ScaleFromOne(closeNBtn.gameObject, 0.3f);
            TweenControl.GetInstance().ScaleFromOne(timeNextLifeTMP.gameObject, 0.3f);
        }
        public void EndAnim()
        {
            TweenControl.GetInstance().ScaleFromZero(refillBtn.gameObject, 0.5f);
            //TweenControl.GetInstance().ScaleFromZero(adsMoreLifeBtn.gameObject, 0.5f);
            TweenControl.GetInstance().ScaleFromZero(closeNBtn.gameObject, 0.5f);
            TweenControl.GetInstance().ScaleFromZero(timeNextLifeTMP.gameObject, 0.5f);
            timeNextLifeTMP.text = PlayerData.GetTimeToNextLife();
        }
    }
}
