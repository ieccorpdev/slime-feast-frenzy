using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    [RequireComponent(typeof(Button))]
    public class CommonButton : MonoBehaviour
    {
        private Button m_Button;

        private Button button
        {
            get
            {
                if (m_Button == null) m_Button = GetComponent<Button>();
                return m_Button;
            }
        }

        private string m_EventName;

        public void SetEvent(string eventName)
        {
            m_EventName = eventName;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => { EventController.RaiseEvent(m_EventName); });
        }
    }
}