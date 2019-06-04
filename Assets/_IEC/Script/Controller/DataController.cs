using System;
using UnityEngine;

namespace IEC
{
    public class DataController : MonoBehaviour, IController, IEventListener, IEventAwake, IEventStart
    {
        [SerializeField]
        private GlobalDataScope m_Data;

        public void LocateController()
        {
            Controllers.Add(this);
            Controllers.Add(m_Data);
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
            EventController.RaiseEvent(EventController.GetList().NONE, new EventParam());
            int emptyData = Controllers.Get<GlobalDataScope>().emptyData.data;
            Debug.Log("emptyData : " + emptyData);
        }
    }
}