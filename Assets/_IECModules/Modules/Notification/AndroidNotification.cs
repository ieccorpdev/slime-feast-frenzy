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

            var permissionRequest = new PermissionRequest();
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

            if (nowTargetZone >= targetTime)
                targetTime = targetTime.AddDays(1);

            DateTime fireTimeLocal = TimeZoneInfo.ConvertTime(targetTime, targetZone, TimeZoneInfo.Local);

            var notificationAndroid = new Unity.Notifications.Android.AndroidNotification
            {
                Title = title,
                Text = message,
                FireTime = fireTimeLocal,
                RepeatInterval = TimeSpan.FromDays(1),
                SmallIcon = "small_icon",
                LargeIcon = "large_icon"
            };

            AndroidNotificationCenter.SendNotification(notificationAndroid, NOTIFICATION_CHANNEL_ID);
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
