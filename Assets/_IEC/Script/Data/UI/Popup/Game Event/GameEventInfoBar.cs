using System;
using System.Collections;
using System.Collections.Generic;
using EnhancedUI.EnhancedScroller;
using IEC.Helper;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

namespace IEC
{
    public class GameEventInfoBar : _BaseBehaviour, IEnhancedScrollerDelegate
    {
        public EventType eventType;
        public GameObject startGroup;
        public Button startButton;
        public EnhancedScroller scroller;

        public GameObject eventTargetGroup;
        public TextMeshProUGUI eventTargetText;
        public TextMeshProUGUI timerText;
        
        public GameEventPlayerDataUI playerDataUIPrefab;
        public List<EventPlayerData> listPlayerData = new ();
        
        protected EventRankController _eventRankController;
        protected GameEventConfig _eventConfig;

        protected EventSaveData eventSaveData;
        private PopupGameEvent popup;
        
        
        // Start is called before the first frame update
        void Start()
        {
            Init();
            startButton.onClick.AddListener(OnStartClick);
        }

        private void OnStartClick()
        {
            // if(eventType == EventType.StarChase)
            //     Locator.Main.PopupStarChase.StartNewEventStage(0);
            // else if(eventType == EventType.SharkRace)
            //     Locator.Main.PopupSharkRace.StartNewEventStage(0);
            
            popup.StartNewEventStage(popup.GetNextEventStageIndex());
            SetupData();
            EventController.RaiseEvent(EventList.GamePlay.UPDATE_GAME_EVENT_UI);
            // if (Locator.Main.PopupBooster.gameObject.activeInHierarchy)
            // {
            //     Locator.Main.PopupBooster.playButton.onClick.Invoke();
            // }
            // else if (Locator.Game.ScreenLevelWin.gameObject.activeInHierarchy)
            // {
            //     
            // }
            
        }

        private void Init()
        {
            _eventRankController = Controllers.Get<EventRankController>();
            _eventConfig = Config.AllGameEventConfig.GetGameEventConfig(eventType);
            
            listPlayerData.Clear();
            var data = _eventRankController.GetEventSaveData(eventType);
            
            var eventData = _eventConfig.GetEventData(data.eventIndex);
            
            listPlayerData = new List<EventPlayerData>(data.listPlayerData);

            if (eventTargetText != null)
            {
                eventTargetText.text = eventData.eventTarget.ToString();
            }
            
            InitScroller();
        }

        // Update is called once per frame
        void Update()
        {
            UpdateTimer();
        }

        public virtual void SetupData()
        {
            Init();
            eventSaveData = _eventRankController.GetEventSaveData(eventType);
            popup = eventType switch
            {
                EventType.StarChase => Locator.Main.PopupStarChase,
                EventType.SharkRace => Locator.Main.PopupSharkRace,
                EventType.ChompOff => Locator.Main.PopupChompOff,
                _=> null,
            };
            
            if(scroller != null)
                scroller.gameObject.SetActive(eventSaveData.listPlayerData.Count > 0 && !eventSaveData.claimedReward);
            
            startGroup.SetActive(eventSaveData.listPlayerData.Count == 0 || eventSaveData.claimedReward);

            if (eventTargetGroup != null)
            {
                eventTargetGroup.SetActive(eventSaveData.listPlayerData.Count > 0 && !eventSaveData.claimedReward);
            }
            
            InitScroller();
            ReloadData();
        }
        
        protected virtual void UpdateTimer()
        {

            if (popup.IsEndEvent() || popup.IsEndEventStage())
            {
                if (eventSaveData.claimedReward)
                {
                    timerText.text = "Finished";
                }
                else
                {
                    timerText.text =
                        DateTimeHelper.GetFormattedTimeHMS(
                            DateTime.Now.AddDays(1).Date - DateTime.Now);
                }
            }
            else
            {
                timerText.text =
                    DateTimeHelper.GetFormattedTimeHMS(
                        eventSaveData.timeEnd - DateTime.Now);
            }
        }

        
        private void InitScroller()
        {
            if (scroller == null || !scroller.gameObject.activeInHierarchy)
                return;
            
            if (scroller.Delegate == null)
            {
                scroller.Delegate = this;
            }
            else
            {
                LoadData();
            }
        }
        
        public void LoadData()
        {
            if (scroller == null || !scroller.gameObject.activeInHierarchy)
                return;
            scroller.RefreshActiveCellViews();
        }

        public void ReloadData()
        {
            if (scroller == null || !scroller.gameObject.activeInHierarchy)
                return;
            
            scroller.ReloadData();
            JumpToPlayer();
        }
        
        private void JumpToPlayer()
        {
            InitScroller();
            int index = listPlayerData.FindIndex(data => data.isPlayer);
            scroller.JumpToDataIndex(index, 0.5f, 0, true, EnhancedScroller.TweenType.easeInOutQuart, 0.2f);
        }

        public int GetNumberOfCells(EnhancedScroller scroller)
        {
            return listPlayerData.Count;
        }

        public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
        {
            // if (listPlayerData[dataIndex].isPlayer)
            // {
            //     return 192;
            // }
            return 162;
        }

        public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
        {
            var eventSaveData = _eventRankController.GetEventSaveData(eventType);

            GameEventPlayerDataUI cellView = scroller.GetCellView(playerDataUIPrefab) as GameEventPlayerDataUI;
                
            cellView.Setup(_eventConfig, _eventConfig.GetEventData(eventSaveData.eventIndex),  eventSaveData.listPlayerData[dataIndex], dataIndex + 1);
                

            return cellView;
        }
    }
}
