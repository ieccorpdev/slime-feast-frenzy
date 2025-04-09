using _IEC.Features.Common.Extension;
using IEC;
using UnityEngine;

namespace _IEC.Features.VipProHackerUITutorial
{
    public class WaitForEvent : CustomYieldInstruction
    {
        private readonly string m_EventID;
        private bool m_IsKeeping = true;
        public WaitForEvent(string eventID)
        {
            m_EventID = eventID;
            this.RegisterListener(m_EventID,OnEventRaise);
        }

        private void OnEventRaise(object param)
        {
            this.UnregisterListener(m_EventID,OnEventRaise);
            m_IsKeeping = false;
        }
        public override bool keepWaiting => m_IsKeeping;
    }
}