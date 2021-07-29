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
                EventController.RaiseEvent(EventList.GamePlay.END_LEVEL_ADS);
            });
        }
    }
}