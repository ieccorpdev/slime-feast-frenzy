using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class PopupChompOff : PopupGameEvent
    {
        public Button playButton;
        public GameEventPlayerDataUI playerUI;
        public GameEventPlayerDataUI opponentUI;
        protected override void Awake()
        {
            base.Awake();
            
            SetEvent(playButton, OnPlayButtonClick);

            IsEventMultipleStage = true;
        }

        private void OnPlayButtonClick()
        {
            Hide();
            Locator.Main.PopupBooster.Show();
        }

        public override void Show()
        {
            base.Show();
            
            UpdateUI();
        }
        
        protected override void UpdateUI()
        {
            var eventSaveData = _eventRankController.GetEventSaveData(eventType);
            var eventData = _eventConfig.GetEventData(eventSaveData.eventIndex);
            
            var player = eventSaveData.listPlayerData.Find(data => data.isPlayer);
            var opponent = eventSaveData.listPlayerData.Find(data => !data.isPlayer);
            
            playerUI.Setup(_eventConfig, eventData, player, 0);
            opponentUI.Setup(_eventConfig, eventData, opponent, 0);
        }
        
        public override bool IsEndEventStage()
        {
            if (_eventRankController == null)
            {
                Init();
            }
            
            var eventSaveData = _eventRankController.GetEventSaveData(eventType);
            var data = _eventConfig.GetEventData(eventSaveData.eventIndex);

            return (DateTime.Now - eventSaveData.timeStart).TotalSeconds >= data.eventStageTimeInSeconds;
        }

        public override bool IsEventStageCompleted()
        {
            if (!IsEndEventStage())
                return false;
            var eventSaveData = _eventRankController.GetEventSaveData(eventType);

            //Event only have player and opponent
            var player = eventSaveData.listPlayerData.Find(data => data.isPlayer);
            var opponent = eventSaveData.listPlayerData.Find(data => !data.isPlayer);

            return player.progress > opponent.progress;

        }
    }
}
