using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Event/Events List Service")]
public class EventsService : ScriptableObject
{
    public EventObject INIT_DONE;
    public EventObject LOGIN_DONE;
    public EventObject FIREBASE_LOADED;
}
