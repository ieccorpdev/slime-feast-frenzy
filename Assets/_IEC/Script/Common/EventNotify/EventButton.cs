using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace IEC
{
    [RequireComponent(typeof(Button))]
    public class EventButton : MonoBehaviour
    {
        [SerializeField]
        private EventObject m_EventObj;

        // Use this for initialization
        void Start()
        {
            var button = GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() =>
                {
                    var controller = Controllers.Get<EventController>();
                    var events = Controllers.Get<EventsList>();
                    var param = new EventParam();
                    param.sender = gameObject;
                    controller.RaiseEvent(m_EventObj, param);
                });
            }
        }
    }

}