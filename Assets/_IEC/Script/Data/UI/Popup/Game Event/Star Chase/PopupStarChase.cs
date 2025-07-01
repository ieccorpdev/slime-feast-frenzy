using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using DG.Tweening;
using EnhancedUI.EnhancedScroller;
using IEC.Helper;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace IEC
{
    public class PopupStarChase : PopupGameEvent
    {
        [SerializeField] private StarChasePlayerDataUI playerDataUIBarTop;
        [SerializeField] private StarChasePlayerDataUI playerDataUIBarBot;

        // Track scroll direction
        private float previousScrollPosition;
        private int playerDataIndex = -1; // Index of the player data in the list

        protected override void Awake()
        {
            base.Awake();
            previousScrollPosition = 0f;
        }


        protected override void ShowStartPopup()
        {
            if (!PlayerData.isShownPopupStartStarChase)
                return;

            if (Locator.Main.PopupStartStarChaseEvent.gameObject.activeInHierarchy)
                return;
            Locator.Main.PopupStartStarChaseEvent.Show();
        }

        public override bool CheckShowStartPopup()
        {
            if (!PlayerData.isShownPopupStartStarChase)
                return false;
            return base.CheckShowStartPopup();
        }

        public override void InitEvent()
        {
            base.InitEvent();

            if (!PlayerData.isShownPopupStartStarChase)
            {
                PlayerData.isShownPopupStartStarChase = true;
            }
        }

        public override void Start()
        {
            base.Start();

            if (scroller != null)
            {
                // Add scroll change listener for direction detection
                scroller.scrollerScrolled += OnScrollerScrolled;

                // Add visibility change listener to detect when cells appear/disappear
            }

            foreach (var eventPlayerData in listPlayerData)
            {
                if (eventPlayerData.isPlayer)
                {
                    //get index of player data
                    playerDataIndex = listPlayerData.IndexOf(eventPlayerData);
                }
            }
           
            SetupStickBar();
        }

        private void SetupStickBar()
        {
            if(playerDataUIBarBot == null || playerDataUIBarTop == null)
            {
                return;
            }
            var eventSaveData = _eventRankController.GetEventSaveData(EventType.StarChase);
            playerDataUIBarBot.Setup(_eventConfig, _eventConfig.GetEventData(eventSaveData.eventIndex),
                eventSaveData.listPlayerData[playerDataIndex], playerDataIndex + 1);
            playerDataUIBarTop.Setup(_eventConfig, _eventConfig.GetEventData(eventSaveData.eventIndex),
                eventSaveData.listPlayerData[playerDataIndex], playerDataIndex + 1);
            var btnBot = playerDataUIBarBot.GetComponent<Button>();
            if(btnBot == null) 
            {
                btnBot = playerDataUIBarBot.gameObject.AddComponent<Button>();
            }
            btnBot.onClick.RemoveAllListeners();
            btnBot.onClick.AddListener(JumpToPlayer);
            var btnTop = playerDataUIBarTop.GetComponent<Button>();
            if(btnTop == null) 
            {
                btnTop = playerDataUIBarTop.gameObject.AddComponent<Button>();
            }
            btnTop.onClick.RemoveAllListeners();
            btnTop.onClick.AddListener(JumpToPlayer);
        }

        private void JumpToPlayer()
        {
            if (scroller == null || playerDataIndex < 0)
                return;

            // Jump to the player's data index
            scroller.JumpToDataIndex(playerDataIndex, 0.5f, 0, true, EnhancedScroller.TweenType.easeInOutQuart, 0.2f);
        }

        private void OnScrollerScrolled(EnhancedScroller scroller, Vector2 val, float scrollPosition)
        {
            // Determine scroll direction by comparing current position with previous position

            CheckPlayerVisibility();
        }


        private void CheckPlayerVisibility()
        {
            if (!scroller || !playerDataUIBarTop || !playerDataUIBarBot || playerDataIndex < 0)
                return;
            bool isPlayerVisible = scroller.GetCellViewAtDataIndex(playerDataIndex);
            playerDataUIBarTop.gameObject.SetActive(!isPlayerVisible && playerDataIndex < scroller.StartCellViewIndex);
            playerDataUIBarBot.gameObject.SetActive(!isPlayerVisible && playerDataIndex > scroller.EndCellViewIndex);
        }


        // Getter to check if player is currently visible in the scroller

        private void OnDestroy()
        {
            if (scroller != null)
            {
                // Remove listeners when object is destroyed
                scroller.scrollerScrolled -= OnScrollerScrolled;
            }
        }

        public override EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
        {
            var cellView = base.GetCellView(scroller, dataIndex, cellIndex);

            // Check player visibility after getting a cell view
            CheckPlayerVisibility();

            return cellView;
        }

        public override void ReloadData()
        {
            base.ReloadData();

            // Check player visibility after reloading data
            CheckPlayerVisibility();
        }
    }
}