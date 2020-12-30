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
    public class NotificationController : MonoBehaviour, IController, IEventListener, IEventStart, IEventPauseApp, IEventQuitApp
    {
        private string[] notiMsg =
        {
            "The more you play the more logical you are. Test with your friends now!",
            "Play now, Connect the pipe, No Matter Where You Are.",
            "Rotate the pipe and connect all the pipe, Do brain exercise now!!",
            "Rotate the pipe and connect all the pipe, Play & relax now!!",
            "Play Water Connect Puzzle and compete with your friends now!"
        };

        public static string GameName => Application.productName;

        public const string NOTIFICATION_CHANNEL_ID = "notification_channel";

        private int m_CheatCodeShowNotificationTime;

#if UNITY_ANDROID
        private AndroidNotificationChannel m_AndroidNotificationChannel;
#endif

        public void LocateController()
        {
            Controllers.Add(this);
        }

        public bool IsActive()
        {
            return gameObject.activeSelf;
        }

        public void BehaviourStart()
        {
            if (!Pool.Get<Game>().isHaveNotification) return;
#if UNITY_ANDROID
            AndroidNotificationCenter.CancelAllNotifications();

            m_AndroidNotificationChannel = new AndroidNotificationChannel
            {
                Id = NOTIFICATION_CHANNEL_ID,
                Name = $"{GameName} Notification Channel",
                Importance = Importance.High,
                Description = "Generic Notification Channel"
            };
            AndroidNotificationCenter.RegisterNotificationChannel(m_AndroidNotificationChannel);
#endif

            ModuleController.cheatCode.AddCheatCodeButtonWithInput("Send Test Notification",
                param => { m_CheatCodeShowNotificationTime = int.Parse(param.inputParams[0].value.ToString()); },
                new CheatCode.InputParam {label = "Second", value = 10});
        }

        public void OnEventRaise(string gameEvent, EventParam param)
        {
        }

        public string RandomMessage()
        {
            return notiMsg[Random.Range(0, notiMsg.Length)];
        }

        public void RegisterNotification()
        {
            CancelAllNotification();

            if (m_CheatCodeShowNotificationTime != 0)
                ScheduleNotification(GameName, RandomMessage(), TimeSpan.FromSeconds(m_CheatCodeShowNotificationTime));
            ScheduleNotification(GameName, RandomMessage(), TimeSpan.FromDays(1));
            ScheduleNotification(GameName, RandomMessage(), TimeSpan.FromDays(2));
            ScheduleNotification(GameName, RandomMessage(), TimeSpan.FromDays(7));
            ScheduleNotification(GameName, RandomMessage(), TimeSpan.FromDays(14));
            ScheduleNotification(GameName, RandomMessage(), TimeSpan.FromDays(30));
        }

        public void CancelAllNotification()
        {
#if UNITY_IOS
            iOSNotificationCenter.RemoveAllScheduledNotifications();
#else
            AndroidNotificationCenter.CancelAllNotifications();
#endif
        }

        public void ScheduleNotification(string title, string message, TimeSpan timeSpan)
        {
#if UNITY_IOS
            ScheduleNotificationIOS(title, message, timeSpan);

#else
            ScheduleNotificationAndroid(title, message, timeSpan);
#endif
        }

        public void ScheduleNotificationIOS(string title, string message, TimeSpan timeSpan)
        {
#if UNITY_IOS
            var timeTrigger = new iOSNotificationTimeIntervalTrigger
            {
                TimeInterval = timeSpan,
                Repeats = false
            };

            var notificationIOS = new iOSNotification
            {
                // You can optionally specify a custom Identifier which can later be 
                // used to cancel the notification, if you don't set one, an unique 
                // string will be generated automatically.
//                Identifier = "_notification_01",
                Title = title,
                Body = message,
//                Subtitle = "This is a subtitle, something, something important...",
                ShowInForeground = true,
                ForegroundPresentationOption = PresentationOption.Alert | PresentationOption.Sound,
                CategoryIdentifier = "category_a",
                ThreadIdentifier = "thread1",
                Trigger = timeTrigger
            };

            iOSNotificationCenter.ScheduleNotification(notificationIOS);
#endif
        }

        public void ScheduleNotificationAndroid(string title, string message, TimeSpan timeSpan)
        {
#if UNITY_ANDROID
            var notificationAndroid = new AndroidNotification
            {
                Title = title, Text = message, FireTime = DateTime.Now + timeSpan, SmallIcon = "small_icon",
                LargeIcon = "large_icon"
            };
            AndroidNotificationCenter.SendNotification(notificationAndroid, NOTIFICATION_CHANNEL_ID);
#endif
        }

        public void BehaviourPauseApp(bool isPause)
        {
            if (isPause)
            {
                RegisterNotification();
            }
        }

        public void BehaviourQuitApp()
        {
            RegisterNotification();
        }
    }
}