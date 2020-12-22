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

        public Transform[] wallParents;   // TOP, BOT, LEFT, RIGHT

        public List<CornerComponent> listWallCorners;   // TOP_LEFT, TOP_RIGHT, BOT_LEFT, BOT_RIGHT

        public Transform transGridParent;
        public Transform transMainCam;

        public void LocateGroup()
        {
            Pool.Set(this);
        }
    }
}