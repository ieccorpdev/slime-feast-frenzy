using System;
using System.Collections.Generic;
using _IEC.Features.Common;
using IEC;
using IEC.TrackingEvent;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _IEC.Features.Booster
{
    public class PopupBuyBooster : BasePopup
    {
        [SerializeField] private BoosterId boosterId;
        [SerializeField] private Button btnBuy;
        [SerializeField] private TextMeshProUGUI txtPrice;
        [SerializeField] private TextMeshProUGUI txtNumber;
        [SerializeField] private GameObject groupIapPacks;
        public BoosterId BoosterId => boosterId;
        private Action<bool> m_OnClose;

        private Dictionary<BoosterId, string> paramCoinSpendById = new Dictionary<BoosterId, string>()
        {
            { Booster.BoosterId.BonusExp, "boosterscreen_growth_potion" },
            { Booster.BoosterId.BonusTime, "boosterscreen_time_extend" },
        };
        
        private Dictionary<BoosterId, string> paramCoinSpendByIdRetry = new Dictionary<BoosterId, string>()
        {
            { Booster.BoosterId.BonusExp, "retryscreen_growth_potion" },
            { Booster.BoosterId.BonusTime, "retryscreen_time_extend" },
        };
        //private BoosterConfigModel ConfigModel => Config.boosterConfig.GetBoosterConfigModelById(boosterId);
        protected override void Awake()
        {
            base.Awake();
            SetupButtons();
        }

        private void SetupButtons()
        {
           //SetEvent(btnBuy, () =>
           //{
           //    if (PlayerData.currencyData.IsEnough(ConfigModel.priceModel.currencyType, ConfigModel.priceModel.number))
           //    {
           //        PlayerData.currencyData.ConsumeCurrency(ConfigModel.priceModel.currencyType, ConfigModel.priceModel.number);
           //        PlayerData.boosterData.BuyBooster(boosterId, ConfigModel.priceModel.result);
           //        Hide();
           //        m_OnClose?.Invoke(true);

           //        bool isInLoseScreen = UI.ScreenLevelLose.gameObject.activeInHierarchy;
           //        if (isInLoseScreen)
           //        {
           //            Services.PushEvent(new CoinSpend(paramCoinSpendByIdRetry[boosterId]));
           //        }
           //        else
           //        {
           //            Services.PushEvent(new CoinSpend(paramCoinSpendById[boosterId]));
           //        }
           //    }
           //    else
           //    {
           //        UI.PopUpIAP.iapPosition = IAPPosition.HelperShop;
           //        UI.ShowScreen(UI.PopUpIAP);
           //    }
           //});
           //SetEvent(btnClose, () =>
           //{
           //    Hide();
           //    m_OnClose?.Invoke(false);
           //});
        }

        public void Show(Action<bool> onCloseCallback)
        {
            m_OnClose = onCloseCallback;
            UpdateData();
            base.Show();
        }

        private void UpdateData()
        {
            //txtPrice.text = ConfigModel.priceModel.number.ToString();
            //txtNumber.text = ConfigModel.priceModel.result.ToString();
        }
    }
}
