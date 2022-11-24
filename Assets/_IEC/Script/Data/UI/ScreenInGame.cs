using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace IEC
{
    public class ScreenInGame : ScreenUI
    {
        public override bool IS_PAUSE_GAME => false;

        public TextMeshProUGUI txtLevel;
        public Button btnRestart;

        [Header("Debug")]
        public Button btnLevelWin;
        public Button btnLevelLose;

        private void Start()
        {
            listButtonHideOnClick.Add(btnRestart);

            SetEvent(btnRestart, () =>
            {
                EventController.RaiseEvent(EventList.GamePlay.END_LEVEL_ADS);
                Services.PushEvent(new TrackingEvent.ClickBtnReplay());
            });
            
            SetEvent(btnLevelWin, () =>
            {
                EventController.RaiseEvent(EventList.GamePlay.LEVEL_WIN);
            });
            
            SetEvent(btnLevelLose, () =>
            {
                EventController.RaiseEvent(EventList.GamePlay.LEVEL_LOSE);
            });
        }

        public void SetLevel(int level)
        {
            txtLevel.text = "LEVEL " + level.ToString();
        }
    }
}