using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public interface IEventLateUpdate: IEventBehaviour
    {
        void BehaviourLateUpdate(float deltaTime);
    }
}

