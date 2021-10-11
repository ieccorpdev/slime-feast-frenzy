using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    [CreateAssetMenu(menuName = "Shop/Theme Config")]
    public class ThemeConfig : ItemShopConfig
    {
        public List<Material> listTerrains;
        public Material wallMat, holeMat, cornerMat;
        public WaterLineConfig waterLineConfig;

        public int wallType, wallPipeType;

        public Color leaveEffectColorMin, leaveEffectColorMax;
    }
}