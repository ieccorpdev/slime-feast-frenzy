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

        
        public void LocateGroup()
        {
            Pool.Set(this);
        }
    }
}