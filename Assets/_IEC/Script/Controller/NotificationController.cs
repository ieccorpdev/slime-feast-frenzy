using System;
using _IECModules;
using UnityEngine;
using Random = UnityEngine.Random;
#if UNITY_IOS
using Unity.Notifications.iOS;
#else
using Unity.Notifications.Android;

#endif

namespace IEC
{
    public class NotificationController : _BaseBehaviour, IController, IEventStart, IEventPauseApp, IEventQuitApp
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
        public void BehaviourPauseApp(bool isPause)
        {
            if (isPause)
            {
                if (Game.isHaveNotification)
                {
                    NotificationBehaviour.RegisterNotification();
                }
            }
        }

        public void BehaviourQuitApp()
        {
            if (Game.isHaveNotification)
            {
                NotificationBehaviour.RegisterNotification();
            }
        }

    }
}