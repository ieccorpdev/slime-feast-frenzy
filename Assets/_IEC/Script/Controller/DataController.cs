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

        public void OnEventRaise(string gameEvent, EventParam param)
        {
            switch (gameEvent)
            {
                case EventList.NONE:
                    Debug.Log("event none");
                    break;
            }
        }

        public void BehaviourAwake()
        {
        }

        public void BehaviourStart()
        {
            EventController.RaiseEvent(EventList.NONE);
            int emptyData = Controllers.Get<GlobalDataScope>().emptyData.data;
            
            Debug.Log("emptyData : " + emptyData);
        }
    }
}