using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    [CreateAssetMenu(menuName = "Game/ComponentConfig")]
    public class ComponentConfig : ScriptableObject
    {
        public GameObject sourceObj;
        public GameObject targetObj;
        public GameObject wallObj;
        public GameObject cornerObj;
        public GameObject straightObj;
        public GameObject crossObj;
        public GameObject curveObj;

        public GameObject noneObj;

        public GameObject islandObj;
    }
}
//-90 0 180 90