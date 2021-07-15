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
            NotificationBehaviour.Init();
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

        void RequestNotification()
        {
            if (PlayerData.level > 3)
            {
                NotificationBehaviour.Notification.RequestAuthorization();
            }
        }
    }
}