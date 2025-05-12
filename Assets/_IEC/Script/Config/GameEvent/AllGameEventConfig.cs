using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    [CreateAssetMenu(menuName = "Game/AllGameEventConfig")]
    public class AllGameEventConfig : ScriptableObject
    {
        public List<GameEventConfig> listAllGameEventConfig = new();

        public GameEventConfig GetGameEventConfig(EventType eventType)
        {
            return listAllGameEventConfig.Find(config => config.eventType == eventType);
        }
    }
}
