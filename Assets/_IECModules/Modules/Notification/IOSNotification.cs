#if(PluginNotification)
using System;
using Unity.Notifications.iOS;
using NotificationServices = UnityEngine.iOS.NotificationServices;
using NotificationType = UnityEngine.iOS.NotificationType;

namespace _IECModules
{
    public class IOSNotification : BaseNotification
    {
        public override void CancelAllNotification()
        {
            base.CancelAllNotification();

            iOSNotificationCenter.RemoveAllScheduledNotifications();
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