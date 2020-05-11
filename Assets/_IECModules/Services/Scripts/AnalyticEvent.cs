using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC.TrackingEvent
{
    public abstract class AnalyticEvent
    {
        public virtual string Name => "EVENT";

        public virtual float GetValue()
        {
            return 0f;
        }

        public virtual Dictionary<string, object> GetParams()
        {
            return new Dictionary<string, object>();
        }
    }
}