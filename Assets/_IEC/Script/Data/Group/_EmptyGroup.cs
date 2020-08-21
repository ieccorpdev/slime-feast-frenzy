using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public class _EmptyGroup : _BaseBehaviour, IGroup
    {
        public GameObject emptyObject;
        public Transform emptyTransform;

        public void LocateGroup()
        {
            Pool.Set(this);
        }
    }
}