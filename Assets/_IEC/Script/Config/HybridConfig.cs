using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace IEC
{
    [CreateAssetMenu(menuName = "Game/HybridConfig")]
    public class HybridConfig : BaseConfig
    {
        public List<TabConfig> listTabConfig;
        public LifeConfig lifeConfig;
        private Dictionary<TabType, TabConfig> m_DictConfigByType;

        public TabConfig GetConfig(TabType type)
        {
            if (m_DictConfigByType == null)
            {
                m_DictConfigByType = listTabConfig.ToDictionary(x => x.type, x => x);
            }

            return m_DictConfigByType[type];
        }
    }

    [Serializable]
    public class TabConfig
    {
        public TabType type;
        public Sprite spriteOn;
        public Sprite spriteOff;
    }
    
    [Serializable]
    public class LifeConfig
    {
        public int maxLife = 5;
        [Tooltip("Time refill in second")]
        public int timeRefill = 30 * 60;

        public int refillPrice = 900;
    }
}