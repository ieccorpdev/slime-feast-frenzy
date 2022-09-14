using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using _IECModules;

namespace IEC
{
    public class NotificationController : _BaseBehaviour, IController, IEventListener, IEventStart
    {
        public NotificationBehaviour NotificationBehaviour => ModuleController.notificationBehaviour;

        public void BehaviourStart()
        {
            if (PlayerData.level > 3)
            {
                NotificationBehaviour.Init();
                m_IsInitNoti = true;
            }
        }

        public bool IsActive()
        {
            return gameObject.activeSelf;
        }

        public void LocateController()
        {
            Controllers.Add(this);
        }

        public void OnEventRaise(string gameEvent, EventParam param)
        {
            switch (gameEvent)
            {
                case EventList.GamePlay.LEVEL_WIN:
                    RequestNotification();
                    break;
            }
        }

        private bool m_IsInitNoti;

        void RequestNotification()
        {
            if (PlayerData.level > 3)
            {
                if (!m_IsInitNoti) NotificationBehaviour.Init();
                NotificationBehaviour.Notification.RequestAuthorization();
                m_IsInitNoti = true;
            }
        }
    }
}