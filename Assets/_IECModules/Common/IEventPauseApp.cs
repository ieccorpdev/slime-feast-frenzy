using System;
using System.Collections.Generic;

namespace IEC
{
    public interface IEventPauseApp : IEventBehaviour
    {
        void BehaviourPauseApp(bool isPause);
    }
}
