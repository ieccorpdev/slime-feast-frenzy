using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace IEC
{
    public class HintController : _BaseBehaviour, IController, IEventListener
    {
        public void LocateController()
        {
            Controllers.Add(this);
        }

        public bool IsActive()
        {
            return gameObject.activeSelf;
        }


        public void BehaviourAwake()
        {
            
        }

        public void OnEventRaise(string gameEvent, EventParam param)
        {
            switch (gameEvent)
            {
                case EventList.GamePlay.GAME_START_HINT:
                    StartHint();
                    break;
                case EventList.GamePlay.GAME_AFTER_HINT:
                    Game.isAllowHint = true;
                    EventController.RaiseEvent(EventList.GamePlay.CHECK_GAME_OVER);
                    break;
            }
        }

        public void StartHint()
        {
            if (!Game.isPlaying) return;
            if (!Game.isAllowHint) return;
            Game.isAllowHint = false;
            Game.maxHintIndex = Mathf.Clamp(Game.maxHintIndex + Game.size - 2, 0, Game.listHints.Count);
            for (int i = 0; i < Game.maxHintIndex; i++)
            {
                var grid = Game.listHints[i];
                if (!grid.IsTrueRot)
                {
                    grid.OnHint();
                }
            }

            EventController.RaiseEventDelay(.3f, EventList.GamePlay.GAME_AFTER_HINT);
        }

    }
}