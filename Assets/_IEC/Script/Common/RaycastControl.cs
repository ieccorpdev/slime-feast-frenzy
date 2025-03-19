using UnityEngine;
using UnityEngine.EventSystems;

public class RaycastControl : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Button clicked!");
    }
}