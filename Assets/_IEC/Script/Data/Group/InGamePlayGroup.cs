using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

namespace IEC
{
    public class InGamePlayGroup : _BaseBehaviour, IGroup
    {
        public ScreenUI GameState { get; private set; }

        public Camera mainCamera;
        public Camera uiCamera;
        public bool IsWin => GameState == Locator.Game.ScreenLevelWin || GameState == Locator.Game.ScreenLevelWinHard;
        
        public bool IsInLevelHard { get; private set; } = false;

        public void Awake()
        {

        }

        public void Start()
        {
            GamePlay.Init();
        }

        public void Init()
        {
            
        }

        public void LocateGroup()
        {
            Pool.Set(this);
        }

        public void SetGameState(ScreenUI screen)
        {
            GameState = screen;
        }

        public void SetNewLevel(int level)
        {

        }

        public void SetNewLevelHard()
        {

        }

        public void SetWin()
        {
            EventController.RaiseEvent(EventList.GamePlay.LEVEL_WIN);
        }

        public bool IsActive()
        {
            return gameObject.activeSelf;
        }
    }
}