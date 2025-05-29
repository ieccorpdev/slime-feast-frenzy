using System;
using System.Collections;
using System.Collections.Generic;
using IEC.Helper;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class PopupStartStarChaseEvent : BasePopup
    {
        public Button btnClosePopup;
        public Button btnStart;

        public TextMeshProUGUI timerText;

        private void Start()
        {
            SetEvent(btnStart, () =>
            {
                if (!Locator.Main.PopupStarChase.gameObject.activeInHierarchy)
                {
                    Locator.Main.PopupStarChase.Show();
                }
                // else
                // {
                //     Locator.Main.PopupStarChase.InitEvent();
                // }

                OnHideClick();
            });
            SetEvent(btnCloseAll, ()=>
            {
                if (Locator.Main.PopupStarChase.gameObject.activeInHierarchy)
                {
                    Locator.Main.PopupStarChase.Hide();
                }
                OnHideClick();
            });
            SetEvent(btnClosePopup, ()=>
            {
                if (Locator.Main.PopupStarChase.gameObject.activeInHierarchy)
                {
                    Locator.Main.PopupStarChase.Hide();
                }
                OnHideClick();
            });
        }

        public override void Show()
        {
            if (isActiveAndEnabled)
                return;
            base.Show();
        }

        private void Update()
        {
            timerText.text = DateTimeHelper.GetFormattedTimeHM(DateTime.Now.AddDays(1).Date - DateTime.Now);
        }
    }
}
