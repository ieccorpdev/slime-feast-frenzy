using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace IEC
{
    public class UIOnClickScaler : _BaseBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private Button m_Button;

        private void Start()
        {
            m_Button = GetComponent<Button>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (m_Button == null)
                return;
            
            if (!m_Button.interactable)
                return;
            
            transform.DOScale(Vector3.one * 0.8f, 0.1f).SetEase(Ease.InQuad);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (m_Button == null)
                return;

            if (!m_Button.interactable)
                return;
            
            transform.DOScale(Vector3.one, 0.1f).SetEase(Ease.OutQuad);
        }
    }
}