using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class ScreenLevelLose : ScreenUI
    {
        public Button btnRetry;
        public Button btnKeepPlaying;
        public Button btnGiveUp;

        private void Start()
        {
            listButtonHideOnClick.Add(btnRetry);

            SetEvent(btnRetry, () =>
            {
                Services.PushEvent(new TrackingEvent.ClickBtnRetry());
                EventController.RaiseEvent(EventList.GamePlay.END_LEVEL_ADS);
            });
            
            SetEvent(btnGiveUp,
                () =>
                {
                    Services.PushEvent(new TrackingEvent.ClickBtnTracking("lose_give_up"));
                    EventController.RaiseEvent(EventList.GamePlay.GO_MAIN);
                    PlayerData.DecreasePlayerLife();
                });
        }
    }
}