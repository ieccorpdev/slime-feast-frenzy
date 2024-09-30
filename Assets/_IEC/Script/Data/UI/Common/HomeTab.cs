using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace IEC
{
    public class HomeTab : _BaseBehaviour
    {
        public TabType type;
        public Action onClick;
        [SerializeField] private Image m_Icon;
        [SerializeField] private GameObject m_Lock;
        private Button m_Btn => SingleGet<Button>();
        private bool m_IsLock;

        private void Start()
        {
            m_Btn.onClick.AddListener(()=>
            {
                if(m_IsLock) return;
                onClick?.Invoke();
            });
        }

        public void SetData(TabConfig config, bool isOn)
        {
            type = config.type;
            m_Icon.sprite = isOn ? config.spriteOn : config.spriteOff;
        }

        public void SetLock(bool isLock)
        {
            m_IsLock = isLock;
            m_Icon.gameObject.SetActive(!isLock);
            m_Lock.SetActive(isLock);
        }
    }
}