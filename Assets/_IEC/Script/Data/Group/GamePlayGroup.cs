
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace IEC
{
    public class GamePlayGroup : _BaseBehaviour, IGroup
    {
        public ScreenUI GameState { get; private set; }
        public bool IsWin => false;

        public Camera mainCamera;
        public Camera uiCamera;


        public void LocateGroup()
        {
            Pool.Set(this);
        }

        public void SetGameState(ScreenUI screen)
        {
            GameState = screen;
        }

        void Start()
        {

        }

        public void SetNewLevel(int level)
        {
        }

        public void SetFinishPlay()
        {
            EventController.RaiseEvent(EventList.GamePlay.LEVEL_WIN);
        }
    }
}