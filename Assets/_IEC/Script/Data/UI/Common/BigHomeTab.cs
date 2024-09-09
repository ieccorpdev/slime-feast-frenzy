using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class BigHomeTab : HomeTab
    {
        [SerializeField] private TMP_Text m_TabName;
        private LayoutElement m_Layout => SingleGet<LayoutElement>();
        private RectTransform m_Rect => SingleGet<RectTransform>();

        public void SetTabName()
        {
            m_TabName.text = type.ToString();
        }

        public void SetSize(int numTab)
        {
            m_Layout.minWidth = numTab < 5 ? 0 : 130f;
        }

        public Vector2 GetSize()
        {
            return m_Rect.sizeDelta;
        }

        public float GetPosX()
        {
            return m_Rect.anchoredPosition.x;
        }
    }
}