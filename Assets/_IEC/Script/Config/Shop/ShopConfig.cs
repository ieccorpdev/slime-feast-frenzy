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

        public GameObject itemShopObj;

        public Material[] waterLineMat1, waterLineMat2;

        public Material[] treebaseMats;
    }

    [System.Serializable]
    public class ThemeConfig
    {
        public List<Material> listTerrains;
        public Material wallMat, holeMat, cornerMat;

        public ItemShopConfig itemShopConfig;
    }

    [System.Serializable]
    public class BackgroundConfig
    {
        public Sprite sprMain, sprTop, sprBot;

        public ItemShopConfig itemShopConfig;
    }

    [System.Serializable]
    public class ItemShopConfig
    {
        public ItemShopType itemShopType;
        public string name;
        public Sprite spriteIcon;
    }
}