using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    [CreateAssetMenu(menuName = "Game/ShopConfig")]
    public class ShopConfig : ScriptableObject
    {
        public List<ThemeConfig> listThemeConfigs;
        public List<BackgroundConfig> listBGConfig;

        public List<Material> listTerrainIsLand1, listTerrainIsLand2;

        public GameObject itemShopObj;
    }
}