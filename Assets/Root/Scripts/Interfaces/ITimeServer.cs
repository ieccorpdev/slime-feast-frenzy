using System;

namespace Root
{
    public interface ITimeServer
    {
        void RequestTimeNow(Action<bool> callback);
        bool IsHaveTimeNow();
        DateTime GetTimeNow();
    }
}

