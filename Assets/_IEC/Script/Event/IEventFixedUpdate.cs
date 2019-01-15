using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IEC
{
    public interface IEventFixedUpdate : IEventBehaviour
    {
        void BehaviourFixedUpdate(float deltaTime);
    }
}
