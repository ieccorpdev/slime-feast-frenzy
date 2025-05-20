#if (PluginNotification)
#if UNITY_IOS
using System;
using Unity.Notifications.iOS;
using NotificationServices = UnityEngine.iOS.NotificationServices;
using NotificationType = UnityEngine.iOS.NotificationType;

namespace _IECModules
{
    public class IOSNotification : BaseNotification
    {
        public override void RequestAuthorization()
        {
            base.RequestAuthorization();
            NotificationServices.RegisterForNotifications(NotificationType.Alert | NotificationType.Badge | NotificationType.Sound);
        }

        public override void CancelAllNotification()
        {
            base.CancelAllNotification();

            iOSNotificationCenter.RemoveAllScheduledNotifications();
        }

        public override void ScheduleNotification(string title, string message, TimeSpan timeInTargetZone, string timeZoneId)
        {
            base.ScheduleNotification(title, message, timeInTargetZone, timeZoneId);
            TimeZoneInfo targetZone;
            try
            {
                targetZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            }
            catch
            {
                targetZone = TimeZoneInfo.Local;
            }

            DateTime nowTargetZone = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, targetZone);
            DateTime targetTime = new DateTime(
                nowTargetZone.Year, nowTargetZone.Month, nowTargetZone.Day,
                timeInTargetZone.Hours, timeInTargetZone.Minutes, timeInTargetZone.Seconds
            );

            if (nowTargetZone > targetTime)
                targetTime = targetTime.AddDays(1);

            DateTime localTriggerTime = TimeZoneInfo.ConvertTimeFromUtc(targetTime.ToUniversalTime(), TimeZoneInfo.Local);

            var trigger = new iOSNotificationCalendarTrigger
            {
                Hour = localTriggerTime.Hour,
                Minute = localTriggerTime.Minute,
                Second = localTriggerTime.Second,
                Repeats = true
            };

            var notification = new iOSNotification
            {
                Title = title,
                Body = message,
                ShowInForeground = true,
                ForegroundPresentationOption = PresentationOption.Alert | PresentationOption.Sound,
                CategoryIdentifier = "category_a",
                ThreadIdentifier = "thread1",
                Trigger = trigger
            };

            iOSNotificationCenter.ScheduleNotification(notification);
        }

        public override void ScheduleNotification(string title, string message, TimeSpan timeSpan)
        {
            base.ScheduleNotification(title, message, timeSpan);

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
        }
    }
}
#endif
#endif
