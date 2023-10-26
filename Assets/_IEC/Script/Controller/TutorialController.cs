using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public class TutorialController : _BaseBehaviour, IController, IEventListener, IEventAwake, IEventStart
    {
        Dictionary<string, EventYieldWait> m_DictYieldWait = new Dictionary<string, EventYieldWait>();

        public void LocateController()
        {
            Controllers.Add(this);
        }

        public bool IsActive()
        {
            return gameObject.activeSelf;
        }

        public void OnEventRaise(string gameEvent, EventParam param)
        {
            switch (gameEvent)
            {
                case EventList.GamePlay.NEW_GAME:
                    OnNewGame();
                    break;
            }

            if (m_DictYieldWait.TryGetValue(gameEvent, out var yieldWait))
            {
                yieldWait.isKeepWaiting = false;
                m_DictYieldWait.Remove(gameEvent);
            }
        }

        public void BehaviourAwake()
        {
        }

        public void BehaviourStart()
        {
        }

        void OnNewGame()
        {
            //HideTutorial();

            // Condition show tutorial
            //if (!Game.isHaveTutorial) return;
            if (PlayerData.level > 1) return;

            StartCoroutine(IEnumTemplateTutorial());
        }

        IEnumerator IEnumTemplateTutorial()
        {
            // show tut step 1

            // wait for event 1
            yield return WaitForEvent("EventList.GamePlay.GAME_EVENT_1");

            // hide tut step 1
            //Services.PushEvent(new TrackingEvent.CompleteStepTutorial() { stepID = 1 });

            // show tut step 2

            // wait for event 2
            yield return WaitForEvent("EventList.GamePlay.GAME_EVENT_2");

            // hide tut step 2
            //Services.PushEvent(new TrackingEvent.CompleteStepTutorial() { stepID = 2 });

            // finish tutorial
        }

        EventYieldWait WaitForEvent(string gameEvent)
        {
            var waitEvent = new EventYieldWait();
            m_DictYieldWait[gameEvent] = waitEvent;
            return waitEvent;
        }
    }

    public class EventYieldWait : CustomYieldInstruction
    {
        public bool isKeepWaiting;

        public override bool keepWaiting
        {
            get
            {
                return isKeepWaiting;
            }
        }

        public EventYieldWait()
        {
            isKeepWaiting = true;
        }
    }
}