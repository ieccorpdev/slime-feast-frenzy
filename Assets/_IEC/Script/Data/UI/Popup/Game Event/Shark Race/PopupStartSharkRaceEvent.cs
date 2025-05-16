using System;
using System.Collections;
using System.Collections.Generic;
using IEC.Helper;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class PopupStartSharkRaceEvent : BasePopup
    {
        public Button btnCloseAll;
        public Button btnStart;

        public TextMeshProUGUI timerText;

        private void Start()
        {
            SetEvent(btnStart, () =>
            {
                Locator.Main.PopupSharkRace.Show();
                OnHideClick();
            });
            SetEvent(btnCloseAll, OnHideClick);
        }

        private void Update()
        {
            timerText.text = DateTimeHelper.GetFormattedTimeHM(DateTime.Now.AddDays(1).Date - DateTime.Now);
        }
    }
}
