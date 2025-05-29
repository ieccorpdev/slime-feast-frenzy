using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public enum TabType
    {
        Shop,
        Tab2,
        Home,
        LeaderBoard,
        Tab5
    }

    public enum ResourceType
    {
        Gold,
        Currency2,
        Live,
        UnlimitedLive,
        Enlarge,
        Vortex,
        Compass,
        TimeStop,     
        BonusTime,
        BonusExp,
        TimeUnlimitedEvolveStone,
        TimeUnlimitedWindSurf,
        DoubleSpecialObjectCount,
        EvolveStone,
        WindSurf,
        LevelStar,
        OceanGiftObject,
        Relic,
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
        private Dictionary<ResourceType, Vector2Int> m_DictResourceLimit = new Dictionary<ResourceType, Vector2Int>();
        public int timeRefillLeft;
        public int timeUnlimitedLive;

        public void AddResource(ResourceType resource, int value)
        {
            // if (resource == ResourceType.Live)
            // {
            //     Debug.LogError("AddResource called on live game");
            //     Debug.LogError(m_DictResourceLimit[resource]);
            // }
            DictResource.TryAdd(resource, 0);
            var resourceCountBeforeAdd = DictResource[resource];
            DictResource[resource] += value;
            m_DictResourceLimit.TryAdd(resource, new Vector2Int(0, -1));
            DictResource[resource] = Mathf.Max(DictResource[resource], m_DictResourceLimit[resource].x);
            if (m_DictResourceLimit[resource].y > 0)
            {
                DictResource[resource] = Mathf.Min(DictResource[resource], m_DictResourceLimit[resource].y);
            }

            if (resource == ResourceType.UnlimitedLive && resourceCountBeforeAdd > 0 && DictResource[resource] <= 0)
            {
                DictResource.TryAdd(ResourceType.Live, 0);
                AddResource(ResourceType.Live, m_DictResourceLimit[ResourceType.Live].y);
            }

            EventController.RaiseEvent(EventList.GamePlay.UPDATE_RESOURCE_UI, new EventParam { p0 = resource });
        }

        public int GetResourceCount(ResourceType resource)
        {
            DictResource.TryAdd(resource, 0);
            return DictResource[resource];
        }

        public bool IsCanPlayWithLife()
        {
            if (GetResourceCount(ResourceType.UnlimitedLive) > 0)
            {
                return true;
            }

            var lifeCount = GetResourceCount(ResourceType.Live);
            return lifeCount > 0;
        }

        public bool IsNotFullLives()
        {
            m_DictResourceLimit.TryGetValue(ResourceType.Live, out var lifeLimit);
            return GetResourceCount(ResourceType.Live) < lifeLimit.y;
        }

        public bool IsOutOfLives()
        {
            return GetResourceCount(ResourceType.Live) <= 0;
        }

        public bool IsUnlitmitedLives()
        {
            return GetResourceCount(ResourceType.UnlimitedLive) > 0;
        }

        public void SetResourceLimit(ResourceType type, int min, int max)
        {
            m_DictResourceLimit[type] = new Vector2Int(min, max);
        }
    }
}