using EnhancedUI.EnhancedScroller;
using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IEC.MapScroller
{
    public class BaseMapBigItem : EnhancedScrollerCellView
    {
        public GameObject header;
        public GameObject footer;

        public virtual int ScrollIndex { get; protected set; }
        public EnhancedScroller Scroller { get; protected set; }

        Image TestImage => SingleGet<Image>();

        List<Color> m_TestColors = new List<Color>()
        {
            Color.red, Color.green, Color.blue, Color.yellow
        };

        public virtual void SetData(int scrollIndex)
        {
            ScrollIndex = scrollIndex;
            
            if (TestImage != null)
            {
                gameObject.name = $"BigItem-{scrollIndex}";
                TestImage.color = m_TestColors[scrollIndex % m_TestColors.Count];
            }
        }

        public virtual void SetHeader(bool isActive)
        {
            SetActive(header, isActive);
        }

        public virtual void SetFooter(bool isActive)
        {
            SetActive(footer, isActive);
        }

        public void SetScroller(EnhancedScroller scroller)
        {
            Scroller = scroller;
        }
    }
}