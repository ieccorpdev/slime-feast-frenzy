using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    [CreateAssetMenu(menuName = "Game/WaterLineConfig")]
    public class WaterLineConfig : ScriptableObject
    {
        public List<Material> listWaterLineMat;
        public List<Material> listRootTreeMat;
    }
}