using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class BannerAdsUI : BaseBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            
        }

        public void SetSize(float size)
        {
            if (gameObject.TryGetComponent<RectTransform>(out var rect))
            {
                var sizeDelta = rect.sizeDelta;
                sizeDelta.y = size;
                sizeDelta.y = Mathf.Clamp(sizeDelta.y, 100, 400);
                rect.sizeDelta = sizeDelta;
            }

            if (gameObject.TryGetComponent<LayoutElement>(out var element))
            {
                element.minHeight = Mathf.Clamp(size, 100, 400);
            }
        }
    }
}