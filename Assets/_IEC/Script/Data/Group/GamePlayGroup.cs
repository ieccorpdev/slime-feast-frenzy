using System.Collections;
using System.Collections.Generic;
using _IEC.Features.Gameplay;
using UnityEngine;

namespace IEC
{
    public class GamePlayGroup : _BaseBehaviour, IGroup
    {
        public ScreenUI GameState { get; private set; }
        public bool IsWin => GameState == Locator.Game.ScreenLevelWin;

        public Camera mainCamera;
        public Camera uiCamera;
        public Camera homeCamera;

        public GameObject ingameDebugConsole;

        public LevelController levelController;

        
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


        public void OnShowScreenHome()
        {
            homeCamera.gameObject.SetActive(true);
            SetNewLevel(PlayerData.level);
        }
        
    }
}