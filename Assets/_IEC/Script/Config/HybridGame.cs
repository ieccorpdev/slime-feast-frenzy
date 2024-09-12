using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public enum TabType
    {
        Shop, Tab2, Home, Tab4, Tab5
    }
    
    public enum ResourceType
    {
        Currency1, 
        Currency2, 
        Live,
        Item1,
        Item2,
        Item3,
        Item4
    }
    
    [Serializable]
    public class HybridGame
    {
        public TabType currentTab;
    }

    [Serializable]
    public class HybridData
    {
        public Dictionary<ResourceType, int> DictResource = new Dictionary<ResourceType, int>();

        public void AddResource(ResourceType resource, int value)
        {
            DictResource.TryAdd(resource, 0);
            DictResource[resource] += value;
            DictResource[resource] = Mathf.Max(DictResource[resource], 0);
        }
    }
}