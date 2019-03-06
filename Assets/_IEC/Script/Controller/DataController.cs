using System;
using UnityEngine;

namespace IEC
{
    public class DataController : MonoBehaviour, IController, IEventListener, IEventAwake, IEventStart
    {
        [SerializeField]
        _EmptyData m_EmptyData;

        public void LocateController()
        {
            Controllers.Add(this);
            Controllers.Add(m_EmptyData);
        }

        public bool IsActive()
        {
            return gameObject.activeSelf;
        }

        public void OnEventRaise(EventObject gameEvent, EventsList events, EventParam param)
        {
            if (gameEvent == events.NONE)
            {
                //handle event
                Debug.Log("event none");
            }
        }

        public void BehaviourAwake()
        {
        }

        public void BehaviourStart()
        {
            Controllers.Get<EventController>().RaiseEvent(Controllers.Get<EventsList>().NONE, new EventParam());
        }
    }
}