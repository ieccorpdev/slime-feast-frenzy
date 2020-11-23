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
        public Transform animalParent;

        public void LocateGroup()
        {
            Pool.Set(this);
        }
    }
}