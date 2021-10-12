#if(PluginNotification)
#if UNITY_ANDROID
using System;
using Unity.Notifications.Android;

namespace _IECModules
{
    public class AndroidNotification : BaseNotification
    {
        private AndroidNotificationChannel m_AndroidNotificationChannel;

        public override void Init()
        {
            base.Init();

            AndroidNotificationCenter.CancelAllNotifications();

            m_AndroidNotificationChannel = new AndroidNotificationChannel
            {
                Id = NOTIFICATION_CHANNEL_ID,
                Name = $"{GameName} Notification Channel",
                Importance = Importance.High,
                Description = "Generic Notification Channel"
            };
            AndroidNotificationCenter.RegisterNotificationChannel(m_AndroidNotificationChannel);
        }

        public override void CancelAllNotification()
        {
            base.CancelAllNotification();
            AndroidNotificationCenter.CancelAllNotifications();
        }

        public override void ScheduleNotification(string title, string message, TimeSpan timeSpan)
        {
            base.ScheduleNotification(title, message, timeSpan);
            var notificationAndroid = new Unity.Notifications.Android.AndroidNotification
            {
                Title = title,
                Text = message,
                FireTime = DateTime.Now + timeSpan,
                SmallIcon = "small_icon",
                LargeIcon = "large_icon"
            };
            AndroidNotificationCenter.SendNotification(notificationAndroid, NOTIFICATION_CHANNEL_ID);
        }
    }
}
#endif
#endif
