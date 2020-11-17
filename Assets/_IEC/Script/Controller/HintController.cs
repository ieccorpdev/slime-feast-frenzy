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
                case EventList.GamePlay.GAME_HINT:
                    Hint();
                    break;
            }
        }

        [NaughtyAttributes.Button]
        public void Hint()
        {
            var maxIndex = Game.listHints.Count - 1;
            var size = Game.size - 2;

            var step = 0;
            var index = 0;

            while (step < size && index < maxIndex)
            {
                var grid = Game.listHints[index];
                if (!grid.IsTrueRot)
                {
                    grid.OnHint();
                    step += 1;
                }
                index += 1;
            }
            EventController.RaiseEventDelay(.3f, EventList.GamePlay.CHECK_GAME_OVER);
        }

    }
}