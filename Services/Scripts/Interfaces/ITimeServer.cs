using System;

namespace IEC
{
    public interface ITimeServer
    {
        void RequestTimeNow(Action<bool> callback);
        bool IsHaveTimeNow();
        DateTime GetTimeNowToLocal();
        DateTime GetTimeNow();
        void SetServerURL(string url);
    }
}

