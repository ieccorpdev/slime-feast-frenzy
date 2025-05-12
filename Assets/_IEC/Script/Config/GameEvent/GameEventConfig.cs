using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    [CreateAssetMenu(menuName = "Game/GameEvent/GameEventConfig")]
    public class GameEventConfig : ScriptableObject
    {
        public int levelUnlock;
        public EventType eventType;
        public float eventDurationInHours;
        public float timeUpdateEventProgressInHours;

        public List<EventData> listEventData = new();

        public EventData GetEventData(int index)
        {
            index = Mathf.Clamp(index, 0, listEventData.Count - 1);
            return listEventData[index];
        }
    }

    [System.Serializable]
    public class EventData
    {
        public int eventTarget;
        public List<EventBotData> listEventBotData = new();
    }

    [System.Serializable]
    public class EventBotData
    {
        public float baseSpeed;
        public float speedRange;
    }
}
