using UnityEngine;

namespace IEC
{
    public class _EmptyController : MonoBehaviour, IController, IEventListener, IEventAwake, IEventStart
    {
        public void LocateController()
        {
            Controllers.Add(this);
        }
        
        public bool IsActive()
        {
            return gameObject.activeSelf;
        }

        public void OnEventRaise(EventObject gameEvent, EventsList events, EventParam param)
        {
            if(gameEvent == events.NONE)
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
        }
    }
}