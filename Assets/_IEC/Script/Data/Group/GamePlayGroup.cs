using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public class GamePlayGroup : _BaseBehaviour, IGroup
    {
        public Camera mainCamera;
        public Camera uiCamera;

        public void LocateGroup()
        {
            Pool.Set(this);
        }
    }
}