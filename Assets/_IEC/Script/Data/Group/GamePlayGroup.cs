using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public class GamePlayGroup : _BaseBehaviour, IGroup
    {
        public Camera mainCamera;
        public Transform transTutHand;
        public Transform effectParent;
        public BackgroundGroup backgroundGroup;

        public void LocateGroup()
        {
            Pool.Set(this);
        }
    }
}