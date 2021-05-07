using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _IECModules
{
    public class BaseNotification
    {
        public static string GameName => Application.productName;

        public const string NOTIFICATION_CHANNEL_ID = "notification_channel";

        protected int m_CheatCodeShowNotificationTime;

        public int CheatCodeShowNotificationTime => m_CheatCodeShowNotificationTime;

        public virtual void Init()
        {
            Debug.LogWarning("Notification: Init");
            ModuleController.cheatCode.AddCheatCodeButtonWithInput("Send Test Notification",
                param => { m_CheatCodeShowNotificationTime = int.Parse(param.inputParams[0].value.ToString()); },
                new CheatCode.InputParam { label = "Second", value = 10 });
        }

        public virtual void CancelAllNotification()
        {
            Debug.LogWarning("Notification: CancelAllNotification");
        }

        public virtual void ScheduleNotification(string title, string message, TimeSpan timeSpan)
        {
            Debug.LogWarning("Notification: ScheduleNotification");
        }

        public virtual void RequestAuthorization()
        {
            Debug.LogWarning("Notification: RequestAuthorization");
        }
    }
}