using System;
using System.Collections.Generic;

namespace IEC
{
    public interface IEventFixedUpdate : IEventBehaviour
    {
        void BehaviourFixedUpdate(float deltaTime);
    }
}
