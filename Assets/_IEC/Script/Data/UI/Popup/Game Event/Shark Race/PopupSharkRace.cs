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

        public override bool IsEndEventStage()
        {
            var eventSaveData = _eventRankController.GetEventSaveData(eventType);
            var data = PlayerData.GetEventSaveData(eventType);
            
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

        private void UpdateUI()
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
