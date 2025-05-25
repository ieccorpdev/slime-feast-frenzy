using IEC;
using UnityEngine;
using UnityEngine.EventSystems;

public class PointerDownEventHandler : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private string methodNameToCall = "PlayTapMusic";

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!string.IsNullOrEmpty(methodNameToCall))
        {
            Invoke(methodNameToCall, 0f);
        }
    }

    private void PlayTapMusic()
    {
        EventController.RaiseEvent(EventList.Audio.HOMEPAGE_TAP);
    }
}