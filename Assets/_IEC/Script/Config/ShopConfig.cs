using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    [CreateAssetMenu(menuName = "Game/ShopConfig")]
    public class ShopConfig : ScriptableObject
    {
        public List<ThemeConfig> listEnviromentConfigs;
        public List<BackgroundConfig> listSpriteBackgrounds;
    }

    [System.Serializable]
    public struct BackgroundConfig
    {
        public Sprite sprMain, sprTop, sprBot;
    }
}