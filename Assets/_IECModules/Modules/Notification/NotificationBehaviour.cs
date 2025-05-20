using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IEC;
namespace _IECModules
{
    public class NotificationBehaviour : BaseBehaviour
    {
        public NotificationData Data => SingleCreateIfNotExist(() => Resources.Load<NotificationData>("NotificationData"));

        private bool m_IsInit;
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
            m_IsInit = true;
        }

        void RegisterNotification()
        {
            if (Data == null || !m_IsInit) return;
            Notification.CancelAllNotification();
            if (Notification.CheatCodeShowNotificationTime != 0)
            {
                Notification.ScheduleNotification(BaseNotification.GameName, Data.RandomMessage, TimeSpan.FromSeconds(Notification.CheatCodeShowNotificationTime));
            }
            for (int i = 0; i < Data.listInfo.Count; i++)
            {
                Notification.ScheduleNotification(BaseNotification.GameName, Data.GetMessage(i), Data.GetTimeSpan(i));
            }
            for (int i = 0; i < Data.listInfoChooseTime.Count; i++)
            {
                Notification.ScheduleNotification(BaseNotification.GameName, Data.GetMessage(i), Data.GetTimeSpanChooseTime(i));
            }
        }
    }
}
