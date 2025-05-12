using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    [CreateAssetMenu(menuName = "Game/GameEvent/GameEventConfig")]
    public class GameEventConfig : ScriptableObject
    {
        public EventType eventType;
        public float eventDurationInHours;
    }
}
