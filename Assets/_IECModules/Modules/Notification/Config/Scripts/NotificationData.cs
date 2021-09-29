using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _IECModules
{
    [CreateAssetMenu(fileName = "NotificationData", menuName = "Services/NotificationData")]
    public class NotificationData : ScriptableObject
    {
        public List<string> listMgs = new List<string>();
        public List<NotificationInfo> listInfo = new List<NotificationInfo>();

        public string RandomMessage => listMgs.Sample();

        public TimeSpan GetTimeSpan(int id)
        {
            var info = listInfo[id];
            switch (info.timeType)
            {
                case NotificationTimeType.Second:
                    return TimeSpan.FromSeconds(info.time);
                case NotificationTimeType.Hour:
                    return TimeSpan.FromHours(info.time);
                case NotificationTimeType.Day:
                    return TimeSpan.FromDays(info.time);
            }
            return TimeSpan.FromDays(info.time);
        }

        public string GetMessage(int id)
        {
            var info = listInfo[id];
            if (string.IsNullOrEmpty(info.message)) return RandomMessage;
            return info.message;
        }
    }

    [Serializable]
    public class NotificationInfo
    {
        public string message;
        public float time;
        public NotificationTimeType timeType;
    }

    public enum NotificationTimeType
    {
        Second = 0,
        Hour = 1,
        Day = 2,
    }
}