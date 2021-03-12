using System;

namespace IEC
{
    public interface ITimeServer
    {
        void SetCheckType(TimeCheckType checkType);
        void RequestTimeAuto(float delayTime, Action<bool> callback);
        void RequestTimeNow(Action<bool> callback);
        bool IsHaveTimeNow();
        DateTime GetTimeNowToLocal();
        DateTime GetTimeNow();
        void SetServerURL(string url);
    }

    public enum TimeCheckType
    {
        AllOffline = 0,
        AllOnline = 1,
        Mix = 2
    }
}

