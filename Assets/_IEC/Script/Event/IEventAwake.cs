using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IEC
{
    public interface IEventAwake : IEventBehaviour
    {
        void BehaviourAwake();
    }
}
