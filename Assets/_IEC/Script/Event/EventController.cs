using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public class EventController : MonoBehaviour
    {
        [SerializeField]
        EventsList m_EventsList;

        IEventListener[] m_Listeners;
        IEventAwake[] m_AwakeEvents;
        IEventStart[] m_StartEvents;
        IEventUpdate[] m_UpdateEvents;
        IEventFixedUpdate[] m_FixedUpdateEvents;

        public static long UpdateStep { get; private set; }

        Dictionary<int, List<Action<EventParam>>> m_DictEvents = new Dictionary<int, List<Action<EventParam>>>();

        void Awake()
        {
            LocateController();
            //ServicesManager.Instance.InitServices(true, false, false);

            CameraHelper.SetMainCam(Camera.main);
            UpdateStep = 0;

            m_Listeners = GetEventArray<IEventListener>();
            m_AwakeEvents = GetEventArray<IEventAwake>();
            m_StartEvents = GetEventArray<IEventStart>();
            m_UpdateEvents = GetEventArray<IEventUpdate>();
            m_FixedUpdateEvents = GetEventArray<IEventFixedUpdate>();

            Application.targetFrameRate = 60;
            //m_CustomFixedUpdate = new CustomFixedUpdate(OnCustomFixedUpdate, 10);

            SetListenEvents();

            for (int i = 0; i < m_AwakeEvents.Length; i++)
            {
                if (m_AwakeEvents[i].IsActive())
                {
                    m_AwakeEvents[i].BehaviourAwake();
                }
            }
        }

        void Start()
        {
            for (int i = 0; i < m_StartEvents.Length; i++)
            {
                if (m_StartEvents[i].IsActive())
                {
                    m_StartEvents[i].BehaviourStart();
                }
            }
        }

        T[] GetEventArray<T>() where T : IEventBehaviour
        {
            List<T> listeners = new List<T>();

            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                var listener = transform.GetChild(i).GetComponent<T>();
                if (listener != null && listener.IsActive())
                {
                    listeners.Add(listener);
                }

                for (int j = 0; j < child.childCount; j++)
                {
                    listener = child.GetChild(j).GetComponent<T>();
                    if (listener != null && listener.IsActive())
                    {
                        listeners.Add(listener);
                    }
                }
            }
            return listeners.ToArray();
        }

        void Update()
        {
            float deltaTime = Time.deltaTime;
            for (int i = 0; i < m_UpdateEvents.Length; i++)
            {
                if (m_UpdateEvents[i].IsActive())
                {
                    m_UpdateEvents[i].BehaviourUpdate(deltaTime);
                }
            }
            //m_CustomFixedUpdate.Update();
        }

        void FixedUpdate()
        {
            UpdateStep++;
            float deltaTime = Time.fixedDeltaTime;
            for (int i = 0; i < m_FixedUpdateEvents.Length; i++)
            {
                if (m_FixedUpdateEvents[i].IsActive())
                {
                    m_FixedUpdateEvents[i].BehaviourFixedUpdate(deltaTime);
                }
            }
        }

        // public void RaiseEvent(GameEvent gameEvent, object sender, params object[] param)
        // {
        //     for (int i = 0; i < m_Listeners.Length; i++)
        //     {
        //         if (m_Listeners[i].IsActive())
        //         {
        //             m_Listeners[i].OnEventRaise(gameEvent, sender, param);
        //         }
        //     }
        // }

        public void RaiseEvent(EventObject gameEvent, EventParam param)
        {
            var eventKey = gameEvent.GetInstanceID();

            if (m_DictEvents.ContainsKey(eventKey))
            {
                var list = m_DictEvents[eventKey];
                for (int i = 0; i < list.Count; i++)
                {
                    list[i].Invoke(param);
                }
            }
        }

        public void ListenEvent(EventObject gameEvent, Action<EventParam> onEvent)
        {
            var eventKey = gameEvent.GetInstanceID();

            if (!m_DictEvents.ContainsKey(eventKey))
            {
                m_DictEvents[eventKey] = new List<Action<EventParam>>();
            }
            m_DictEvents[eventKey].Add(onEvent);
        }

        public void ListenEvent(EventObject[] gameEvents, Action<EventParam> onEvent)
        {
            for (int i = 0; i < gameEvents.Length; i++)
            {
                ListenEvent(gameEvents[i], onEvent);
            }
        }

        public void ListenEvent(Action<EventParam> onEvent, params EventObject[] gameEvents)
        {
            for (int i = 0; i < gameEvents.Length; i++)
            {
                ListenEvent(gameEvents[i], onEvent);
            }
        }

        void LocateController()
        {
            Controllers.Add(this);
            Controllers.Add(m_EventsList);

            for (int i = 0; i < transform.childCount; i++)
            {
                var go = transform.GetChild(i).gameObject;
                var controller = go.GetComponent<IController>();
                if (controller != null && go.activeSelf)
                {
                    controller.LocateController();
                }

                var child = transform.GetChild(i);
                for (int j = 0; j < child.childCount; j++)
                {
                    go = child.GetChild(j).gameObject;
                    controller = go.GetComponent<IController>();
                    if (controller != null && go.activeSelf)
                    {
                        controller.LocateController();
                    }
                }
            }
        }

        void SetListenEvents()
        {
            for (int i = 0; i < m_Listeners.Length; i++)
            {
                if (m_Listeners[i].IsActive())
                {
                    m_Listeners[i].SetListenEvents(this, m_EventsList);
                }
            }
        }
    }

    public struct EventParam
    {
        public object sender;
        public object p0;
        public object p1;
        public object p2;
        public object p3;
        public object p4;

        public EventParam(object sender)
        {
            this.sender = sender;
            p0 = p1 = p2 = p3 = p4 = null;
        }

        public EventParam(object sender, object p0)
        {
            this.sender = sender;
            this.p0 = p0;
            p1 = p2 = p3 = p4 = null;
        }

        public EventParam(object sender, object p0, object p1)
        {
            this.sender = sender;
            this.p0 = p0;
            this.p1 = p1;
            p2 = p3 = p4 = null;
        }

        public EventParam(object sender, object p0, object p1, object p2)
        {
            this.sender = sender;
            this.p0 = p0;
            this.p1 = p1;
            this.p2 = p2;
            p3 = p4 = null;
        }

        public EventParam(object sender, object p0, object p1, object p2, object p3)
        {
            this.sender = sender;
            this.p0 = p0;
            this.p1 = p1;
            this.p2 = p2;
            this.p3 = p3;
            p4 = null;
        }

        public EventParam(object sender, object p0, object p1, object p2, object p3, object p4)
        {
            this.sender = sender;
            this.p0 = p0;
            this.p1 = p1;
            this.p2 = p2;
            this.p3 = p3;
            this.p4 = p4;
        }
    }
}
