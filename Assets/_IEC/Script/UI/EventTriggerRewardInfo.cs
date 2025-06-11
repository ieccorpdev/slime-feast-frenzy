using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(EventTrigger))]
public class EventTriggerRewardInfo : MonoBehaviour
{
    private EventTrigger trigger;

    private void Awake()
    {
        trigger = GetComponent<EventTrigger>();
        SetUpEventTrigger();
    }

    private void SetUpEventTrigger()
    {
        EventTrigger.Entry entry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerDown
        };
        entry.callback.AddListener((data) => {
            var currentInfo = RewardInfoGroup.currentInfo;
            if (currentInfo)
            {
                currentInfo.SetActive(false);
            }
        });
        trigger.triggers.Add(entry);
    }

}
