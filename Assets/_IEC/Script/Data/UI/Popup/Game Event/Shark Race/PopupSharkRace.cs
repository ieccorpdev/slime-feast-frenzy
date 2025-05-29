using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class PopupSharkRace : PopupGameEvent
    {
        public Button playButton;
        public GameObject playGroups;
        public TextMeshProUGUI eventTargetText;
        protected override void Awake()
        {
            base.Awake();

            IsEventMultipleStage = true;
        }
        public override void Start()
        {
           base.Start();
           SetEvent(playButton,()=>
           {
               Hide();
               Locator.Main.HomeScreenTab.OnClickPlayButton();
           });
        }
        public override void Show()
        {
            base.Show();
            
            UpdateUI();
        }

        public override void InitEvent()
        {
            base.InitEvent();

            UpdateUI();
            
            if (!PlayerData.isShownPopupStartSharkRace)
            {
                PlayerData.isShownPopupStartSharkRace = true;
            }
        }

        public override bool CheckShowStartPopup()
        {
            if (!PlayerData.isShownPopupStartSharkRace)
                return false;
            
            return base.CheckShowStartPopup();
        }

        protected override void ShowStartPopup()
        {
            if (!PlayerData.isShownPopupStartSharkRace)
                return;
            
            if (Locator.Main.PopupStartSharkRaceEvent.gameObject.activeInHierarchy)
                return;
            Locator.Main.PopupStartSharkRaceEvent.Show();
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
            if (_eventRankController == null)
            {
                Init();
            }
            
            CheckUpdateEventProgress();
            
            var eventSaveData = _eventRankController.GetEventSaveData(eventType);
            var data = PlayerData.GetEventSaveData(eventType);

            if (eventSaveData.listPlayerData.Count == 0)
                return false;

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

            var data = eventSaveData.listPlayerData.OrderBy(data => data.indexInUI).ToList();
            
            for (int i = 0; i < listPlayerData.Count; i++)
            {
                listPlayerDataUI[i]
                    .Setup(
                        _eventConfig,
                        eventData,
                        data[i],
                        eventSaveData.listPlayerData.IndexOf(data[i]) + 1);
                (listPlayerDataUI[i] as SharkRacePlayerDataUI).UpdateUI((float)data[i].progress / eventData.eventTarget);
            }
            playGroups.gameObject.SetActive(!claimRewardGroup.gameObject.activeInHierarchy);
        }
        
    }
}
