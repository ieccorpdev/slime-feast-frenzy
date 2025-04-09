using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IEC.Wind.Leaderboard
{
    public class PopupDoubleMedal : BasePopup
    {
        public ButtonTicket getBtn;
        public Image medalImg;
        public Image eventImg;
        [Header("Ticket")]
        public GameObject adsObj;
        public GameObject ticketObj;
        public override void Show()
        {
            base.Show();

            medalImg.sprite = LeaderboardConfig.GetSpirteCurrentLeague();
            getBtn.UpdateUI();
            //eventImg.gameObject.SetActive(Game.isHaveEvent);
            //if (!Game.isHaveEvent) return;
            //eventImg.sprite = MonthEvent.EventConfig.GetDisplayData(MonthEventData.CurrentMonth).currencySpr;
        }

        private void Start()
        {
            base.SetEvent(getBtn.btn, () =>
            {
                Services.ShowRewardAds(() =>
                {
                    Services.PushEvent(new TrackingEvent.SRApostition("X2Medal"));                    
                    ActiveDoubleMedal();
                });
                //GameController.UseTicket(() =>
                //{
                //    ActiveDoubleMedal();
                //}, () =>
                //{
                //    Services.ShowRewardAds(() =>
                //    {
                //        Services.PushEvent(new TrackingEvent.CountRewardAds()
                //        {
                //            pos = "X2Medal",
                //        });
                //        ActiveDoubleMedal();
                //    });
                //}, "X2Medal");


                //if (Game.isHaveTicket && PlayerData.ticket > 0)
                //{
                //    PlayerData.AddTicket(-1);
                //    Services.PushEvent(new TrackingEvent.UseTicket("X2Medal"));
                //    ActiveDoubleMedal();
                //    if (Game.isHaveEvent) MonthEvent.ActiveDoubleCurrency();
                //    return;
                //}
                //Services.ShowRewardAds(() =>
                //{
                //    Services.PushEvent(new TrackingEvent.SRAPosition("X2Medal"));
                //    ActiveDoubleMedal();
                //    if (Game.isHaveEvent) MonthEvent.ActiveDoubleCurrency();
                //});
            });
        }

        private void ActiveDoubleMedal()
        {
            LeaderboardData.ActiveDoubleMedal();
            Locator.Main.PopupLeaderboard.UpdateUI();
            //UI.ScreenHome.UpdateLeaderboard();
            Hide();
        }

        //public void UpdateTicket()
        //{
        //    ticketObj.SetActive(PlayerData.ticket > 0 && Game.isHaveTicket);
        //    adsObj.SetActive(PlayerData.ticket <= 0 || !Game.isHaveTicket);
        //}
    }
}