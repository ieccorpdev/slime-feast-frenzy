using System;
using System.Collections.Generic;

namespace IEC
{
    public interface IEventListener : IEventBehaviour
    {
        void OnEventRaise(string gameEvent, EventParam param);
    }
}
