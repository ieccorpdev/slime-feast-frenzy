using System;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public class RelicEventController : _BaseBehaviour, IController, IEventListener, IEventAwake, IEventStart
    {

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
            PlayerData.relicData.eventTimeEnd = Config.eventLiveOpsConfig.GetTimeEndEvent(EventType.RelicHunt,
                Mathf.FloorToInt(Config.relicConfig.eventDurationInHours / 24), PlayerData.GetDaysSinceDownload());
            //PlayerData.relicData.eventTimeEnd  = DateTime.Now;
        }



    }
}