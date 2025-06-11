using System;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public class RelicEventController : _BaseBehaviour, IController, IEventListener, IEventAwake, IEventStart
    {
        private DateTime currentTimeUpdate;

        public void LocateController()
        {
            Controllers.Add(this);
        }

        public bool IsActive()
        {
            return gameObject.activeSelf;
        }

        public void BehaviourStart()
        {
            InitData();
        }

        public void BehaviourAwake()
        {
            
        }

        public void OnEventRaise(string gameEvent, EventParam param)
        {
            switch (gameEvent)
            {
                case EventList.GamePlay.LEVEL_WIN:
                    PlayerData.relicData.IsDoneLevel(Config.relicConfig);
                    break;
            }
        }

        public void InitData()
        {
            DateTime timeEnd = Config.eventLiveOpsConfig.GetTimeEndEvent(EventType.RelicHunt,
                Mathf.FloorToInt(Config.relicConfig.eventDurationInHours / 24), PlayerData.GetDaysSinceDownload());
            currentTimeUpdate = DateTime.Now;
            bool isNewEvent = (timeEnd.Day - PlayerData.relicData.eventTimeEnd.Day) != 0;
            if (isNewEvent)
            {
                PlayerData.relicData.eventTimeEnd = timeEnd;
                PlayerData.relicData.ResetEvent(Config.relicConfig);
            }
        }

        private void Update()
        {
            if ((currentTimeUpdate.Date - DateTime.Now.Date).TotalDays >= 1)
            {
                InitData();
            }
        }
    }
}