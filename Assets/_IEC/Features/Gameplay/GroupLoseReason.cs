using System;
using _IEC.Features.Common;
using _IEC.Features.Common.Extension;
using IEC;
using IEC.TrackingEvent;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _IEC.Features.Gameplay
{
    public class GroupLoseReason : BasePopup
    {
        [SerializeField] protected Button btnPlayOn;
        [SerializeField] protected TextMeshProUGUI txtNumberCoinPlayOn;
        [SerializeField] private bool isByBomb;
        [SerializeField] private GroupLoseReasonConfirm groupLoseReasonConfirm;
        [SerializeField] private RectTransform notEnoughGoldWarning;
        private Action m_PlayOnAction;
        protected override void Awake()
        {
            base.Awake();
            btnClose.onClick.AddListener(Hide);
            //btnClose.onClick.AddListener(() =>
            //{
            //    Services.PushEvent(new ClickButton("losescreen_close_1st"));
            //});
            //SetEvent(btnPlayOn, () =>
            //{
            //    if (PlayerData.currencyData.GetCurrencyCount(CurrencyType.Coin) >=
            //        Config.gamePlayConfig.baseNumberCoinForRevive)
            //    {
            //        Services.PushEvent(new ClickButton("losescreen_play_on"));
            //        Services.PushEvent(new CoinSpend( isByBomb ? "losescreen_play_on_bomb" : "losescreen_play_on_no_time"));
            //        PlayerData.currencyData.ConsumeCurrency(CurrencyType.Coin,
            //            Config.gamePlayConfig.baseNumberCoinForRevive);
            //        m_PlayOnAction?.Invoke();
            //    }
            //    else
            //    {
            //        //todo
            //        notEnoughGoldWarning.DoShowForDuration(2f);
            //        UI.PopUpIAP.iapPosition = IAPPosition.EndGameShop;
            //        UI.ShowScreen(UI.PopUpIAP);
            //    }
            //});
            onHideDone.AddListener(groupLoseReasonConfirm.Show);
            
        }

        public void SetActionPlayOn(Action action)
        {
            m_PlayOnAction = action;
            groupLoseReasonConfirm.SetActionPlayOn(action);
        }

        public override void Show()
        {
            notEnoughGoldWarning.gameObject.SetActive(false);
            base.Show();
        }
    }
}