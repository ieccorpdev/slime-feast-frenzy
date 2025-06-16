using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class ToggleSetting : _BaseBehaviour
    {
        public Button btn;
        [SerializeField] private GameObject m_ToggleOn, m_ToggleOff;

        public void SetStateUI(bool state)
        {
            m_ToggleOn.SetActive(state);
            m_ToggleOff.SetActive(!state);
        }
    }
}