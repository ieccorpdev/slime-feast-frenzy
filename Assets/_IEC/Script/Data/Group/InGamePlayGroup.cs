using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public class InGamePlayGroup : _BaseBehaviour, IGroup
    {
        public void LocateGroup()
        {
            Pool.Set(this);
        }
    }
}