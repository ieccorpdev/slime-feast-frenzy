using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace IEC
{
    public class PopupSharkRace : PopupGameEvent
    {
        public TextMeshProUGUI eventTargetText;
        protected override void Awake()
        {
            base.Awake();

            IsEventMultipleStage = true;
        }

        public override void Show()
        {
            base.Show();
            
            UpdateUI();
        }

        public override bool IsEventCompleted()
        {
            //var eventSaveData = _eventRankController.GetEventSaveData(eventType);
            var data = PlayerData.GetEventSaveData(eventType);

            if (data == null || data.listPlayerData.Count == 0)
                return true;
            
            if (data.claimedReward && _eventConfig.listEventData.Count - 1 == data.eventIndex)
                return true;

            return false;
        }

        public override bool IsEndEventStage()
        {
            var eventSaveData = _eventRankController.GetEventSaveData(eventType);
            var data = PlayerData.GetEventSaveData(eventType);

            //If player completed then move to next stage
            if (eventSaveData.listPlayerData.Find(data => data.isPlayer).progress >=
                _eventConfig.GetEventData(data.eventIndex).eventTarget)
            {
                return true;
            }
            
            for (int i = 0; i < 3; i++)
            {
                //If one of top three not finished => race not completed
                if (eventSaveData.listPlayerData[i].progress < _eventConfig.GetEventData(data.eventIndex).eventTarget)
                    return false;
            }
            
            return true;
        }

        public override bool IsEventStageCompleted()
        {
            if (!IsEndEventStage())
                return false;
            
            var eventSaveData = _eventRankController.GetEventSaveData(eventType);

            for (int i = 0; i < 3; i++)
            {
                if (eventSaveData.listPlayerData[i].isPlayer)
                    return true;
            }
            
            return false;
        }

        protected override void UpdateUI()
        {
            var eventSaveData = _eventRankController.GetEventSaveData(eventType);
            var eventData = _eventConfig.GetEventData(eventSaveData.eventIndex);
            eventTargetText.text = eventData.eventTarget.ToString();
            for (int i = 0; i < listPlayerData.Count; i++)
            {
                listPlayerDataUI[i]
                    .Setup(
                        _eventConfig,
                        eventData,
                        eventSaveData.listPlayerData[i],
                        i + 1);
                (listPlayerDataUI[i] as SharkRacePlayerDataUI).UpdateUI((float)eventSaveData.listPlayerData[i].progress / eventData.eventTarget);
            }
        }
        
    }
}
