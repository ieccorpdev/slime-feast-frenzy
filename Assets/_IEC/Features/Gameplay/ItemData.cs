using UnityEngine;

namespace _IEC.Features.Gameplay
{
    [System.Serializable]
    public class ItemData
    {
        public string groupID;
        public Vector3 groupPosition;
        public string itemId;
        public Vector3 position;
        public Vector3 eulerAngle;
        public Vector3 localScale = Vector3.one;
        public bool isStartKinematic;
        // unknown
        public string cont;
        public int oLvl;
    }
}