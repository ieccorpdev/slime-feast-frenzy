using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IEC
{
    public interface IEventListener : IEventBehaviour
    {
        void SetListenEvents(EventController controller, EventsList events);
    }
}
