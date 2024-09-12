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

    [System.Serializable]
    public class TabConfig
    {
        public TabType type;
        public Sprite sprite;
    }
}