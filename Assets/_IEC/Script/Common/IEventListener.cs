using System;
using System.Collections.Generic;

namespace IEC
{
    public interface IEventListener : IEventBehaviour
    {
        void OnEventRaise(EventObject gameEvent, EventsList events, EventParam param);
    }
}
