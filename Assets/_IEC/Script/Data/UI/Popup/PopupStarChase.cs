using System;
using System.Collections;
using System.Collections.Generic;
using EnhancedUI.EnhancedScroller;
using UnityEngine;

namespace IEC
{
    public class PopupStarChase : BasePopup
    {
        [SerializeField] EnhancedScroller scroller;
        
        
        private EventRankController _eventRankController;
        private GameEventConfig _eventConfig;

        protected override void Awake()
        {
            base.Awake();
            _eventRankController = Controllers.Get<EventRankController>();
            _eventConfig = Config.AllGameEventConfig.GetGameEventConfig(EventType.StarChase);
        }

        public override void Show()
        {
            base.Show();
            CheckInitEventProgress();
            UpdateUI();
        }

        private void CheckInitEventProgress()
        {
            if (DateTime.Now >= PlayerData.GetEventSaveData(EventType.StarChase).timeEnd)
            {
                _eventRankController
                    .InitData(
                        EventType.StarChase,
                        DateTime.Now,
                        DateTime.Now.AddHours(_eventConfig.eventDurationInHours),
                        new List<EventPlayerData>() {}
                        );
            }
        }

        private void UpdateUI()
        {
            
        }
    }
}
