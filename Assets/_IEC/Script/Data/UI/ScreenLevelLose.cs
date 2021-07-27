using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class ScreenLevelLose : ScreenUI
    {
        public Button btnRetry;

        private void Start()
        {
            SetEvent(btnRetry, () =>
            {
                EventController.RaiseEvent(EventList.GamePlay.END_LEVEL_ADS);
            });
        }
    }
}