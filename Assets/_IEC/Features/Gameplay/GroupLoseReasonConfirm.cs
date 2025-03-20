using System;
using System.Collections.Generic;
using _IEC.Features.Booster;
using _IEC.Features.Common;
using _IEC.Features.Common.Extension;
using IEC;
using IEC.TrackingEvent;
using UnityEngine;
using UnityEngine.UI;

namespace _IEC.Features.Gameplay
{
    public class GroupLoseReasonConfirm : BasePopup
    {
        [SerializeField] private GroupCurrencyResult groupCurrencyResult;
        [SerializeField] private Button btnPlayOn;
        [SerializeField] private bool isByBomb;
        [SerializeField] private GroupLevelFailed groupLevelFailed;
        [SerializeField] private RectTransform iapShopScroll;
        [SerializeField] private RectTransform notEnoughGoldWarning;
        private Action m_ActionPlayOn;
        protected override void Awake()
        {
            base.Awake();
            //btnPlayOn.onClick.AddListener(() =>
            //{
            //    if (PlayerData.currencyData.GetCurrencyCount(CurrencyType.Coin) >=
            //        Config.gamePlayConfig.baseNumberCoinForRevive)
            //    {
            //        Services.PushEvent(new ClickButton("losescreen_play_on"));
            //        Services.PushEvent(new CoinSpend( isByBomb ? "losescreen_play_on_bomb" : "losescreen_play_on_no_time"));
            //        PlayerData.currencyData.ConsumeCurrency(CurrencyType.Coin,Config.gamePlayConfig.baseNumberCoinForRevive);
            //        Hide();
            //        m_ActionPlayOn?.Invoke();
            //    }
            //    else
            //    {
            //        notEnoughGoldWarning.DoShowForDuration(2f);
            //        UI.PopUpIAP.iapPosition = IAPPosition.EndGameShop;
            //        UI.ShowScreen(UI.PopUpIAP);
            //    }
            //});
            btnClose.onClick.AddListener(BtnCloseFunction);
            onHideDone.AddListener(() =>
            {
                groupLevelFailed.Show();
                //if (Game.isHaveBooster && PlayerData.level >= Config.boosterConfig.startShowAtLevel)
                //{
                //    groupLevelFailed.ShowBoosterGroup(new List<BoosterId>() {BoosterId.BonusExp,BoosterId.BonusTime});
                //}
                //else
                //{
                //    groupLevelFailed.HideBoosterGroup();
                //}
            });
        }

        public void SetActionPlayOn(Action action)
        {
            m_ActionPlayOn = action;
        }

        public override void Show()
        {
            notEnoughGoldWarning.gameObject.SetActive(false);
            groupCurrencyResult.UpdateUI();
            base.Show();
        }

        private void UpdateUI()
        {
            //todo
        }

        private void BtnCloseFunction()
        {
            //Services.PushEvent(new ClickButton("losescreen_close_2nd"));
            //iapShopScroll.gameObject.SetActive(false);
            PlayerData.DecreasePlayerLife();
            Hide();
        }
    }
}