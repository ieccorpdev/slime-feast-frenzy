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
        public Button btnClosePopup;
        public Button btnStart;

        public TextMeshProUGUI timerText;

        public TextMeshProUGUI infoText;

        private void Start()
        {
            SetEvent(btnStart, () =>
            {
                if (!Locator.Main.PopupSharkRace.gameObject.activeInHierarchy)
                {
                    Locator.Main.PopupSharkRace.Show();
                }
                // else
                // {
                //     Locator.Main.PopupSharkRace.InitEvent();
                // }
                OnHideClick();
            });
            SetEvent(btnCloseAll, ()=>
            {
                if (Locator.Main.PopupSharkRace.gameObject.activeInHierarchy)
                {
                    Locator.Main.PopupSharkRace.Hide();
                }
                OnHideClick();
            });
            
            SetEvent(btnClosePopup, ()=>
            {
                if (Locator.Main.PopupSharkRace.gameObject.activeInHierarchy)
                {
                    Locator.Main.PopupSharkRace.Hide();
                }
                OnHideClick();
            });
        }

        public override void Show()
        {
            if (isActiveAndEnabled)
                return;
            
            base.Show();

            UpdateInfoText();
        }

        private void UpdateInfoText()
        {
            var eventSaveData = PlayerData.GetEventSaveData(EventType.SharkRace);
            int index = Locator.Main.PopupSharkRace.IsEventStageCompleted() ? eventSaveData.eventIndex + 1 : eventSaveData.eventIndex;
            if (Locator.Main.PopupSharkRace.IsEndEvent())
            {
                index = 0;
            }
            var eventData = Config.AllGameEventConfig.GetGameEventConfig(EventType.SharkRace).GetEventData(index);
            infoText.text = $"clear {eventData.eventTarget} levels!";
        }

        private void Update()
        {
            timerText.text = DateTimeHelper.GetFormattedTimeHM(DateTime.Now.AddDays(1).Date - DateTime.Now);
        }
    }
}
