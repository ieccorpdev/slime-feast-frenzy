using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    [CreateAssetMenu(menuName = "Game/Event/Events List")]
    public class EventsList : ScriptableObject
    {
        public EventsService services;
        public EventObject NONE;
    }
}
        