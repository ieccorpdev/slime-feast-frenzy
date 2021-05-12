using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _IECModules
{
    public class NotificationBehaviour : BaseBehaviour
    {
        public NotificationData Data => SingleCreateIfNotExist(() => Resources.Load<NotificationData>("NotificationData"));

        public BaseNotification Notification => SingleCreateIfNotExist(() =>
        {
#if (PluginNotification)
#if UNITY_IOS
            return new IOSNotification();
#endif
#if UNITY_ANDROID
            return new AndroidNotification();
#endif
#endif
            return new BaseNotification();
        });

        private void Start()
        {
            Init();
        }

        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                RegisterNotification();
            }
        }

        private void OnApplicationQuit()
        {
            RegisterNotification();
        }

        public void Init()
        {
            Notification.Init();
        }

        void RegisterNotification()
        {
            if (Data == null) return;
            Notification.CancelAllNotification();
            if (Notification.CheatCodeShowNotificationTime != 0)
            {
                Notification.ScheduleNotification(BaseNotification.GameName, Data.RandomMessage, TimeSpan.FromSeconds(Notification.CheatCodeShowNotificationTime));
            }
            for (int i = 0; i < Data.listInfo.Count; i++)
            {
                Notification.ScheduleNotification(BaseNotification.GameName, Data.GetMessage(i), Data.GetTimeSpan(i));
            }
        }
    }
}
