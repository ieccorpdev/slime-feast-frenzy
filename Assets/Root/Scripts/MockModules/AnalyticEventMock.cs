using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Root
{
    public class AnalyticEventMock : IAnalyticEvent
    {
        public void Init()
        {
        }

        public void PushEvent(string eventName, float value = 1, Dictionary<string, object> param = null)
        {
        }
    }

}

