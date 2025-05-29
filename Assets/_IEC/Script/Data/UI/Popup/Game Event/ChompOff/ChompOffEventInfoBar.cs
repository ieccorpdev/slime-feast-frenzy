using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public class ChompOffEventInfoBar : GameEventInfoBar
    {
        public GameEventPlayerDataUI playerUI;
        public GameEventPlayerDataUI opponentUI;
        public ChompOffProgressBarUI progressBarUI;

        private PopupChompOff _popupChompOff;

        private void Awake()
        {
            _popupChompOff = Locator.Main.PopupChompOff;
        }

        public override void SetupData()
        {
            base.SetupData();

            _popupChompOff = Locator.Main.PopupChompOff;
            
            progressBarUI.gameObject.SetActive(eventSaveData.listPlayerData.Count > 0 && !eventSaveData.claimedReward);
            
            var eventData = _eventConfig.GetEventData(eventSaveData.eventIndex);

            if (eventSaveData != null && eventSaveData.listPlayerData.Count > 0)
            {
                var player = eventSaveData.listPlayerData.Find(data => data.isPlayer);
                var opponent = eventSaveData.listPlayerData.Find(data => !data.isPlayer);

                playerUI.Setup(_eventConfig, eventData, player, 0);
                opponentUI.Setup(_eventConfig, eventData, opponent, 0);

                progressBarUI.UpdateProgressBarUI(player, opponent);
            }
        }

        protected override void UpdateTimer()
        {
            if (!gameObject.activeInHierarchy)
                return;
            
            timerText.text = _popupChompOff.GetTimerText();
        }
    }
}
