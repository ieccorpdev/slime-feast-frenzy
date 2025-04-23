using UnityEngine;

namespace _IEC.Features.Gameplay.ObserverEventData
{
    [System.Serializable]
    public class UpdateCameraTarget
    {
        public Transform target;
        public int curLevel;
        public int nextLevel;
    }
}