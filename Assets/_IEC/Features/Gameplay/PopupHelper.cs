using System;
using System.Collections.Generic;
using _IEC.Features.Common;
using _IEC.Features.Common.Extension;
using IEC;
using IEC.TrackingEvent;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _IEC.Features.Gameplay
{
    public class PopupHelper : BasePopup
    {
        [SerializeField] protected TextMeshProUGUI txtPrice;
        [SerializeField] protected Button btnBuy;
        [SerializeField] protected HelperId helperId;
        [SerializeField] protected TextMeshProUGUI txtNumberBuyingResult;

        private Dictionary<HelperId, string> _paramByHelperId = new Dictionary<HelperId, string>()
        {
            { HelperId.ScaleUp, "gameplay_bigger_hole" },
            { HelperId.Magnet, "gameplay_magnet" },
            { HelperId.Compass, "gameplay_compass" },
            { HelperId.FrozenTime, "gameplay_time_freeze" },
        };

        protected int CurrentCoinCount => PlayerData.GetPlayerCurrencyCount(ResourceType.Gold);
        protected int Price => Config.gamePlayConfig.GetHelperPrice(helperId);
        protected int NumberBuyingResult => Config.gamePlayConfig.GetNumberBuyingResult(helperId);
        public void Show(Action<Result> callback)
        {
            btnClose.onClick.AddListener(()=>callback?.Invoke(Result.Cancel));
            SetEvent(btnBuy, () =>
            {
                //if (CurrentCoinCount >= Price)
                //{
                //    Services.PushEvent(new CoinSpend(_paramByHelperId[helperId]));
                //    PlayerData.currencyData.ConsumeCurrency(CurrencyType.Coin,Price); 
                //    PlayerData.helperData.CollectHelper(helperId,NumberBuyingResult);
                //    callback?.Invoke(Result.Buy);
                //    Hide();
                //}
                //else
                //{
                //    UI.PopUpIAP.iapPosition = IAPPosition.HelperShop;
                //    UI.ShowScreen(UI.PopUpIAP);
                //}
                //SoundController.PlayClickBtnAudio();
            });
            UpdateUI();
            this.RaiseEvent(EventList.GamePlay.PAUSE_GAME);
            base.Show();
        }
        
        public override void Hide()
        {
            this.RaiseEvent(EventList.GamePlay.RESUME_GAME);
            base.Hide();
        }

        private void UpdateUI()
        {
            txtNumberBuyingResult.text = "x" + NumberBuyingResult;
            txtPrice.text = Price.ToString();
        }
        public enum Result
        {
            Cancel,
            Buy
        }
    }
}