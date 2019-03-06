using System;
using System.Collections.Generic;

namespace IEC
{
    public interface IEventUpdate : IEventBehaviour
    {
        void BehaviourUpdate(float deltaTime);
    }
}
