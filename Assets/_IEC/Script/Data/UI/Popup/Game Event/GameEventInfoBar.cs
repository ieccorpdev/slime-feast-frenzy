using System;
using System.Collections;
using System.Collections.Generic;
using EnhancedUI.EnhancedScroller;
using IEC.Helper;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

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
        
        
        // Start is called before the first frame update
        void Start()
        {
            Init();
            startButton.onClick.AddListener(OnStartClick);
        }

        private void OnStartClick()
        {
            if(eventType == EventType.StarChase)
                Locator.Main.PopupStarChase.StartNewEventStage(0);
            else if(eventType == EventType.SharkRace)
                Locator.Main.PopupSharkRace.StartNewEventStage(0);

            if (Locator.Main.PopupBooster.gameObject.activeInHierarchy)
            {
                Locator.Main.PopupBooster.playButton.onClick.Invoke();
            }
            else if (Locator.Game.ScreenLevelWin.gameObject.activeInHierarchy)
            {
                
            }
            
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

        public void SetupData()
        {
            Init();
            var eventSaveData = _eventRankController.GetEventSaveData(eventType);
            
            scroller.gameObject.SetActive(eventSaveData.listPlayerData.Count > 0);
            startGroup.SetActive(eventSaveData.listPlayerData.Count == 0);

            if (eventTargetGroup != null)
            {
                eventTargetGroup.SetActive(eventSaveData.listPlayerData.Count > 0);
            }
            
            InitScroller();
            ReloadData();
        }
        
        private void UpdateTimer()
        {

            var eventSaveData = PlayerData.GetEventSaveData(eventType);

            if (eventSaveData.timeEnd <= DateTime.Now)
            {
                timerText.text = "Finished";
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
            scroller.RefreshActiveCellViews();
        }

        public void ReloadData()
        {
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
