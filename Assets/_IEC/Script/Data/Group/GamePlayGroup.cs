using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public class GamePlayGroup : MonoBehaviour, IGroup
    {
        public Camera mainCamera;

        public void LocateGroup()
        {
            Pool.Set(this);
        }
    }
}