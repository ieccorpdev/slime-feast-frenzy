using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace IEC
{
    [CreateAssetMenu(menuName = "Game/PackConfig")]
    public class PackConfig : BaseConfig
    {
        public string bundleId;
        public string packName;
        public string badge;
        public List<ResourceConfig> resources;

        private Dictionary<ResourceType, int> m_DictResources;

        public int GetResourceValueByType(ResourceType type)
        {
            m_DictResources ??= resources.ToDictionary(x => x.type, x => x.value);
            if (m_DictResources.TryGetValue(type, out var value)) return value;
            Debug.LogError("Invalid resource type");
            return -1;
        }
    }

    public class ResourceConfig
    {
        public ResourceType type;
        public int value;
    }
}