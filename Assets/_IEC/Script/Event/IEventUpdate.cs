using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IEC
{
    public interface IEventUpdate : IEventBehaviour
    {
        void BehaviourUpdate(float deltaTime);
    }
}
