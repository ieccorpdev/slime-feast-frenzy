using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class ScreenLevelWin : ScreenUI
    {
        public Button btnNext;

        private void Start()
        {
            listButtonHideOnClick.Add(btnNext);

            SetEvent(btnNext, () =>
            {
                Services.PushEvent(new TrackingEvent.ClickBtnNext());
                EventController.RaiseEvent(EventList.GamePlay.END_LEVEL_ADS);
                Locator.Main.PopupPiggyBank.AddGoldToPiggyBank(10);//CustomGold here
            });
        }
    }
}