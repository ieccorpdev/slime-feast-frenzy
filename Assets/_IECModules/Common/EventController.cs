using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace IEC
{
    public class EventController : MonoBehaviour
    {
        private static EventController s_Instance;
        private static StaticObjectsContainer s_StaticObjects;

        private static bool s_IsCheckInstance;

        [SerializeField]
        private bool m_IsDontDestroy;

        IEventListener[] m_Listeners;
        IEventAwake[] m_AwakeEvents;
        IEventStart[] m_StartEvents;
        IEventUpdate[] m_UpdateEvents;
        IEventFixedUpdate[] m_FixedUpdateEvents;
        IEventPauseApp[] m_PauseAppEvents;
        IEventQuitApp[] m_QuitAppEvents;
        IEventLateUpdate[] m_LateUpdateEvents;

        void Awake()
        {
            if (!s_IsCheckInstance)
            {
                s_IsCheckInstance = true;
                var instances = FindObjectsOfType<EventController>();
                int count = 0;
                for (int i = 0; i < instances.Length; i++)
                {
                    if (instances[i] != null && s_Instance != instances[i])
                    {
                        count++;
                    }
                }
                if (count > 1)
                {
                    throw new Exception("More than 1 EventController on Scene");
                }
                else
                {
                    for (int i = 0; i < instances.Length; i++)
                    {
                        if (instances[i] != null && s_Instance != instances[i])
                        {
                            if (s_Instance != null)
                            {
                                Destroy(s_Instance.gameObject);
                            }
                            s_Instance = instances[i];
                            break;
                        }
                    }
                }
            }
            if (s_Instance != this) return;

            Debug.Log("EventController Awake");

            if (m_IsDontDestroy)
            {
                DontDestroyOnLoad(gameObject);
            }

            if (s_StaticObjects == null)
            {
                s_StaticObjects = FindObjectOfType<StaticObjectsContainer>();
                if (s_StaticObjects)
                {
                    DontDestroyOnLoad(s_StaticObjects.gameObject);
                }
            }
            else
            {
                var otherStaticContainers = FindObjectsOfType<StaticObjectsContainer>();
                foreach (var item in otherStaticContainers)
                {
                    if (item != s_StaticObjects)
                        Destroy(item.gameObject);
                }
            }

            LocateController();
            LocateGroup();

            var consentController = FindObjectOfType<ConsentController>();
            if (consentController != null)
            {
                gameObject.SetActive(false);

                consentController.StartGetConsent(Controllers.Get<ServicesController>().config, () =>
                {
                    gameObject.SetActive(true);
                    InitFromAwake();
                });
            }
            else
            {
                InitFromAwake();
            }
        }

        void InitFromAwake()
        {
            m_Listeners = GetEventArray<IEventListener>();
            m_AwakeEvents = GetEventArray<IEventAwake>();
            m_StartEvents = GetEventArray<IEventStart>();
            m_UpdateEvents = GetEventArray<IEventUpdate>();
            m_FixedUpdateEvents = GetEventArray<IEventFixedUpdate>();
            m_PauseAppEvents = GetEventArray<IEventPauseApp>();
            m_QuitAppEvents = GetEventArray<IEventQuitApp>();
            m_LateUpdateEvents = GetEventArray<IEventLateUpdate>();

            if (Application.isEditor)
            {
                Debug.unityLogger.filterLogType = LogType.Log;
            }
            else
            {
                Debug.unityLogger.filterLogType = LogType.Warning;
            }

            CameraHelper.SetMainCam(Camera.main);
            Application.targetFrameRate = 60;
            //m_CustomFixedUpdate = new CustomFixedUpdate(OnCustomFixedUpdate, 10);

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
            Debug.Log("EventController Start");
            s_IsCheckInstance = false;
            if (m_StartEvents == null) return;
            foreach (var eventStart in m_StartEvents)
            {
                if (eventStart.IsActive())
                {
                    eventStart.BehaviourStart();
                }
            }
        }

        private T[] GetEventArray<T>() where T : IEventBehaviour
        {
            var listeners = new List<T>();

            if (s_StaticObjects != null)
            {
                GetListFromObject(s_StaticObjects.staticControllersParent, listeners);
            }
            GetListFromObject(transform, listeners);

            return listeners.ToArray();
        }

        void GetListFromObject<T>(Transform parentT, List<T> list) where T : IEventBehaviour
        {
            for (var i = 0; i < parentT.childCount; i++)
            {
                var child = parentT.GetChild(i);
                var listener = parentT.GetChild(i).GetComponent<T>();
                if (listener != null && listener.IsActive())
                {
                    list.Add(listener);
                }

                for (var j = 0; j < child.childCount; j++)
                {
                    listener = child.GetChild(j).GetComponent<T>();
                    if (listener != null && listener.IsActive())
                    {
                        list.Add(listener);
                    }
                }
            }
        }

        void Update()
        {
            if (m_UpdateEvents == null) return;
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
            if (m_FixedUpdateEvents == null) return;
            float deltaTime = Time.fixedDeltaTime;
            for (int i = 0; i < m_FixedUpdateEvents.Length; i++)
            {
                if (m_FixedUpdateEvents[i].IsActive())
                {
                    m_FixedUpdateEvents[i].BehaviourFixedUpdate(deltaTime);
                }
            }
        }

        void LateUpdate()
        {
            if (m_LateUpdateEvents == null) return;
            float deltaTime = Time.deltaTime;
            for (int i = 0; i < m_LateUpdateEvents.Length; i++)
            {
                if (m_LateUpdateEvents[i].IsActive())
                {
                    m_LateUpdateEvents[i].BehaviourLateUpdate(deltaTime);
                }
            }
        }

        void OnApplicationPause(bool pause)
        {
            if (m_PauseAppEvents != null)
            {
                for (int i = 0; i < m_PauseAppEvents.Length; i++)
                {
                    if (m_PauseAppEvents[i].IsActive())
                    {
                        m_PauseAppEvents[i].BehaviourPauseApp(pause);
                    }
                }
            }
        }

        void OnApplicationQuit()
        {
            if (m_QuitAppEvents != null)
            {
                for (int i = 0; i < m_QuitAppEvents.Length; i++)
                {
                    if (m_QuitAppEvents[i].IsActive())
                    {
                        m_QuitAppEvents[i].BehaviourQuitApp();
                    }
                }
            }
        }

        void RaiseEventLocal(string gameEvent, EventParam param)
        {
            for (int i = 0; i < m_Listeners.Length; i++)
            {
                if (m_Listeners[i].IsActive())
                {
                    m_Listeners[i].OnEventRaise(gameEvent, param);
                }
            }
        }

        public static void RaiseEvent(string gameEvent, EventParam param = new EventParam())
        {
            if (s_Instance != null)
            {
                s_Instance.RaiseEventLocal(gameEvent, param);
            }
        }

        public static void RaiseEventDelay(float timeDelay, string gameEvent, EventParam param = new EventParam())
        {
            if (s_Instance != null)
            {
                s_Instance.StartCoroutine(IERaiseEventDelay(timeDelay, gameEvent, param));
            }
        }

        static IEnumerator IERaiseEventDelay(float timeDelay, string gameEvent, EventParam param)
        {
            if (s_Instance != null)
            {
                yield return new WaitForSeconds(timeDelay);
                s_Instance.RaiseEventLocal(gameEvent, param);
            }
        }

        void LocateController()
        {
            Controllers.Add(this);

            if (s_StaticObjects != null)
            {
                LocateController(s_StaticObjects.staticControllersParent);
            }
            LocateController(transform);
        }

        void LocateController(Transform parentT)
        {
            for (int i = 0; i < parentT.childCount; i++)
            {
                var go = parentT.GetChild(i).gameObject;
                var controller = go.GetComponent<IController>();
                if (controller != null && go.activeSelf)
                {
                    controller.LocateController();
                }

                var child = parentT.GetChild(i);
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

        void LocateGroup()
        {
            var groups = FindObjectsOfType<MonoBehaviour>().OfType<IGroup>();
            foreach (var g in groups)
            {
                g.LocateGroup();
            }
        }
    }

    [Serializable]
    public struct EventListeners
    {
        public MonoBehaviour[] listenersBehaviour;
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
