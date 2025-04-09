using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class ResourceUI : ScreenBehaviour
    {
        protected ResourceType type;
        protected Action onClick;
        [SerializeField] private Button m_Btn;
        [SerializeField] private TMP_Text m_Text;

        private void Awake()
        {
            Init();
        }

        public void SetText(string str)
        {
            m_Text.text = str;
        }

        protected virtual void Init()
        {
            SetEvent(m_Btn, () => onClick?.Invoke());
        }

        protected virtual void UpdateUI()
        {
            
        }
    }
}