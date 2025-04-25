using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _IEC.Features.VipProHackerUITutorial;
using UnityEngine;

namespace IEC
{
    public class TutorialController : MonoBehaviour, IController, IEventListener, IEventAwake, IEventStart
    {
        Dictionary<string, EventYieldWait> m_DictYieldWait = new Dictionary<string, EventYieldWait>();
        public GameObject darkBackGround;
        public HighLightCanvas tutorialCanvas;
        public Transform hand;

        public void LocateController()
        {
            Controllers.Add(this);
        }

        public void BehaviourAwake()
        {
            darkBackGround.gameObject.SetActive(false);
            hand.gameObject.SetActive(false);
        }

        public void BehaviourStart()
        {
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
                case EventList.GamePlay.NEW_GAME_HARD_LEVEL:
                    OnNewGame();
                    break;
            }

            if (m_DictYieldWait.TryGetValue(gameEvent, out var yieldWait))
            {
                yieldWait.isKeepWaiting = false;
                m_DictYieldWait.Remove(gameEvent);
            }
        }


        void OnNewGame()
        {
           
        }


        public void SetStatusAllPanelTutorial(bool stt)
        {
            darkBackGround.gameObject.SetActive(stt);
            tutorialCanvas.imgDark.gameObject.SetActive(stt);
            SetStatusHand(false);
        }

        public void ActiveHandAndSetPos(Vector3 pos)
        {
            SetStatusHand(true);
            hand.transform.position = pos + Vector3.up;
        }

        public void SetStatusHand(bool stt)
        {
            hand.gameObject.SetActive(stt);
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
            get { return isKeepWaiting; }
        }

        public EventYieldWait()
        {
            isKeepWaiting = true;
        }
    }
}