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
        [SerializeField] private Image m_IconNoti;
        [SerializeField] private GameObject m_Lock;
        private Button m_Btn => SingleGet<Button>();
        private bool m_IsLock;
        public bool isBigTabOn;
 
        private float localPosIconOff = -35;
        private void Start()
        {
            m_Btn.onClick.AddListener(()=>
            {
                if(m_IsLock) return;
                onClick?.Invoke();
            });
            
            SetActiveNoti(false);
        }

        public void Init()
        {
            SetSpineIcon();

        }

        private void SetSpineIcon()
        {
          
        }

        public void SetData(TabConfig config, bool isOn,TabType lastTabType)
        {
            type = config.type;
            //m_Icon.sprite = isOn ? config.spriteOn : config.spriteOff;
            //m_Icon.SetNativeSize();
            if (!isOn)
            {
                SetAnimationSpineOff(lastTabType);
            }
        }
        
        public void SetLock(bool isLock)
        {
            m_IsLock = isLock;
            //m_Icon.gameObject.SetActive(!isLock);
            //if (iconSpine) iconSpine.gameObject.SetActive(!isLock);
            m_Lock.SetActive(isLock);
            if(isLock) m_Lock.transform.SetAsLastSibling();
            
        }

        public void SetActiveNoti(bool isShowNoti)
        {
            if (m_IconNoti == null)
                return;
            
            m_IconNoti.gameObject.SetActive(isShowNoti);
        }
        public void SetAnimationSpineOn()
        {
            SetSpineIcon();
       
        }
        public void SetAnimationSpineOff(TabType lastTabType)
        {
            SetSpineIcon();

          
        }
    }
}